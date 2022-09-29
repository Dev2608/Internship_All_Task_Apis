using DataBaseConnection;
using AllProjectCombine.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using System.Data;
using System.Collections.Generic;
using Microsoft.Ajax.Utilities;

namespace DmartAPI.Controllers
{
    [RoutePrefix("api/InsertProduct")]
    public class InsertProductController : ApiController
    {
        connect ob1 = new connect();
        FinalDBEntities db = new FinalDBEntities();


        [Route("SelectAllWithoutAnyParam")]
        [HttpGet]
        public IHttpActionResult SelectAllWithoutAnyParam()
        {
            ob1.connection();
            DataSet ds1 = ob1.spSelectData("spGetPurchaseId");

            var list1 = ds1.Tables[0].AsEnumerable().Select(dataRow => new purchase_table
            {
                purchase_id = dataRow.Field<int>("purchase_id"),
                purchase_number = dataRow.Field<string>("purchase_number"),
                purchase_date = dataRow.Field<DateTime>("purchase_date"),
                total_amount = dataRow.Field<double>("total_amount")
            }).ToList();

            int tmpPurId;

            List<List<purchaseProductTable>> finalList = new List<List<purchaseProductTable>>();

            foreach (var item in list1)
            {
                tmpPurId = item.purchase_id;

                DataSet ds2 = ob1.spSelectData($"spGetAllProductsById {tmpPurId}");

                var list2 = ds2.Tables[0].AsEnumerable().Select(dataRow => new purchaseProductTable
                {
                    purchase_product_id = dataRow.Field<int>("purchase_product_id"),
                    purchase_id = dataRow.Field<int>("purchase_id"),
                    item_name = dataRow.Field<string>("item_name"),
                    qty = dataRow.Field<int>("qty"),
                    amount = dataRow.Field<double>("amount")
                }).ToList();

                item.list = list2;

                //finalList.Add(list2);
            }
            return Ok(list1);
        }

        [Route("selectAllDetailByOneSp")]
        [HttpGet]
        public IHttpActionResult selectAllDetailByOneSp()
        {
            ob1.connection();
            DataSet ds = ob1.spSelectData("spGetAllDetailByOne");

            var list2 = ds.Tables[1].AsEnumerable().Select(dataRow => new purchaseProductTable
            {
                purchase_product_id = dataRow.Field<int>("purchase_product_id"),
                purchase_id = dataRow.Field<int>("purchase_id"),
                item_name = dataRow.Field<string>("item_name"),
                qty = dataRow.Field<int>("qty"),
                amount = dataRow.Field<double>("amount")
            }).ToList();

            var list1 = ds.Tables[0].AsEnumerable().Select(dataRow => new purchase_table
            {
                purchase_id = dataRow.Field<int>("purchase_id"),
                purchase_number = dataRow.Field<string>("purchase_number"),
                purchase_date = dataRow.Field<DateTime>("purchase_date"),
                total_amount = dataRow.Field<double>("total_amount"),
                list = list2.Where(a => a.purchase_id == dataRow.Field<int>("purchase_id")).ToList()
            }).ToList();

            return Ok(list1);
        }


        // using inner join to join both tables.
        // used only one list to get the output
        [Route("selectAllDetailByOneSpAndOneList")]
        [HttpGet]
        public IHttpActionResult selectAllDetailByOneSpAndOneList()
        {
            ob1.connection();
            DataSet ds = ob1.spSelectData("spFinalSelectFromBothTable");

            var list1 = ds.Tables[0].AsEnumerable().Select(dataRow => new purchase_table
            {
                purchase_id = dataRow.Field<int>("purchase_id"),
                purchase_number = dataRow.Field<string>("purchase_number"),
                purchase_date = dataRow.Field<DateTime>("purchase_date"),
                total_amount = dataRow.Field<double>("total_amount"),
                list = ds.Tables[0].AsEnumerable().Select(dataRow1 => new purchaseProductTable
                {
                    purchase_product_id = dataRow1.Field<int>("purchase_product_id"),
                    purchase_id = dataRow1.Field<int>("purchase_id"),
                    item_name = dataRow1.Field<string>("item_name"),
                    qty = dataRow1.Field<int>("qty"),
                    amount = dataRow1.Field<double>("amount")
                }).Where(temp => temp.purchase_id == dataRow.Field<int>("purchase_id")).ToList()
            }).DistinctBy(t => t.purchase_id).ToList();

            return Ok(list1);
        }

        // insert API
        // this will take all the detail of product transaction

        [Route("InsertProduct")]
        [HttpPost]
        public IHttpActionResult InsertProduct([FromBody] purchaseProductTable[] data)
        {
            int len = data.Length;

            double TOTAL_AMOUNT = 0.0;

            purchase_table obj = new purchase_table();

            obj.purchase_date = DateTime.Now;
            obj.purchase_number = ob1.generateUniqueNumber();

            int PUR_ID;

            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < len; i++)
                    {
                        TOTAL_AMOUNT = TOTAL_AMOUNT + data[i].amount;
                    }
                    obj.total_amount = TOTAL_AMOUNT;

                    db.purchase_table.Add(obj);
                    db.SaveChanges();

                    PUR_ID = obj.purchase_id;

                    for (int i = 0; i < len; i++)
                    {
                        data[i].purchase_id = PUR_ID;
                        db.purchaseProductTables.Add(data[i]);
                        db.SaveChanges();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }
            return Ok();
        }

        [Route("DeleteProductByProductId")]
        [HttpDelete]
        public IHttpActionResult DeleteProductByProductId([FromBody] int data)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var temp = db.purchase_table.Where(t => t.purchase_id == data).FirstOrDefault();
                    db.Entry(temp).State = EntityState.Deleted;
                    db.SaveChanges();

                    db.purchaseProductTables.RemoveRange(db.purchaseProductTables.Where(t => t.purchase_id == data));
                    db.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    return (IHttpActionResult)ex;
                }
            }
            return Ok();
        }

        [Route("DeleteProductInpurchaseProductTable")]
        [HttpDelete]
        public IHttpActionResult DeleteProductInpurchaseProductTable([FromBody] int ID)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var temp1 = db.purchaseProductTables.Where(t => t.purchase_product_id == ID).FirstOrDefault();
                    double d_amount = temp1.amount;

                    db.Entry(temp1).State = EntityState.Deleted;
                    db.SaveChanges();

                    var temp = db.purchase_table.Where(t => t.purchase_id == temp1.purchase_id).FirstOrDefault();
                    temp.total_amount -= d_amount;
                    db.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    return (IHttpActionResult)ex;
                }
            }
            return Ok();
        }

        // data[0] = purchase_product_id
        // data[1] = qty
        // data[2] = updated amount
        [Route("updateProductDetail")]
        [HttpPut]
        public IHttpActionResult updateProductDetail([FromBody] int[] data)
        {
            int pur_product_id = data[0];
            int QTY = data[1];
            double amount = data[2];

            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (data[1] <= 0)
                    {
                        var temp1 = db.purchaseProductTables.Where(t => t.purchase_product_id == pur_product_id).FirstOrDefault();
                        db.Entry(temp1).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();

                        transaction.Commit();

                        return Ok();
                    }
                    else
                    {
                        var temp1 = db.purchaseProductTables.Where(t => t.purchase_product_id == pur_product_id).FirstOrDefault();

                        double d_amount = temp1.amount;

                        temp1.qty = QTY;
                        temp1.amount = amount;
                        db.SaveChanges();

                        var temp = db.purchase_table.Where(t => t.purchase_id == temp1.purchase_id).FirstOrDefault();
                        temp.total_amount -= d_amount;
                        temp.total_amount += amount;
                        db.SaveChanges();

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    return (IHttpActionResult)ex;
                }
            }
            return Ok();
        }
    }
}
