using dapdon.ViewModels;
using System.Windows;

namespace dapdon.Views  // Đúng với vị trí thư mục
{
    public partial class MainWindow : Window
    {     
        public MainWindow(MainContentViewModel mainContentViewModel)
        {
            InitializeComponent();
            DataContext = mainContentViewModel;
        }
    }
}
