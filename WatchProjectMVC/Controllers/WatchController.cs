using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web;
using WatchProjectMVC.Models;

namespace WatchProjectMVC.Controllers
{
    public class WatchController : Controller
    {

#if DEBUG
        readonly Uri basedAddress = new("https://localhost:44370/api");
#else
        readonly Uri basedAddress = new("https://watchprojectapi2023.azurewebsites.net/api");
#endif

        private readonly HttpClient _client;

        public WatchController(IWebHostEnvironment webHostEnvironment)
        {
            _client = new HttpClient();
            _client.BaseAddress = basedAddress;
          
        }




        [HttpGet]
        public async Task<IActionResult> Index(string? watchName)
        {
            List<WatchViewModel> watchlist = new List<WatchViewModel>();
            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "/Watch/GetAll?watchname=" + watchName).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                watchlist = JsonConvert.DeserializeObject<List<WatchViewModel>>(data)!;
            }
            return View(watchlist);
        }
      

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                WatchViewModel watch = new WatchViewModel();
                HttpResponseMessage response =  await _client.GetAsync(_client.BaseAddress + "/Watch/GetbyId/" + id).ConfigureAwait(false); 

                if (response.IsSuccessStatusCode)
                {

                    string data = response.Content.ReadAsStringAsync().Result;
                    watch = JsonConvert.DeserializeObject<WatchViewModel>(data)!;

                }
                return View(watch);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return await Task.Run(() => View());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(IFormFile watchImage, WatchViewModel model)
        {
            try
            {
                if (watchImage == null || watchImage.Length == 0)
                {
                    TempData["errorMessageforImage"] = "Select Image for upload";
                    return View();
                }

                string extesion = System.IO.Path.GetExtension(watchImage.FileName);
                if(extesion.ToUpper() == ".JPG" || extesion.ToUpper() == ".PNG")
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await watchImage.CopyToAsync(ms);
                        model.ImageUrl = watchImage.FileName;
                        model.WatchImage = ms.ToArray();
                    }

                    string data = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Watch/Create/", content).Result;


                    if (response.IsSuccessStatusCode)
                    {
                        TempData["successMessage"] = "Watch Added.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["errorMessageforImage"] = "Image file should be PNG or JGP only";
                    return View();
                }
              


               
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            try
            {
                WatchViewModel watch = new WatchViewModel();
                HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "/Watch/GetbyId/" + id).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    string data =await response.Content.ReadAsStringAsync().ConfigureAwait(false); 
                    watch = JsonConvert.DeserializeObject<WatchViewModel>(data)!;

                }
                return View(watch);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }
        
        //not working in [HttpPut]
        [HttpPost]
        public async Task<IActionResult> Update(IFormFile watchImage, WatchViewModel model)
        {
            try
            {
                if (watchImage != null && watchImage.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await watchImage.CopyToAsync(ms);
                        model.ImageUrl = watchImage.FileName;
                        model.WatchImage = ms.ToArray();
                    }
                }
               

                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PutAsync(_client.BaseAddress + "/Watch/Update/" + model.Id, content).ConfigureAwait(false);


                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Watch details Updated.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();

        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                WatchViewModel model = new WatchViewModel();
                HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "/Watch/GetbyId/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    model = JsonConvert.DeserializeObject<WatchViewModel>(data)!;

                }
                 return View(model);
               
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        //[HttpDelete]
        public async Task<IActionResult> Delete(WatchViewModel model)
        {
            try
            {

                HttpResponseMessage response = await _client.DeleteAsync(_client.BaseAddress + "/Watch/Delete/" + model.Id).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Deleted successfully.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ViewWatchList()
        {
            List<WatchViewModel> watchlist = new List<WatchViewModel>();
            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "/Watch/GetRandom8").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                watchlist = JsonConvert.DeserializeObject<List<WatchViewModel>>(data)!;
            }
            return View(watchlist);
        }
    }
}
