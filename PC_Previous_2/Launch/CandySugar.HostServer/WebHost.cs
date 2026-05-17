using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

namespace CandySugar.HostServer
{
    public class WebHost
    {
        public static void StartWeb()
        {
            Host.CreateDefaultBuilder().ConfigureWebHostDefaults(t =>
            {
                t.UseKestrel();
                t.ConfigureKestrel(x => x.ListenAnyIP(99));
                t.ConfigureServices(services =>
                {
                    services.AddControllers().AddNewtonsoftJson(opt =>
                    {
                        opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                        opt.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    });
                    services.AddSwaggerGen(opt =>
                    {
                        opt.SwaggerDoc("CandySugar", new OpenApiInfo { Title = "甜糖", Version = "CandySugar" });
                    });
                });
                t.Configure(app =>
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(opt =>
                    {
                        opt.SwaggerEndpoint("/swagger/CandySugar/swagger.json", "甜糖");
                    });
                    app.UseRouting();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
            }).UseWindowsService().Build().Start();
        }
    }
}
