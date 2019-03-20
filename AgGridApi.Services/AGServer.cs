using System.Data;

using AgGridApi.Common;
using AgGridApi.Models.Response;
using DataFactory;
using Newtonsoft.Json;

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

        public string GetDataColumns(string datasource)
        {
            DataTable dataTable = _dataServiceSample.GetGridField("AB5DBB3289A348AE87B415498B02749C", datasource);
            return JsonConvert.SerializeObject(dataTable.GetHeader(0, 2));
        }

        public ServerRowsResponse GetData(IRequestBuilder requestBuilder)
        {
            int pageCount = 0;

            DataTable dtTable = _dataServiceSample.GetBigDataPage("AB5DBB3289A348AE87B415498B02749C", 0,
                requestBuilder.GetFilters(), requestBuilder.GetSorts(), requestBuilder.GetGroups(),
                requestBuilder.GetGroupWheres(), requestBuilder.GetPageIndex(), requestBuilder.GetPageSize(), ref pageCount);

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
