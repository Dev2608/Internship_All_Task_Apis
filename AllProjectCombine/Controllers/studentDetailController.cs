using AllProjectCombine.Models;
using DataBaseConnection;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace facultyWebApi.Controllers
{
    [RoutePrefix("api/studentDetail")]
    public class studentDetailController : ApiController
    {
        connect ob1 = new connect();
        FinalDBEntities db = new FinalDBEntities();

        // select
        [Route("selectStudent")]
        [HttpGet]
        public IHttpActionResult selectStudent()
        {
            try 
            {
                ob1.connection();
                DataSet ds = ob1.selectStudent("spgetAllStudent");

                var list = ds.Tables[0].AsEnumerable().Select(dataRow => new facultyDetail
                {
                    f_id = dataRow.Field<String>("f_id"),
                    s_id = dataRow.Field<String>("s_id")
                });

                return Ok(list);
            }
            catch(Exception ex)
            {
                return (IHttpActionResult)ex;
            }
        }

        // Insert
        [Route("InsertStudent")]    // to call this api using method name
        [System.Web.Http.HttpPost]
        public IHttpActionResult InsertStudent([FromBody] student s)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (s.f_id != null)
                    {
                        facultyDetail f = new facultyDetail();

                        f.f_id = s.f_id;
                        f.s_id = s.s_id;
                        db.facultyDetails.Add(f);
                        db.SaveChanges();
                    }

                    db.students.Add(s);
                    db.SaveChanges();

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

        // insert JSON array
        [Route("InsertMultipleStudent")]      // to call this api using method name
        [HttpPost]
        public IHttpActionResult InsertMultipleStudent([FromBody] student[] s)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int l = s.Length;

                    for (int i = 0; i < l; i++)
                    {
                        if (s[i].f_id != null)
                        {
                            facultyDetail f = new facultyDetail();
                            f.f_id = s[i].f_id;
                            f.s_id = s[i].s_id;

                            db.facultyDetails.Add(f);
                        }
                        db.students.Add(s[i]);
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
    }
}
