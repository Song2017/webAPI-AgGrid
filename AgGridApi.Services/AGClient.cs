using Newtonsoft.Json;
using System.Data;
using System.Linq;

using DataFactory;
using AgGridApi.Common;

namespace AgGridApi.Services
{
    public class AGClient : IAGClient
    {
        private DataServiceFactory _dataServiceSample;

        public AGClient() {
            DataFactory.DataFactory.ConnectionString = StaticConfigs.GetDBConfig("OracleConnectionString");
            DataFactory.DataFactory.SqlCommandTimeout = int.Parse(StaticConfigs.GetDBConfig("SqlCommandTimeout"));
            _dataServiceSample = new DataServiceFactory();
        }

        public string GetDemoDataSource()
        {
            DataTable dtTable = _dataServiceSample.GetNotes("AB5DBB3289A348AE87B415498B02749C", 'T');
            return JsonConvert.SerializeObject(dtTable);
        }

        public string GetDemoDataColumns(string datasource)
        {
            DataTable dataTable = _dataServiceSample.GetGridField("AB5DBB3289A348AE87B415498B02749C", datasource);
            return JsonConvert.SerializeObject(dataTable.GetHeader(0, 2));
        }
        
    }
}
