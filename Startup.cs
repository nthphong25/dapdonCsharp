using dapdon.Configs;
using dapdon.ViewModels;
using dapdon.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace dapdon
{
    public class Startup
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());
                //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Đăng ký AppSettings và bind với cấu hình từ appsetting.json
            //var appSettings = Configuration.GetSection("AppSettings").Get<AppSetting>();
            //services.AddSingleton<AppSetting>(appSettings);
            // Đăng ký TimerService
            // Đăng ký các dịch vụ cần thiết
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainContentViewModel>();
            // Đăng ký IConfiguration để có thể sử dụng trong các dịch vụ
            services.AddSingleton<IConfiguration>(Configuration);
        }

        public void Run()
        {
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
