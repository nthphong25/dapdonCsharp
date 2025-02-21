using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using dapdon.ViewModels;
using Microsoft.Data.SqlClient;

namespace dapdon.Views
{
    public partial class MoSelectionWindow : Window
    {
        public List<string> MoNoList { get; set; }

        private string _selectedMoNo;
        public string SelectedMoNo
        {
            get { return _selectedMoNo; }
            set { _selectedMoNo = value; }
        }
        private MainContentViewModel _viewModel;


        public MoSelectionWindow(MainContentViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;  // Gán ViewModel làm DataContext

            if (_viewModel.MoNoList.Count > 0)
            {
                _viewModel.SelectedMoNo = _viewModel.MoNoList.First();  // Chọn giá trị đầu tiên mặc định
            }
        }




        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra xem MO Number đã chọn hợp lệ chưa
            if (string.IsNullOrEmpty(_viewModel.SelectedMoNo))
            {
                MessageBox.Show("Vui lòng chọn MO Number!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lấy danh sách EPC từ MainWindow
            if (Owner is MainWindow mainWindow)
            {
                List<string> epcList = mainWindow.GetEpclist();

                if (epcList == null || epcList.Count == 0)
                {
                    MessageBox.Show("Không có EPC nào để cập nhật!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Gọi stored procedure
                ExecuteStoredProcedure(epcList, _viewModel.SelectedMoNo);

                // Đóng modal
                DialogResult = true;
                Close();
            }
        }


        private void ExecuteStoredProcedure(List<string> epcList, string moNo)
        {
            string connectionString = "Server=10.30.0.18,1433;Database=DV_DATA_LAKE;User Id=sa;Password=greenland@VN;TrustServerCertificate=True;";
            string epcString = string.Join(",", epcList); // Ghép danh sách EPC thành chuỗi

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_transmono", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@mode", "A");
                        cmd.Parameters.AddWithValue("@org_epc", epcString);
                        cmd.Parameters.AddWithValue("@new_mono", moNo);
                        cmd.ExecuteNonQuery();
                    }
                }



                MessageBox.Show("Succes OK!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật MO Number: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
