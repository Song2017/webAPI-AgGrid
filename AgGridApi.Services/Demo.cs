using Newtonsoft.Json;
using System.Data;
using System.Linq;

using DataFactory;
using AgGridApi.Common;

namespace AgGridApi.Services
{
    public class Demo : IDemo
    {
        private DataServiceFactory _dataServiceSample;

        public Demo() {
            DataFactory.DataFactory.ConnectionString = StaticConfigs.GetDBConfig("OracleConnectionString");
            DataFactory.DataFactory.SqlCommandTimeout = int.Parse(StaticConfigs.GetDBConfig("SqlCommandTimeout"));
            _dataServiceSample = new DataServiceFactory();
        }

        public string GetDemoDataSource()
        {
            DataTable dtTable = _dataServiceSample.GetNotesByOwnerPlant("AB5DBB3289A348AE87B415498B02749C", 'T');
            return JsonConvert.SerializeObject(dtTable);
        }

        public string GetDemoDataColumns()
        {
            DataTable dtTable = _dataServiceSample.GetNotesByOwnerPlant("AB5DBB3289A348AE87B415498B02749C", 'T');

            return JsonConvert.SerializeObject(dtTable.Columns.Cast<DataColumn>().
                Select(x => x.ColumnName).ToArray());
        }

         
    }
}
