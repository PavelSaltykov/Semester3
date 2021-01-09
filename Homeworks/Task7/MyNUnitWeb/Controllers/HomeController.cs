using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyNUnitWeb.Models;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Task7.Models;

namespace Task7.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment environment;
        private string PathToAssemblies => Path.Combine(environment.WebRootPath, "Assemblies");

        public HomeController(IWebHostEnvironment environment)
        {
            this.environment = environment;
            Directory.CreateDirectory(PathToAssemblies);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var ivm = new IndexViewModel(PathToAssemblies);
            return View(ivm);
        }

        [HttpPost]
        public IActionResult Clear()
        {
            var files = Directory.GetFiles(PathToAssemblies);
            foreach (var file in files)
            {
                System.IO.File.Delete(file);
            }
            var ivm = new IndexViewModel(PathToAssemblies);
            return RedirectToAction("Index", ivm);
        }

        [HttpPost]
        public async Task<IActionResult> UploadAssemblies(IFormFileCollection files)
        {
            foreach (var file in files)
            {
                var path = Path.Combine(PathToAssemblies, file.FileName);
                await using var fileStream = System.IO.File.Create(path);
                await file.CopyToAsync(fileStream);
            }
            var ivm = new IndexViewModel(PathToAssemblies);
            return RedirectToAction("Index", ivm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
