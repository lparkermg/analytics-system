using System;
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
        public ActionResult Post(){
            _counter.Increment();
            return Ok();
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
