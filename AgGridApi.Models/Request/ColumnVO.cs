using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgGridApi.Models.Request
{
    public class ColumnVO
    {
        public String Id { get; set; }
        public String DisplayName { get; set; }
        public String Field { get; set; }
        public String AggFunc { get; set; }

        public ColumnVO()
        {
        }

        public ColumnVO(String id, String displayName, String field, String aggFunc)
        {
            this.Id = id;
            this.DisplayName = displayName;
            this.Field = field;
            this.AggFunc = aggFunc;
        }


    }
}
