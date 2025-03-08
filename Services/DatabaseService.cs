using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using dapdon.Models;
using System.Data;

namespace dapdon.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString = "Server=10.30.0.18,1433;Database=DV_DATA_LAKE;User Id=sa;Password=greenland@VN;TrustServerCertificate=True";

        public List<EpcMoModel> GetMoNoByEpc(string epc)
        {
            List<EpcMoModel> results = new();
            using SqlConnection conn = new(_connectionString);
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
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@epc", epc);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new EpcMoModel
                {
                    EPC = epc,
                    MoNo = reader["mo_no"]?.ToString() ?? "",
                    ShoeStyle = reader["shoestyle_codefactory"]?.ToString() ?? "",
                    MatColor = reader["mat_color_assemble"]?.ToString() ?? "",
                    MatCode = reader["mat_code"]?.ToString() ?? ""
                });
            }
            return results;
        }

        public List<string> GetAllMoNumbers()
        {
            List<string> moNumbers = new List<string>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT DISTINCT mo_no FROM dv_rfidmatchmst";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        moNumbers.Add(reader.GetString(0));
                    }
                }
            }
            return moNumbers;
        }

        public bool UpdateMoNumber(List<string> epcList, string moNo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_transmono", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@mode", "A");
                        cmd.Parameters.AddWithValue("@org_epc", string.Join(",", epcList));
                        cmd.Parameters.AddWithValue("@new_mono", moNo);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}