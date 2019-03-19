using AgGridApi.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgGridApi.Models.Request
{
    public class ServerRowsRequest
    {
        // start from 0
        public int PageIndex { set; get; }

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

        // if filtering, what the filter model is
        public Dictionary<String, ColumnFilter> FilterModel { set; get; }

        // if sorting, what the sort model is
        public List<SortModel> SortModel { set; get; }

        public ServerRowsRequest()
        {
            this.RowGroupCols = new List<ColumnVO>();
            this.ValueCols = new List<ColumnVO>();
            this.PivotCols = new List<ColumnVO>();
            this.GroupKeys = new List<String>();
            this.FilterModel = new Dictionary<string, ColumnFilter>();
            this.SortModel = new List<SortModel>();
        }
    }
}
