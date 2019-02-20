using System.Collections.Generic;
using System.Threading.Tasks;
using AgGridApi.Services;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace AgGridApi.Controllers
{
    [Route("api/aggrid")]
    [ApiController]
    public class AgGridController : ControllerBase
    {

        private readonly IDemo _demo;

        public AgGridController(IDemo demo)
        {
            _demo = demo;
        }

        [HttpGet]
        [Route("GetData")]
        public async Task<string> GetData()
        {
            return await Task.Run(() => _demo.GetDemoDataSource());
        }

        [HttpGet]
        [Route("GetDataColumns")]
        public async Task<string> GetDataColumns()
        {
            return await Task.Run(() => _demo.GetDemoDataColumns());
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
