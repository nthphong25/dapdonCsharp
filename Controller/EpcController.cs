using dapdon.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace dapdon.Controllers
{
    [ApiController]
    [Route("api/epc")]
    public class EpcController : ControllerBase
    {
        private readonly MainContentViewModel _viewModel;

        public EpcController(MainContentViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        [HttpPost]
        public IActionResult ReceiveData([FromBody] ProbeData data)
        {
            if (data == null || data.Data == null || data.Data.TagList == null)
            {
                return BadRequest("Invalid data received");
            }

            // Lấy danh sách EPC từ request
            List<string> epcList = data.Data.TagList.Select(tag => tag.Epc).ToList();

            // Cập nhật ViewModel để hiển thị trong WPF
            //_viewModel.UpdateEpcList(epcList);

            return Ok(new { message = "Data received successfully", epcList });
        }
    }

    public class ProbeData
    {
        public string Timestamp { get; set; }
        public Data Data { get; set; }
    }

    public class Data
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
