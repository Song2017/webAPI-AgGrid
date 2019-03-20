using System;
using System.Collections.Generic;
using System.Text;

namespace AgGridApi.Common
{
    public static class Constants
    {
        public const string WHITESPACE = @" ";
        public const string COMMA = @","; 
    }


    public static class FilterConditionType {
        public const string TEXT = "text";
        public const string NUMBER = "number";
        public const string DATE = "date";
        public const string SET = "set";
    }


    public static class FilterType {

        public const string EQUALS = "equals";
        public const string NOT_EQUAL = "notEqual";
        public const string LESS_THAN = "lessThan";
        public const string LESS_THAN_OR_EQUAL = "lessThanOrEqual";
        public const string GREATER_THAN = "greaterThan";
        public const string GREATER_THAN_OR_EQUAL = "greaterThanOrEqual";
        public const string IN_RANGE = "inRange";
        public const string CONTAINS = "contains"; //1;
        public const string NOT_CONTAINS = "notContains"; //1;
        public const string STARTS_WITH = "startsWith"; //4;
        public const string ENDS_WITH = "endsWith"; //5;    
    }
}
