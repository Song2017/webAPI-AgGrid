using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AgGridApi.Models.Response
{
    public class ServerRowsResponse
    {
        public DataTable Data { get; set; }
        public int LastRow { get; set; }
        public int TotalCount { get; set; }
        public List<String> SecondaryColumnFields { get; set; }

        public ServerRowsResponse() { }

        public ServerRowsResponse(DataTable data,  List<String> secondaryColumnFields, int lastRow, int totalCount)
        {
            this.Data = data;
            this.LastRow = lastRow;
            this.TotalCount = totalCount;
            this.SecondaryColumnFields = secondaryColumnFields;
        }

    }
}
