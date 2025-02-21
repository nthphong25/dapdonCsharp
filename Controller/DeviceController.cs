using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dapdon.Controller
{
    [ApiController]
    [Route("api/device")]
    public class DeviceController : ControllerBase
    {
        public static event Action<string> OnEpcReceived;
        private static HashSet<string> UniqueEpcList = new HashSet<string>(); // Lưu EPC không trùng

        [HttpPost("report")]
        public async Task<IActionResult> ReceiveData([FromBody] DeviceReport data)
        {
            if (data?.Data?.TagList != null && data.Data.TagList.Count > 0)
            {
                List<string> newEpcList = new List<string>();

                foreach (var tag in data.Data.TagList)
                {
                    if (UniqueEpcList.Add(tag.Epc)) // Nếu EPC chưa có thì thêm vào
                    {
                        newEpcList.Add(tag.Epc);
                        OnEpcReceived?.Invoke(tag.Epc);
                    }
                }

                if (newEpcList.Count == 0)
                {
                    return Ok(new { message = "Không có EPC mới", count = 0 });
                }

                return Ok(new { message = "Received unique EPCs", epcList = newEpcList });
            }
            return BadRequest("Invalid data");
        }

        // API để lấy danh sách EPC đã nhận
        [HttpGet("get-epcs")]
        public IActionResult GetEpcList()
        {
            return Ok(UniqueEpcList.ToList()); // Chuyển HashSet thành List và trả về
        }
        // API để xóa danh sách EPC đã nhận
        [HttpPost("clear")]
        public IActionResult ClearEpcList()
        {
            UniqueEpcList.Clear();
            return Ok(new { message = "Danh sách EPC đã được xóa." });
        }

    }

    public class DeviceReport
    {
        public string Timestamp { get; set; }
        public DeviceData Data { get; set; }
        public string Method { get; set; }
        public string Sn { get; set; }
    }

    public class DeviceData
    {
        public string Timestamp { get; set; }
        public string Id { get; set; }
        public string Temperature { get; set; }
        public List<Tag> TagList { get; set; }
    }

    public class Tag
    {
        public string Direction { get; set; }
        public long FirstTime { get; set; }
        public int Ant { get; set; }
        public int FirstAnt { get; set; }
        public string Rssi { get; set; }
        public long LastTime { get; set; }
        public string Epc { get; set; }
    }
}
