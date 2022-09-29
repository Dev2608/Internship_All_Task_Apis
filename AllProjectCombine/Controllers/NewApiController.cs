using AllProjectCombine.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace FirstAPI.Controllers
{
    [RoutePrefix("api/NewApi")]
    public class NewApiController : ApiController
    {        
        FinalDBEntities DB = new FinalDBEntities();

        [Route("GetAllStudents")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetAllStudents()
        {
            List<student> obj = DB.students.ToList();
            return Ok(obj);
        }

        // Select By Id.
        [Route("GetStudentsById")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetStudentsById(String id)
        {
            var obj = DB.students.Where(model => model.s_id.Equals(id)).FirstOrDefault();
            return Ok(obj);
        }

        // Insert
        [Route("InsertStudent")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult InsertStudent([FromBody] student s)
        {
            using(DbContextTransaction transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    DB.students.Add(s);
                    DB.SaveChanges();

                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }
            return Ok();
        }

        // Update
        [Route("UpdateStudent")]
        [System.Web.Http.HttpPut]
        public IHttpActionResult UpdateStudent([FromBody] student s)
        {
            using (DbContextTransaction transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    DB.Entry(s).State = System.Data.Entity.EntityState.Modified;
                    var temp = DB.students.Where(model => model.s_id.Equals(s.s_id)).FirstOrDefault();

                    if (temp != null)
                    {
                        temp.s_name = s.s_name;
                        temp.s_age = s.s_age;
                        temp.s_mobile = s.s_mobile;
                    }

                    DB.SaveChanges();

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

        // Delete
        [Route("DeleteStudent")]
        [System.Web.Http.HttpDelete]
        public IHttpActionResult DeleteStudent(String id)
        {
            using (DbContextTransaction transaction = DB.Database.BeginTransaction())
            {
                try
                {
                    var obj = DB.students.Where(model => model.s_id.Equals(id)).FirstOrDefault();

                    DB.Entry(obj).State = System.Data.Entity.EntityState.Deleted;

                    DB.SaveChanges();

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
