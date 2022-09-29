using AllProjectCombine.Models;
using DataBaseConnection;
using System;
using System.Data.Entity;
using System.Web.Http;

namespace NotMappedCheck.Controllers
{
    public class StudentDetailWithNotMappedController : ApiController
    {
        FinalDBEntities DB = new FinalDBEntities();

        [Route("api/StudentDetailWithNotMapped/InsertStudent")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult InsertStudent([FromBody] student s)
        {
            using (DbContextTransaction transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    if (s.f_id != null)
                    {
                        facultyDetail f = new facultyDetail();

                        f.f_id = s.f_id;
                        f.s_id = s.s_id;

                        DB.facultyDetails.Add(f);
                    }
                    DB.students.Add(s);

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
