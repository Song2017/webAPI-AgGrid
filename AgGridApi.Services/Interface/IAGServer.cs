using AgGridApi.Models.Request;
using AgGridApi.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AgGridApi.Services
{
    public interface IAGServer
    {
        string GetDataColumns(string datasource);

        ServerRowsResponse GetData(IRequestBuilder requestBuilder);
    }
}
