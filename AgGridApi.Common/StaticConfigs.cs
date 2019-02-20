using System.IO;
using Microsoft.Extensions.Configuration;

namespace AgGridApi.Common
{
    public class StaticConfigs
    {
        //Read key and get value from AppConfig section of AppSettings.json.
        public static string GetDBConfig(string keyName)
        {
            var rtnValue = string.Empty;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("dbsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            var value = configuration["DataFactorySetting:" + keyName];
            if (!string.IsNullOrEmpty(value))
            {
                rtnValue = value;
            }
            return rtnValue;
        }
    }
}
