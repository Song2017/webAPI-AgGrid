using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using AgGridApi.Common;
using AgGridApi.Models.Request;
using AgGridApi.Models.Response;
using DataFactory;

namespace AgGridApi.Services
{
    public class AGServer : IAGServer
    {

        private DataServiceFactory _dataServiceSample;

        public AGServer()
        {
            DataFactory.DataFactory.ConnectionString = StaticConfigs.GetDBConfig("OracleConnectionString");
            DataFactory.DataFactory.SqlCommandTimeout = int.Parse(StaticConfigs.GetDBConfig("SqlCommandTimeout"));
            _dataServiceSample = new DataServiceFactory();
        }

        public ServerRowsResponse GetData(ServerRowsRequest request)
        {
            int pageCount = 0;
            
            DataTable dtTable = _dataServiceSample.GetBigDataPage("AB5DBB3289A348AE87B415498B02749C", 0, string.Empty,
                 request.PageIndex + 1, request.PageSize, ref pageCount);

            return ResponseBuilder(dtTable, pageCount);
        }

        public ServerRowsResponse GetData(IRequestBuilder requestBuilder)
        {
            int pageCount = 0;

            DataTable dtTable = _dataServiceSample.GetBigDataPage("AB5DBB3289A348AE87B415498B02749C", 0,
                 requestBuilder.GetSorts(), requestBuilder.GetPageIndex(), requestBuilder.GetPageSize(), ref pageCount);

            return ResponseBuilder(dtTable, pageCount);
        }

        private ServerRowsResponse ResponseBuilder(DataTable dtTable, int pageCount)
        {
            ServerRowsResponse rowsResponse = new ServerRowsResponse
            {
                LastRow = pageCount,
                Data = dtTable
            };

            return rowsResponse;
        }
 
    }
}
