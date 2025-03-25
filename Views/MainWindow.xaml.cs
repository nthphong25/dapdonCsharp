using System.Collections.Generic;
using System.Linq;
using System.Windows;
using dapdon.ViewModels;
using dapdon.Controller;

namespace dapdon.Views
{
    public partial class MainWindow : Window
    {
        private MainContentViewModel _viewModel;
        private List<string> _epcList = new List<string>(); // Lưu danh sách EPC từ API

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainContentViewModel();
            DataContext = _viewModel;

            DeviceController.OnEpcReceived += epc =>
            {
                Dispatcher.Invoke(() =>
                {
                    _epcList.Add(epc); // Thêm EPC vào danh sách
                    _viewModel.LoadMoSummary(_epcList); // Load danh sách EPC
                });
            };
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _epcList.Clear();  // Xóa danh sách EPC khi refresh
         
            _viewModel.MoSummaryList.Clear();
        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
