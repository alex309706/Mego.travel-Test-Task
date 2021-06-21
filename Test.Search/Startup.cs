using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Test.Search.Interfaces;
using Test.Search.Models;

namespace Test.Search
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        //configuration of services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //Немного английского :)

            //Added Interface and Implementation to the DI. I understand that we are able to implement whatever we like. For example, DB on MSSQL Server,
            //and use EF Core for data access.
            //But i thought that it is quite enough to use Singletone object (one instatnce for the App)
            // So i transfer it to the input parameters of Controller constructor
            services.AddSingleton<IStorage<Metric>, MetricStorageList>();
            //добавление swagger
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mego Travel Test Task");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
