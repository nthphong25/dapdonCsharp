using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dapdon.Controller
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("API is working!");
        }
    }
}
