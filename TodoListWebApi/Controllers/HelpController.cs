using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListApi.Controllers
{
    [ApiController]
    
    public class HelpController : ControllerBase
    {
        [Route("api/[Controller]")]
        public IActionResult Get()
        {
            string helphelp =
                "Help about Authenticate Pass\r\n" +
                "Get Token => api/Authenticate?User=[]&Pwd=[] , then you Get Token & RefreshToken \r\n" +
                "Refresh   => api/Authenticate/Refresh?User=[]&Refresh=[] \r\n" +
                "Now must be add Token Headers ,any request\r\n\r\n" +
                "Help about used Todolist \r\n" +
                "url = ( api/Todo/[User] ) => Example ( api/Todo/Test1 )\r\n" +
                "Get    ( )                              ,GetTodolist \r\n" +
                "Post   (need Body=> Title & description), =>Create Success OR Create failed \r\n" +
                "Put    (need Body=> Title & description), =>Update Success OR Update failed \r\n" +
                "Delete (need Body=> Title)              , =>Delete Success OR Delete failed \r\n\r\n" +
                "<Wearning>\r\n" +
                "               Any Title cannot be repeated in ToDoList.\r\n" +
                "               Must be add Token Headers ,any Request.\r\n" +
                "<Wearning>";
            return Ok(helphelp);
        }
    }
}
