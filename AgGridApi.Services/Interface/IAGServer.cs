using AgGridApi.Models.Request;
using AgGridApi.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgGridApi.Services
{
    public interface IAGServer
    { 
        ServerRowsResponse GetData(IRequestBuilder requestBuilder);
    }
}
