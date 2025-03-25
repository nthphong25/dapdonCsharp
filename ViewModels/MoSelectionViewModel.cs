using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using dapdon.Views;

namespace dapdon.ViewModels
{
    public class MoSelectionViewModel : ViewBaseModel
    {
        private string _selectedMoNo;
        private string _originalMoNo;
        private string _searchText;

        public bool HasMoNoList { get; private set; } = false;
        public ObservableCollection<string> FilteredMoNoList { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> MoNoList { get; set; } = new ObservableCollection<string>();

        public string SelectedMoNo
        {
            get => _selectedMoNo;
            set
            {
                _selectedMoNo = value;
                OnPropertyChanged();
            }
        }
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterMoNoList(); // 🔎 Gọi hàm lọc dữ liệu khi thay đổi giá trị
            }
        }
        public void FilterMoNoList()
        {
            FilteredMoNoList.Clear();
            foreach (var mo in MoNoList.Where(m => m.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            {
                FilteredMoNoList.Add(mo);
            }
        }

        public ICommand ConfirmCommand { get; }

        private string _epcDetails;
        private readonly string _connectionString = "Server=10.30.0.18,1433;Database=DV_DATA_LAKE;User Id=sa;Password=greenland@VN;TrustServerCertificate=True";
        private readonly Action _onSuccess; // Callback để reload DataGrid

        public MoSelectionViewModel(string originalMoNo, string epcDetails, Action onSuccess)
        {
            _originalMoNo = originalMoNo;
            _epcDetails = epcDetails;
            _onSuccess = onSuccess; // Lưu callback để reload DataGrid

            LoadMoNoList(_originalMoNo);
            ConfirmCommand = new RelayCommand(ExecuteConfirm);
        }

        private void LoadMoNoList(string moNo)
        {
            Debug.WriteLine($"Original MO_NO: {moNo}");
            MoNoList.Clear();

            string query = @"
            SELECT DISTINCT dr2.mo_no
            FROM dv_rfidmatchmst dr2
            JOIN (
                SELECT DISTINCT shoestyle_codefactory, mat_code  
                FROM dv_rfidmatchmst 
                WHERE EPC_Code IN (SELECT value FROM STRING_SPLIT(@epcDetails, ','))
            ) dr1 ON dr2.shoestyle_codefactory = dr1.shoestyle_codefactory
                AND dr2.mat_code = dr1.mat_code
            WHERE dr2.mo_no IS NOT NULL AND dr2.mo_no <> @moNo;";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@epcDetails", _epcDetails);
                        cmd.Parameters.AddWithValue("@moNo", string.IsNullOrEmpty(moNo) ? (object)DBNull.Value : moNo);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MoNoList.Add(reader.GetString(0));
                            }
                        }
                    }
                }

                // Nếu có dữ liệu, đặt HasMoNoList = true
                HasMoNoList = MoNoList.Count > 0;

                if (!HasMoNoList)
                {
                    MessageBox.Show("Không tìm thấy MO_NO phù hợp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi truy vấn MO_NO: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void ExecuteConfirm(object obj)
        {
            if (string.IsNullOrEmpty(SelectedMoNo))
            {
                MessageBox.Show("Vui lòng chọn MO_NO.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            RunStoredProcedure(_epcDetails, SelectedMoNo);

            MessageBox.Show($"Chuyển đổi EPC thành công sang MO {SelectedMoNo}.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            _onSuccess?.Invoke(); // Gọi callback để reload DataGrid
        }

        private void RunStoredProcedure(string epcDetails, string newMoNo)
        {
            string query = "EXEC [dbo].[SP_transmono] @mode = 'A', @org_epc = @org_epc, @new_mono = @new_mono";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@org_epc", epcDetails);
                    cmd.Parameters.AddWithValue("@new_mono", newMoNo);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
