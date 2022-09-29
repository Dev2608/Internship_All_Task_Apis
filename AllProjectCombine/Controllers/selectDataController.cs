using AllProjectCombine.Models;
using DataBaseConnection;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace ApiGridView.Controllers
{
    [RoutePrefix("api/selectData")]
    public class selectDataController : ApiController
    {        
        connect ob1 = new connect();
        FinalDBEntities db = new FinalDBEntities();
        DateTime endDate = new DateTime();        

        // complete API with all 4 attributes
        // Data[0] = emp_id (OrderBy Parameter) Data[1] = 20(Number of rows on single page) and Data[2] = 2(page number) Data[3] = ASEC/DESC
        // Data[4] = nameToBeSearched in the table
        // in offset, the result is less then the offset number then no data will be displayed.
        [Route("selectEmployeeComplete")]
        [HttpGet]
        public IHttpActionResult selectEmployeeComplete([FromBody] String[] Data)
        {
            var temp1 = int.Parse(Data[1]);
            var temp2 = int.Parse(Data[2]);

            var temp = temp1 * temp2;
            temp = temp - temp1;
            var total = temp.ToString();
                
            try
            {
                ob1.connection();

                DataSet ds = ob1.selectEmployee($"spGetEmployeeByAllParam '{Data[4]}', '{Data[3]}', '{Data[0]}', {total}, {Data[1]}");

                // for converting the dataset object in list
                var list = ds.Tables[0].AsEnumerable().Select(dataRow => new employee1
                {
                    emp_id = dataRow.Field<int>("emp_id"),
                    emp_name = dataRow.Field<String>("emp_name"),
                    emp_email = dataRow.Field<String>("emp_email")
                });

                return Ok(list);
            }
            catch (Exception ex)
            {
                return (IHttpActionResult)ex;
            }
        }


        // insert API
        // this will take all the detail of employee
        // it will not take id bcoz it is identity
        // it will not take end date from the user, here end date will generating directly
        [Route("InsertEmployee")]
        [HttpPost]
        public IHttpActionResult InsertEmployee([FromBody] joinendEmployee data)
        {
            // this is for validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    endDate = data.join_date.AddYears(1);
                    data.end_date = endDate;

                    db.joinendEmployees.Add(data);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }
            return Ok("Ending Date : " + endDate);
        }

        // gives the list of employees whose end date is greater then the current date
        [Route("selectEmployeeWithEndDate")]
        [HttpGet]
        public IHttpActionResult selectEmployeeWithEndDate()
        {            
            try
            {
                ob1.connection();

                DataSet ds = ob1.selectEmployee("select * from joinendEmployee where end_date>GETDATE()");                    

                // for converting the dataset object in list
                var list = ds.Tables[0].AsEnumerable().Select(dataRow => new joinendEmployee
                {
                    emp_id = dataRow.Field<int>("emp_id"),
                    emp_name = dataRow.Field<String>("emp_name"),
                    emp_email = dataRow.Field<String>("emp_email"),
                    join_date = dataRow.Field<DateTime>("join_date"),
                    end_date = dataRow.Field<DateTime>("end_date")
                });

                return Ok(list);
            }
            catch (Exception ex)
            {
                return (IHttpActionResult)ex;
            }
        }
    }
}
