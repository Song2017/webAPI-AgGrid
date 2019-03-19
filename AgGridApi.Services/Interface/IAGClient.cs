using System;
using System.Threading.Tasks;

namespace AgGridApi.Services
{
    public interface IAGClient
    {
        string GetDemoDataSource();

        string GetDemoDataColumns(string datasource);
    }
}
