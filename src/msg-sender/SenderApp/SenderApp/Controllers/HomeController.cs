using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SenderApp.Models;
using SenderApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SenderApp.Controllers
{
    //Comment4
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        private static List<Task> _continuousMessageSender = new List<Task>();
        private CancellationTokenSource _cancelationTokenSource;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewData["MessagesCounter"] = ServiceBusService.MessagesSent;
            ViewData["RunningSenders"] = _continuousMessageSender.Count;
            return View();
        }

        public async Task<IActionResult> SendServiceBusMessages(int count)
        {
            if (count == 0 && _cancelationTokenSource != null)
            {
                _cancelationTokenSource.Cancel();
                _cancelationTokenSource = null;
                await Task.WhenAll(_continuousMessageSender);

                _continuousMessageSender = new List<Task>();
                return RedirectToAction(nameof(Index), "Home");
            }

            if (_cancelationTokenSource == null)
            {
                _cancelationTokenSource = new CancellationTokenSource();
            }
            CancellationToken token = _cancelationTokenSource.Token;
            var service = new ServiceBusService(_logger, _configuration);
            _continuousMessageSender.Add(service.SendMessageAsync(token, count));

            return RedirectToAction(nameof(Index), "Home");
        }

        public async Task<IActionResult> Privacy()
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
