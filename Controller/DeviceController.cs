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

        [HttpPost("report")]
        public async Task<IActionResult> ReceiveData([FromBody] DeviceReport data)
        {
            if (data?.Data?.TagList != null && data.Data.TagList.Count > 0)
            {
                List<string> epcList = data.Data.TagList.Select(tag => tag.Epc).ToList();

                foreach (var epc in epcList)
                {
                    OnEpcReceived?.Invoke(epc);
                }

                return Ok(new { message = "Received", epcList });
            }
            return BadRequest("Invalid data");
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
