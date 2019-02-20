using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFactory
{
    public class AppConstants
    {
       
        public const string NUL_VALUE = "NUL";

        public const string ALL_VALUE = "All";

        public const string CursorParam = "OUT_CURSOR";

        public static string SqlInjectionKeyword = @"^exec(\s+)|(\s+)exec(\s+)|[;']exec(\s+)|^execute(\s+)immediate|(\s+)execute(\s+)immediate" +
            @"|[;']execute(\s+)immediate|^select(\s+)|(\s+)select(\s+)|[;']select(\s+)|^insert(\s+)into|(\s+)insert(\s+)into|[;']" +
            @"insert(\s+)into|^delete(\s+)from|(\s+)delete(\s+)from|[;']delete(\s+)from|^drop(\s+)table(\s*)|[;']drop(\s+)table(\s*)|" +
            @"(\s+)drop(\s+)table(\s*)|^update(\s+)|(\s+)update(\s+)|[;']update(\s+)|^truncate(\s+)table(\s*)|(\s+)truncate(\s+)table(\s*)|" +
            @"[;']truncate(\s+)table(\s*)|^(create|drop)(\s+)(tablespace|user|table|view|index|procedure|function|trigger)|" +
            @"(\s+)(create|drop)(\s+)(tablespace|user|table|view|index|procedure|function|trigger)|" +
            @"[;'](create|drop)(\s+)(tablespace|user|table|view|index|procedure|function|trigger)|" +
            @"^create(\s+)or(\s+)replace|(\s+)create(\s+)or(\s+)replace|[;']create(\s+)or(\s+)replace|^alter(\s+)table|(\s+)alter(\s+)table|" +
            @"[;']alter(\s+)table|(\s+)asc$|(\s+)asc'|(\s+)asc(\s+)|substr(\s*)\(|chr(\s*)\(";
    }
}
