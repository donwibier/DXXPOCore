using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using web.Utils;

namespace web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddXpoPooledDataLayer(MSSqlConnectionProvider.GetConnectionString("192.168.5.7", "test-user", "test123", "WebAppDemo"))                
                .AddXpoUnitOfWork()
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    // use the custom resolver (above)
                    options.SerializerSettings.ContractResolver = new XpoContractResolver();
                    // don't kill yourself over loop properties (probably not really needed now, I 
                    // inserted this originally to work around the This property on the XPO types)
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });        
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseXpoDemoData();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
