# AgGridAPI
This is a simple aggrid with  web api framework, aim to use aggrid with web api on .net core.     
Simply, aggrid send params to server, and transfer to procedure then get data from Database.     
- parameter(filterModels, sortModels..): Client(browser) => Server(.net core 2.2) => Oracle DB(procedure)    

## Function
1. Infinite Mode: Pagination, Filter, Sort
2. Server Mode: Pagination, Filter, Sort, Group by

## Application Architecture
1. Front End: Ag-grid + JavaScript
2. Server: Web API + ADO.Net(No EF)
3. DataBase: Oracle: procedure

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
	- Startup.cs: 配置和注入服务
```
	//add mvc serv		
	services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
	//inject costumed instance
	//  Scoped objects are the same within a request but different across requests. 
	services.AddScoped<IDemo, Demo>();
```	
2. Controller: DI+RESTful风格使用
```
    [Route("api/aggrid")]
    [ApiController]
    public class AgGridController : ControllerBase
    {
        private readonly IAGServer _aGServer;
        private readonly IRequestBuilder _requestBuilder;

        public AgGridController(IAGServer aGServer, IRequestBuilder requestBuilder)
        {
            _aGServer = aGServer;
            _requestBuilder = requestBuilder;
        }
	
        [HttpGet]
        [Route("GetDataColumns/{datasource}")]
        public Task<string> GetDataColumns(string datasource)
        {
            return Task.Run(() => _aGServer.GetDataColumns(datasource));
        }
        ...
     }
```
3. wwwroot: 前端文件 

### AgGridApi.Services 服务项目
定义接口及其实现类
1. 在startup文件中完成注入`services.AddScoped<IAGServer, AGServer>();`.    
2. 在消费类的构造函数中添加接口类IDemo作为参数`public AgGridController(IAGServer aGServer, IRequestBuilder requestBuilder)`, 
3. .net core运行时作为一个容器, 在调用服务时自动注入AGServer实例  
    1. IAGServer.cs 接口类
    2. AGServer.cs 结构实现类

### AgGridApi.Models 实体项目
1. 映射表格参数filterModels, sortModels...

### AgGridApi.Common 辅助方法项目
1. 生成表格列名
2. 辅助函数
3. 静态参数

### DataFactory 数据库项目
1. 使用ADO.Net调用存储过程, 获取数据. 
 
## Note
1. Not use Entity Framework, base on ADO.Net
因为工作中更多使用存储过程+Oracle, 不太适合使用Entity Framework, 作为试验写了一下.
