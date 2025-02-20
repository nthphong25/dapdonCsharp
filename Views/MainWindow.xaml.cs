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

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainContentViewModel();
            DataContext = _viewModel;

            DeviceController.OnEpcReceived += epc =>
            {
                Dispatcher.Invoke(() => _viewModel.LoadMoNoByEpc(epc));
            };
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearList();
        }

        private void OpenModal_Click(object sender, RoutedEventArgs e)
        {
            // Lấy danh sách MO Number duy nhất
            var moNumbers = _viewModel.EpcMoList.Select(item => item.MoNo).Distinct().ToList();

            // Mở modal
            MoSelectionWindow modal = new MoSelectionWindow(moNumbers);
            modal.Owner = this;

            if (modal.ShowDialog() == true && !string.IsNullOrEmpty(modal.SelectedMoNo))
            {
                MessageBox.Show("MO Number đã chọn: " + modal.SelectedMoNo, "Thông báo");
            }

        }
    }
}
