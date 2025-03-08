using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;

namespace dapdon.ViewModels
{
    public class EpcMoModel
    {
        public string EPC { get; set; }
        public string MoNo { get; set; }
        public string ShoeStyle { get; set; }
        public string MatColor { get; set; }
        public string MatCode { get; set; }
    }

    public class MainContentViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient = new HttpClient();

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
                    string query = @"
                        SELECT DISTINCT 
                            a.mat_code,
                            a.mo_no, 
                            a.shoestyle_codefactory, 
                            (b.mat_color + ' / ' + b.mat_ecolor) AS mat_color_assemble
                        FROM dv_rfidmatchmst a
                        LEFT JOIN wuerp_vnrd.dbo.ta_productmst b 
                            ON a.mat_code = b.mat_code AND b.isactive = 'Y'
                        LEFT JOIN wuerp_vnrd.dbo.ta_manufacturmst c 
                            ON b.mat_code = c.mat_code AND c.isactive = 'Y'
                        WHERE a.EPC_Code = @epc";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@epc", epc);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            bool hasData = false;

                            while (reader.Read())
                            {
                                string moNo = reader["mo_no"] as string ?? "";
                                string shoeStyle = reader["shoestyle_codefactory"] as string ?? "";
                                string matColor = reader["mat_color_assemble"] as string ?? "";
                                string matCode = reader["mat_code"] as string ?? "";
                                EpcMoList.Add(new EpcMoModel
                                {
                                    EPC = epc,
                                    MoNo = moNo,
                                    ShoeStyle = shoeStyle,
                                    MatColor = matColor,
                                    MatCode = matCode,
                                });

                                hasData = true;
                            }

                            // Nếu không có dữ liệu từ DB, vẫn hiển thị EPC và để trống các cột khác
                            if (!hasData)
                            {
                                EpcMoList.Add(new EpcMoModel
                                {
                                    EPC = epc,
                                    MoNo = "",       // Để trống thay vì null
                                    ShoeStyle = "",
                                    MatColor = "",
                                    MatCode = "",
                                });
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

  

        public async Task ClearList()
        {
            var response = await _httpClient.PostAsync("http://localhost:5000/api/device/clear", null);
            if (response.IsSuccessStatusCode)
            {
                EpcMoList.Clear();  // Xóa danh sách EPC trên UI
            }
        }

        public void ReloadData()
        {
            EpcMoList.Clear();
            LoadMoNoByEpc("");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
