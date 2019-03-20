using AgGridApi.Models.Filter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgGridApi.Models.Request
{
    public class ServerRowsRequest
    {
        // start from 0
        [JsonProperty("pageIndex", NullValueHandling = NullValueHandling.Ignore)]
        public int PageIndex { set; get; }
        [JsonProperty("pageSize", NullValueHandling = NullValueHandling.Ignore)]
        public int PageSize { set; get; }

        public int StartRow { set; get; }

        public int EndRow { set; get; }

        // row group columns
        public List<ColumnVO> RowGroupCols { set; get; }

        // value columns
        public List<ColumnVO> ValueCols { set; get; }

        // pivot columns
        public List<ColumnVO> PivotCols { set; get; }

        // true if pivot mode is one, otherwise false
        public Boolean IsPivotMode { set; get; }

        // what groups the user is viewing
        public List<String> GroupKeys { set; get; }


        [JsonProperty("filterModel")]
        // if filtering, what the filter model is
        public List<ColumnFilter> FilterModels { get; set; }

        [JsonProperty("sortModel")]
        // if sorting, what the sort model is
        public List<SortModel> SortModels { set; get; }

        public ServerRowsRequest()
        {
            RowGroupCols = new List<ColumnVO>();
            ValueCols = new List<ColumnVO>();
            PivotCols = new List<ColumnVO>();
            GroupKeys = new List<String>();
            FilterModels = new List<ColumnFilter>();
            SortModels = new List<SortModel>();
        }
    }
}
