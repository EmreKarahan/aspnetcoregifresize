using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using GifResize.Hubs;
using System.Diagnostics;
using StackExchange.Redis;

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

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);
            try
            {

                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
                redis.PreserveAsyncOrder = false;
                ISubscriber sub = redis.GetSubscriber();

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        uploadFileName = Path.Combine(uploads, file.FileName);
                        using (var fileStream = new FileStream(uploadFileName, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        ProcessStartHelper.OutputDataReceived += OutputHandler;//(s, e) => _hub.Clients.All.hello(e.Data);
                        ProcessStartHelper.Exited += Exited;
                        ProcessStartHelper.Resize(_configuration.EncoderPath, uploadFileName, "./image/output.gif", 650, 330);
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }

            return Json(new { Path = uploadFileName, Width = files[0] });
        }


        private void OutputHandler(object sender, DataReceivedEventArgs e)
        {
            _hub.Clients.All.hello(e.Data);

        }

        private void Exited(object sender, System.EventArgs e)
        {
            _hub.Clients.All.exited("Procees Finished...");

        }







        // public static MagickImageCollection StretchGifImageSize(Image imgSource, int? nWidthLimit, int? nHeightLimit)
        // {
        //     // Limit yoksa veya resim boyutları limitlere eşitse kopyası gider
        //     if ((!nWidthLimit.HasValue || nWidthLimit.Value == imgSource.Width) && (!nHeightLimit.HasValue || nHeightLimit.Value == imgSource.Height))
        //     {


        //         var convertedImage = ConvertBitmapToJpeg(imgSource);
        //         //return new MagickImageCollection(convertedImag;


        //     }
        //     int nChoosenWidth = imgSource.Width;
        //     if (nWidthLimit.HasValue) nChoosenWidth = nWidthLimit.Value;

        //     int nChoosenHeight = imgSource.Height;
        //     if (nHeightLimit.HasValue) nChoosenHeight = nHeightLimit.Value;

        //     string input = string.Format("{0}/{1}-{2}.gif", ConfigurationCus.FffmpegTempPath, "InputGifFile", Guid.NewGuid().ToString().Replace("-", string.Empty));
        //     string output = string.Format("{0}/{1}-{2}x{3}-{4}.gif", ConfigurationCus.FffmpegTempPath, "OutputGifFile", nChoosenWidth, nChoosenHeight, DateTime.Now.ToString("hh-mm-ss"));

        //     return InvokeFfmpeg(input, nChoosenWidth, nChoosenHeight, output);

        ////Image bmp = Image.FromFile(HttpContext.Current.Server.MapPath(returnOutput));
        ////return (Image) bmp.Clone();
        //MemoryStream memoryStream = new MemoryStream();
        //using (FileStream file = new FileStream(HttpContext.Current.Server.MapPath(returnOutput), FileMode.Open, FileAccess.Read))
        //{
        //    memoryStream.WriteTo(file);
        //}
        //return memoryStream;
    }

    // public static MagickImageCollection LimitGifImageSize(Image imgSource, int? nWidthLimit, int? nHeightLimit)
    // {
    //     // Limit yoksa veya resim limitlerin altında veya eşitse kopyası gider
    //     if ((!nWidthLimit.HasValue || nWidthLimit.Value >= imgSource.Width) && (!nHeightLimit.HasValue || nHeightLimit.Value >= imgSource.Height))
    //     {

    //         var convertedImage = ConvertBitmapToJpeg(imgSource);
    //         //return (Bitmap)convertedImage;
    //     }

    //     int nScaledWidthDueToHeight = imgSource.Width;
    //     int nScaledHeightDueToWidth = imgSource.Height;
    //     int nChoosenWidth = imgSource.Width;
    //     int nChoosenHeight = imgSource.Height;

    //     // Genişlik&Yükseklik limitine göre, ölçülendirilmiş yükseklik hesabı
    //     if (nWidthLimit.HasValue && nHeightLimit.HasValue)
    //     {
    //         nScaledHeightDueToWidth = GetHeightWithWidthScaled(imgSource.Size, nWidthLimit.Value);
    //         if (nScaledHeightDueToWidth <= nHeightLimit.Value)
    //         {
    //             nChoosenWidth = nWidthLimit.Value;
    //             nChoosenHeight = nScaledHeightDueToWidth;
    //         }
    //         else
    //         {
    //             nScaledWidthDueToHeight = GetWidthWithHeightScaled(imgSource.Size, nHeightLimit.Value);
    //             if (nScaledWidthDueToHeight <= nWidthLimit.Value)
    //             {
    //                 nChoosenWidth = nScaledWidthDueToHeight;
    //                 nChoosenHeight = nHeightLimit.Value;
    //             }
    //         }
    //     }

    //     // Genişlik limitine göre, ölçülendirilmiş genişlik hesabı
    //     if (nWidthLimit.HasValue && !nHeightLimit.HasValue)
    //     {
    //         nChoosenWidth = nWidthLimit.Value;
    //         nChoosenHeight = GetHeightWithWidthScaled(imgSource.Size, nWidthLimit.Value);
    //     }

    //     // Yükseklik limitine göre, ölçülendirilmiş genişlik hesabı
    //     if (!nWidthLimit.HasValue && nHeightLimit.HasValue)
    //     {
    //         nChoosenWidth = GetWidthWithHeightScaled(imgSource.Size, nHeightLimit.Value);
    //         nChoosenHeight = nHeightLimit.Value;
    //     }


    //     //float aspectRatioOriginal = (float) imgSource.Width / imgSource.Height;
    //     //float aspectRatioLimits = (float)imgSource.Width / imgSource.Height;


    //     string input = string.Format("{0}/{1}-{2}.gif", ConfigurationCus.FffmpegTempPath, "InputGifFile", Guid.NewGuid().ToString().Replace("-", string.Empty));
    //     string output = string.Format("{0}/{1}-{2}x{3}-{4}.gif", ConfigurationCus.FffmpegTempPath, "OutputGifFile", nChoosenWidth, nChoosenHeight, DateTime.Now.ToString("hh-mm-ss"));

    //     return InvokeFfmpeg(input, nChoosenWidth, nChoosenHeight, output);

    //     //MemoryStream memoryStream = new MemoryStream();
    //     //using (FileStream file = new FileStream(HttpContext.Current.Server.MapPath(returnOutput), FileMode.Open, FileAccess.Read))
    //     //{
    //     //    memoryStream.WriteTo(file);
    //     //}
    //     //return memoryStream;

    //     //Image bmp = Image.FromFile(HttpContext.Current.Server.MapPath(returnOutput));
    //     //return (Image)bmp.Clone();
    // }

    // public static MagickImageCollection GifImageResizeWithColor(Image imgPhoto, int Height, int Width, int Quality, Color color)
    // {
    //     int sourceWidth = imgPhoto.Width;
    //     int sourceHeight = imgPhoto.Height;
    //     int sourceX = 0;
    //     int sourceY = 0;
    //     int destX = 0;
    //     int destY = 0;

    //     float nPercent = 0;
    //     float nPercentW = 0;
    //     float nPercentH = 0;

    //     //#CE#09/11/2009: Imaj aynı boyuttaysa sadece jpeg'e çevir
    //     //if (Height == sourceHeight && Width == sourceWidth)
    //     //{
    //     //    return ConvertBitmapToJpeg(imgPhoto);
    //     //}

    //     nPercentW = ((float)Width / (float)sourceWidth);
    //     nPercentH = ((float)Height / (float)sourceHeight);
    //     if (nPercentH < nPercentW)
    //     {
    //         nPercent = nPercentH;
    //         destX = System.Convert.ToInt16((Width - (sourceWidth * nPercent)) / 2);
    //     }
    //     else
    //     {
    //         nPercent = nPercentW;
    //         destY = System.Convert.ToInt16((Height - (sourceHeight * nPercent)) / 2);
    //     }

    //     int destWidth = (int)(sourceWidth * nPercent);
    //     int destHeight = (int)(sourceHeight * nPercent);

    //     string input = string.Format("{0}/{1}-{2}.gif", ConfigurationCus.FffmpegTempPath, "InputGifFile", Guid.NewGuid().ToString().Replace("-", string.Empty));
    //     imgPhoto.Save(HttpContext.Current.Server.MapPath(input));
    //     string output = string.Format("{0}/{1}-{2}x{3}-{4}.gif", ConfigurationCus.FffmpegTempPath, "OutputGifFile", destWidth, destHeight, DateTime.Now.ToString("hh-mm-ss"));

    //     return InvokeFfmpeg(input, destWidth, destHeight, output);

    //     //MemoryStream memoryStream = new MemoryStream();
    //     //using (FileStream file = new FileStream(HttpContext.Current.Server.MapPath(returnOutput), FileMode.Open, FileAccess.Read))
    //     //{
    //     //    memoryStream.WriteTo(file);
    //     //}

    //     //Image bmp = Image.FromFile(HttpContext.Current.Server.MapPath(returnOutput));
    //     //return (Image)bmp.Clone();
    //     //return memoryStream;

    // }

    // private static MagickImageCollection InvokeFfmpeg(string input, int width, int height, string output)
    // {
    //     string ffmpegFileName = ConfigurationCus.FffmpegPath;
    //     string ffmpegArguments = string.Format("-i \"{0}\" -vf scale={1}:{2} -y \"{3}\"", HttpContext.Current.Server.MapPath(input), width, height, HttpContext.Current.Server.MapPath(output));
    //     try
    //     {

    //         //StreamReader streamReader = null;
    //         SecureString secureString = new SecureString();
    //         "sp5498s6f465s".ToCharArray().ToList().ForEach(secureString.AppendChar);
    //         ProcessStartInfo psi = new ProcessStartInfo()
    //         {
    //             CreateNoWindow = false,
    //             WindowStyle = ProcessWindowStyle.Hidden,
    //             UseShellExecute = false,
    //             RedirectStandardOutput = true,
    //             RedirectStandardInput = true,
    //             RedirectStandardError = true,
    //             //Domain = "DGNET",
    //             //UserName = "admindg",
    //             //Password = secureString,
    //             FileName = ffmpegFileName,
    //             Arguments = ffmpegArguments
    //         };

    //         Process ffProcess = new Process {StartInfo = psi};
    //         EventLog.WriteEntry("FFMpeg Commandline", ffmpegFileName + " " + ffmpegArguments);
    //         ffProcess.Start();
    //         //ffProcess.BeginErrorReadLine();
    //         //ffProcess.BeginOutputReadLine();



    //         ////streamReader = ffProcess.StandardOutput;                
    //         ////string streamMessage = streamReader.ReadToEnd();


    //         ////EventLog.WriteEntry("FFMpeg StreamReader Message", streamMessage);

    //         //string error = "";
    //         //string outs = "";

    //         //while (!ffProcess.HasExited)
    //         //{
    //         //    while (!ffProcess.StandardError.EndOfStream)
    //         //    {
    //         //        error += ffProcess.StandardError.ReadLine();
    //         //    }

    //         //    while (!ffProcess.StandardOutput.EndOfStream)
    //         //    {
    //         //        outs += ffProcess.StandardOutput.ReadLine();
    //         //    }
    //         //}

    //         //EventLog.WriteEntry("FFMpeg Error", error);
    //         //EventLog.WriteEntry("FFMpeg Output", outs);

    //         ffProcess.WaitForExit(50000);
    //     }
    //     catch (Exception w32E)
    //     {

    //         EventLog.WriteEntry("FFMpeg Commandline", w32E.Message);
    //     }


    //     MagickImageCollection imageCollection = new MagickImageCollection(HttpContext.Current.Server.MapPath(output));
    //     return imageCollection;

    // }

}
