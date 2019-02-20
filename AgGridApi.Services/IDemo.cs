using System;
using System.Threading.Tasks;

namespace AgGridApi.Services
{
    public interface IDemo
    {
        string GetDemoDataSource();

        string GetDemoDataColumns();
    }
}
