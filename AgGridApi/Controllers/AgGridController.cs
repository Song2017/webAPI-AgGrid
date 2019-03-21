using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgGridApi.Common;
using AgGridApi.Models.Request;
using AgGridApi.Models.Response;
using AgGridApi.Services;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace AgGridApi.Controllers
{
    [Route("api/aggrid")]
    [ApiController]
    public class AgGridController : ControllerBase
    {

        private readonly IAGServer _aGServer;
        private readonly IRequestBuilder _requestBuilder;

        public AgGridController(IAGServer aGServer, IRequestBuilder requestBuilder)
        {
            _aGServer = aGServer;
            _requestBuilder = requestBuilder;
        }

        [HttpGet]
        [Route("GetDataColumns/{datasource}")]
        public Task<string> GetDataColumns(string datasource)
        {
            return Task.Run(() => _aGServer.GetDataColumns(datasource));
        }

        [HttpPost]
        [Route("GetAllData")]
        public Task<ServerRowsResponse> GetAllData([FromBody] ServerRowsRequest request)
        {
            return Task.Run(() =>
            _aGServer.GetData(_requestBuilder.GetRequestBuilder(request)));
        }

        #region operate

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
        #endregion
    }
}
