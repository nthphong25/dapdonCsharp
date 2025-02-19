using System.Windows;
using dapdon.Views; // Import thư mục Views

namespace dapdon
{
    public partial class App : Application
    {
        private readonly Startup _startup;

        public App()
        {
            _startup = new Startup();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _startup.Run();
        }
    }
}
