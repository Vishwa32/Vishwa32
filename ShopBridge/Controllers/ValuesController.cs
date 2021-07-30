using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ShopBridge.Controllers
{
    public class ValuesController : ApiController
    {

        List<Stud> studs = new List<Stud>()
            {
                new Stud{ Id=1,Name="Vishwa"},
                new Stud{ Id=2,Name="Bharat"}
            };
        // GET api/values
        public async Task<List<Stud>> Get()
        {
            var students = studs.ToList();
            return students;
        }

        // GET api/values/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }




        public async Task<Stud> getData(int id)
        {

            Mobile mobile = new Stud();

            mobile.CallFunctionality();

            Stud stud = new Stud();
            stud.TextSMS();
            var stu = studs.Where(s => s.Id == id).FirstOrDefault();

            return stu;
        }
    }

    public abstract class Mobile
    {
        public abstract void CallFunctionality();

        public abstract void TextSMS();
    }

    public class Stud:Mobile
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public override void CallFunctionality()
        {
            string title = "CallFunctionality()";
            title = title +" ";
        }

        public override void TextSMS()
        {
            string title = "TextSMS()";
            title = title + " ";
            //throw new NotImplementedException();
        }
    }

   
}
