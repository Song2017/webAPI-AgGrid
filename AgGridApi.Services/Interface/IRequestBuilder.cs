using AgGridApi.Common;
using AgGridApi.Models.Filter;
using AgGridApi.Models.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgGridApi.Services
{
    public interface IRequestBuilder
    {
        void AssignRequest(ServerRowsRequest request);

        int GetPageIndex();

        int GetPageSize();

        String GetFilters();

        String GetSorts();
    }

}
