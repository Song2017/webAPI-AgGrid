using AgGridApi.Models.Request;
using AgGridApi.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgGridApi.Services
{
    public interface IAGServer
    {
        ServerRowsResponse GetData(ServerRowsRequest request);

        ServerRowsResponse GetData(IRequestBuilder requestBuilder);
    }
}
