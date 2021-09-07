using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListApi.Controllers
{
    [Route("api/[Controller]")]
    public class RecycleController : ControllerBase
    {
        public IActionResult Get()
        {
            return Ok("Under construction, Not available.");
        }
    }
}
