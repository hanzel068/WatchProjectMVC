using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WatchProjectMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;


namespace WatchProjectMVC.Controllers
{
    public class HomeController : Controller
    {
#if DEBUG
        readonly Uri basedAddress = new("https://localhost:44370/api");
#else
        readonly Uri basedAddress = new("https://watchprojectapi2023.azurewebsites.net/api");
#endif
        private readonly HttpClient _client;

        public HomeController()
        {
            _client = new HttpClient();
            _client.BaseAddress = basedAddress;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<WatchViewModel> watchlist = new List<WatchViewModel>();
            HttpResponseMessage response =await _client.GetAsync(_client.BaseAddress + "/Watch/GetRandom").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                watchlist = JsonConvert.DeserializeObject<List<WatchViewModel>>(data)!;
            }
            return View(watchlist);
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