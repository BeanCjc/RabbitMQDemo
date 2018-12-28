using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BackStageManagementCenter.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System;

namespace BackStageManagementCenter.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("indexpage");
        }

        [HttpPost("[action]")]
        public IActionResult SendMessage([FromForm] FormMessage formMessage)
        {
            if (ModelState.IsValid)
            {


                var factory = new ConnectionFactory() { HostName = "localhost", UserName = "xuanyakeji", Password = "123456" };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare("ExchangeMiniWPOS", ExchangeType.Fanout, true, false, null);
                        channel.QueueDeclare("QueueMiniWPOS", true, false, false, null);
                        channel.QueueBind("QueueMiniWPOS", "ExchangeMiniWPOS", "", null);
                        var propertites = channel.CreateBasicProperties();
                        propertites.Persistent = true;
                        propertites.DeliveryMode = 2;
                        var message = $"WebServer   {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n {formMessage.Message}\r\n\r\n";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish("ExchangeMiniWPOS", "", propertites, body);
                        ViewData["message"] = message;
                        return View("indexpage");
                    }
                }
            }
            return BadRequest();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
