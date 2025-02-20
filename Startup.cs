using dapdon.Configs;
using dapdon.ViewModels;
using dapdon.Views;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dapdon
{
    public class Startup
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        private IHost _webApiHost;

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Khởi chạy API
            StartWebApi();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainContentViewModel>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddControllers().AddNewtonsoftJson(); // ✅ Đảm bảo Newtonsoft.Json hoạt động
        }

        private void StartWebApi()
        {
            Task.Run(async () =>
            {
                _webApiHost = Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseKestrel()
                                  .UseUrls("http://localhost:5000") // Lắng nghe API tại cổng 5000
                                  .UseStartup<WebApiStartup>();
                    })
                    .Build();

                await _webApiHost.RunAsync(); // ✅ Đảm bảo API chạy đúng cách
            });
        }

        public void StopWebApi()
        {
            _webApiHost?.Dispose();
        }

        public void Run()
        {
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Closed += (s, e) => StopWebApi(); // ✅ Dừng API khi app đóng
            mainWindow.Show();
        }
    }

    public class WebApiStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Cho phép API hoạt động
            });
        }
    }
}
