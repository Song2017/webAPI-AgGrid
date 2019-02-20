# AgGridAPI
This is a simple aggrid with .net core 2.2 web api framework, aim to use aggrid with web api on .net core.

## Front End
HTML5 + CSS + Ag-grid + JavaScript

## Server
Web API + ADO.Net(No EF)

## DataBase
Oracle: config connection in dbsettting.json

## App Context
1. 为了显示大数据量, 以及考虑到控件自身的功能丰富度和support, 前端使用[ag-grid](https://www.ag-grid.com/)控件
2. 为了容器化, 采用 [.net core 2.2](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-2.2)框架
3. 兼容移动端, 使用web api
4. 为了更好的利用已有的存储过程, 未使用EF, 通过 ADO.Net获取数据库数据.

## Project Intro
### AgGridApi 网站程序项目
1. 配置文件
	- launchSettings.json: 默认的程序启动配置文件
	- dbsettings.json: 配置数据库连接字符串
	- Startup.cs: 因为 .net core 疯狂的采用依赖注入, 当然确实挺好用的, startup文件是配置和注入服务的地方, 无论官方还是自定的服务.
		
		//add mvc serv		services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
		//inject costumed instance
		//  Scoped objects are the same within a request but different across requests. 
		services.AddScoped<IDemo, Demo>();
2. Api controller, 简单的DI及RESTful风格使用Api controller, 简单的DI及RESTful风格使用
```
    [Route("api/aggrid")]
    [ApiController]
    public class AgGridController : ControllerBase
    {

        private readonly IDemo _demo;        
        
		public AgGridController(IDemo demo)
        {
            _demo = demo;
        }

        [HttpGet]
        [Route("GetData")]
        public async Task<string> GetData()
        {
            return await Task.Run(() => _demo.GetDemoDataSource());
        }
        ...
     }
```
3. wwwroot: 前端文件
4. 下面是文件结构图
	![在这里插入图片描述](https://img-blog.csdnimg.cn/20190220193353745.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3NnczU5NTU5NQ==,size_16,color_FFFFFF,t_70)
###	AgGridApi.Common 公用方法项目
5. StaticConfigs: 读取dbsettings.json中的参数, 并作为静态类使用.
	推荐绑定类读取的方式.
```
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
```
### AgGridApi.Services 自定义服务项目
定义接口及其实现类
1. 在startup文件中完成注入`services.AddScoped<IDemo, Demo>();`.    
2. 在消费类的构造函数中添加接口类IDemo作为参数`public AgGridController(IDemo demo)  {  _demo = demo;  }`, 
3. .net core运行时作为一个容器, 在调用消费类时将自动注入Demo实例  
    1. IDemo.cs 接口类
    2. Demo.cs 结构实现类
### DataFactory 数据库项目
1. 作为类库项目, 需要在使用时配置数据库连接字符串
	```
	DataFactory.DataFactory.ConnectionString = StaticConfigs.GetDBConfig("OracleConnectionString");
	DataFactory.DataFactory.SqlCommandTimeout = int.Parse(StaticConfigs.GetDBConfig("SqlCommandTimeout"));
	_dataServiceSample = new DataServiceFactory();
	```
2. 使用 ADO.Net调用存储过程, 获取数据.
```
	 using (OracleConnection conn = new OracleConnection(DataFactory.ConnectionString))
	 {
	     using (OracleCommand cmd = new OracleCommand(procedureName))
	     {
	         AddParams(cmd, parameters);
	         cmd.Connection = conn;
	         cmd.CommandType = CommandType.StoredProcedure;
	         cmd.CommandTimeout = DataFactory.SqlCommandTimeout;
	
	         conn.OpenAsync();
	       
	         using (DbDataAdapter da = new OracleDataAdapter(cmd))
	         {
	             da.Fill(dt);
	         }
	     }
	 }
```
 
## Note
1. Not use Entity Framework, base on ADO.Net
因为工作中更多使用存储过程+Oracle, 不太适合使用Entity Framework, 作为试验写了一下.