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

        private readonly IAGClient _aGClient;
        private readonly IAGServer _aGServer;
        private readonly IRequestBuilder _requestBuilder;

        public AgGridController(IAGClient demo, IAGServer aGServer, IRequestBuilder requestBuilder)
        {
            _aGClient = demo;
            _aGServer = aGServer;
            _requestBuilder = requestBuilder;
        }

        [HttpGet]
        [Route("GetData")]
        public async Task<string> GetData()
        {
            return await Task.Run(() => _aGClient.GetDemoDataSource());
        }

        [HttpGet]
        [Route("GetDataColumns/{datasource}")]
        public async Task<string> GetDataColumns(string datasource)
        {
            return await Task.Run(() => _aGClient.GetDemoDataColumns(datasource));
        }

        [HttpPost]
        [Route("GetAllData")]
        public ServerRowsResponse JsonStringBody([FromBody] ServerRowsRequest request)
        {
            _requestBuilder.AssignRequest(request);
            return _aGServer.GetData(_requestBuilder);
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
