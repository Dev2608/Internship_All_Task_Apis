using AllProjectCombine.Models;
using DataBaseConnection;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Security;

namespace LoginRegisterAPI.Controllers
{
    [RoutePrefix("api/Register")]
    public class RegisterController : ApiController
    {
        connect ob1 = new connect();
        FinalDBEntities db = new FinalDBEntities();

        [Route("selectStudent")]
        [Authorize]
        [HttpGet]
        public IHttpActionResult selectStudent()
        {
            try
            {
                ob1.connection();
                DataSet ds = ob1.select_Register("select * from register");

                var list = ds.Tables[0].AsEnumerable().Select(dataRow => new register
                {
                    name = dataRow.Field<String>("name"),
                    userid = dataRow.Field<String>("userid"),
                    password = dataRow.Field<String>("password"),
                    mobile = dataRow.Field<String>("mobile")
                });

                return Ok(list);
            }
            catch (Exception ex)
            {
                return (IHttpActionResult)ex;
            }
        }

        [Route("InsertStudent")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult InsertStudent([FromBody] register d)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (db.registers.Any(
                        loginDetail => loginDetail.mobile.Equals(d.mobile, StringComparison.OrdinalIgnoreCase)))
                    {
                        return BadRequest("User Already Exist!!");
                    }
                    else
                    {
                        MembershipUser n_user = Membership.CreateUser(d.userid, d.password);
                        db.registers.Add(d);
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
