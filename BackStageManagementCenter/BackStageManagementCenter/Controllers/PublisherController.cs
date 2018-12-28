using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BackStageManagementCenter.Controllers
{
    [Route("[controller]")]
    public class PublisherController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public  IActionResult SendMessage([FromForm] FormMessage formMessage)
        {

            return View("index");
        }
    }

    public class FormMessage
    {
        [Required]
        public string Message { get; set; }

    }
}