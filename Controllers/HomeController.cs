using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using GifResize.Hubs;
using System.Collections.Generic;

namespace GifResize.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext _hub;
        private AppConfiguration _configuration { get; set; }
        private IHostingEnvironment _environment;


        public HomeController(IConnectionManager connectionManager, IHostingEnvironment environment, IOptions<AppConfiguration> configuration)
        {
            _environment = environment;
            _configuration = configuration.Value;
            _hub = connectionManager.GetHubContext<TestHub>();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            var files = Request.Form.Files;
            var uploads = Path.Combine(_environment.ContentRootPath, _configuration.UploadFolderName);
            string uploadFileName = string.Empty;
            string tempFileName = string.Empty;

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);
            try
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        tempFileName = CommonHelper.GetRandomFileName(file.FileName); ;
                        uploadFileName = Path.Combine(uploads, tempFileName);
                        using (var fileStream = new FileStream(uploadFileName, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            return Json(new { Path = _configuration.ImagesBaseUrl + "/" + tempFileName, FileName = tempFileName, Width = files[0] });
        }



        public void ProcessGif(string fileName)
        {
            ProcessStartHelper process = new ProcessStartHelper();

            process.OutputDataReceived += (s, e) => _hub.Clients.All.message($"{e.Data}");
            process.ErrorDataReceived += (s, e) => _hub.Clients.All.error($"{e.Data}");
            process.Exited += (s, e) => _hub.Clients.All.exited($"Procees Finished...");
            process.Error += (s, e) => _hub.Clients.All.error(e.ErrorMessage);

            process.Resize(_configuration.EncoderPath, _configuration.UploadFolderName + "/" + fileName, "./image/output.gif");
        }
    }

}
