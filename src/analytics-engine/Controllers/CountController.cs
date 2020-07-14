using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using analytics_engine.Services;
using Microsoft.AspNetCore.Mvc;

namespace analytics_engine.Controllers
{
    [ApiController]
    [Route("count")]
    public class CountController : ControllerBase
    {
        private readonly ICountService _counter;
        public CountController(ICountService counter)
        {
            _counter = counter;
        }

        [HttpPost]
        public async Task<ActionResult> Post(){
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                _counter.Increment(await reader.ReadToEndAsync());
            }
            return await Task.FromResult(Ok());
        }

        [HttpGet("today")]
        public ActionResult Get()
        {
            return Ok(_counter.Get());
        }

        [HttpGet("all")]
        public JsonResult GetAll()
        {
            return new JsonResult(_counter.GetAll());
        }
    }
}
