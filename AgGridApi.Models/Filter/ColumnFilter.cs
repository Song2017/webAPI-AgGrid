using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgGridApi.Models.Filter
{

    [JsonConverter(typeof(JsonSubtypes))]
    [JsonSubtypes.KnownSubTypeWithProperty(typeof(NumberColumnFilter), "filterTo")]
    [JsonSubtypes.KnownSubTypeWithProperty(typeof(SetColumnFilter), "Type")]
    public class ColumnFilter
    {
        public String FilterType { get; set; }
    }
}
