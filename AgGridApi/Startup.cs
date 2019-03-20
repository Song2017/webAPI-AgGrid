using System.Text.Encodings.Web;
using AgGridApi.Models.Request;
using AgGridApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AgGridApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("dbsettings.json", optional: true, reloadOnChange: true)
                //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton(Configuration); //IConfigurationRoot
            //Get config with class
            //services.Configure<DataFactorySetting>(Configuration.GetSection("DataFactorySetting"));
            //You can access that class from any method that the framework calls by adding it as a parameter in the constructor.The framework handles finding and providing the class to the constructor.Include Microsoft.Extension.Options in controller to work with IOption collection
            //services.AddOptions();

            //Instance injection
            //  Transient objects are always different
            //services.AddTransient<IOperationTransient, Operation>(); 
            //  Scoped objects are the same within a request but different across requests.
            //services.AddScoped<IOperationScoped, Operation>();
            //  Singleton objects are the same for every object and every request 
            //services.AddSingleton<IOperationSingleton, Operation>();
            services.AddScoped<IAGServer, AGServer>();
            services.AddTransient<IRequestBuilder, RequestBuilder>(); 
            //Cors policy is added to controllers via [EnableCors("CorsPolicy")]
            //or .UseCors("CorsPolicy") globally
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
            // Serilog config
            Log.Logger = new LoggerConfiguration()
                    .WriteTo.RollingFile(pathFormat: "logs\\log-{Date}.log")
                    .CreateLogger();

            if (env.IsDevelopment())
            {
                loggerFactory.AddDebug().AddConsole().AddSerilog();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                loggerFactory.AddSerilog();
                app.UseExceptionHandler(errorApp =>

                //Application level exception handler here - this is just a place holder
                errorApp.Run(async (context) =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync("<html><body>\r\n");
                    await context.Response.WriteAsync(
                            "We're sorry, we encountered an un-expected issue with your application.<br>\r\n");

                    //Capture the exception
                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        //This error would not normally be exposed to the client
                        await context.Response.WriteAsync("<br>Error: " +
                            HtmlEncoder.Default.Encode(error.Error.Message) + "<br>\r\n");
                    }
                    await context.Response.WriteAsync("<br><a href=\"/\">Home</a><br>\r\n");
                    await context.Response.WriteAsync("</body></html>\r\n");
                    await context.Response.WriteAsync(new string(' ', 512)); // Padding for IE
                }));
            }

            app.UseDatabaseErrorPage();
            app.UseStatusCodePages();
            //https://localhost:44364 to  index.html
            app.UseDefaultFiles();
            app.UseStaticFiles();

            //Apply CORS.
            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();
            //put last so header configs like CORS or Cookies etc can fire
            app.UseMvcWithDefaultRoute();
        }
    }

}
