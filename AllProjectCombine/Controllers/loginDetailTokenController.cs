using AllProjectCombine.Models;
using DataBaseConnection;
using System;
using System.Linq;
using System.Web.Http;
using System.Data;

namespace WebApiOauth.Controllers
{
    public class loginDetailTokenController : ApiController
    {
        connect ob1 = new connect();

        // select
        [Route("api/loginDetailToken/selectStudent")]
        [Authorize]
        [HttpGet]
        public IHttpActionResult selectStudent()
        {
            try
            {
                ob1.connection();
                DataSet ds = ob1.selectStudentOAuth("select * from facultyDetail");

                var list = ds.Tables[0].AsEnumerable().Select(dataRow => new facultyDetail
                {
                    f_id = dataRow.Field<String>("f_id"),
                    s_id = dataRow.Field<String>("s_id")
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
