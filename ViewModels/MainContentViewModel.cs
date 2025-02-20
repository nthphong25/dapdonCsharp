using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Linq;

namespace dapdon.ViewModels
{
    public class EpcMoModel
    {
        public string EPC { get; set; }
        public string MoNo { get; set; }
    }

    public class MainContentViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<EpcMoModel> _epcMoList = new ObservableCollection<EpcMoModel>();
        public ObservableCollection<EpcMoModel> EpcMoList
        {
            get { return _epcMoList; }
            set
            {
                _epcMoList = value;
                OnPropertyChanged();
            }
        }

        private string _connectionString = "Server=10.30.0.18,1433;Database=DV_DATA_LAKE;User Id=sa;Password=greenland@VN;TrustServerCertificate=True";

        public void LoadMoNoByEpc(string epc)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT mo_no FROM dv_rfidmatchmst WHERE EPC_Code = @epc";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@epc", epc);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string moNo = reader["mo_no"].ToString();
                                EpcMoList.Add(new EpcMoModel { EPC = epc, MoNo = moNo });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi truy vấn database: " + ex.Message);
                }
            }
        }

        public void ClearList()
        {
            EpcMoList.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
