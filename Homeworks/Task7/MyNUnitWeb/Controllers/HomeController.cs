﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyNUnit;
using MyNUnit.MethodInformation;
using MyNUnitWeb.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Task7.Models;

namespace Task7.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment environment;
        private string PathToAssemblies => Path.Combine(environment.WebRootPath, "Assemblies");
        private readonly IndexViewModel ivm;

        public HomeController(IWebHostEnvironment environment)
        {
            this.environment = environment;
            Directory.CreateDirectory(PathToAssemblies);
            ivm = new IndexViewModel(PathToAssemblies);
        }

        [HttpGet]
        public IActionResult Index() => View(ivm);

        public IActionResult Clear()
        {
            var files = new DirectoryInfo(PathToAssemblies).GetFiles();
            foreach (var file in files)
            {
                file.Delete();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UploadAssemblies(IFormFileCollection files)
        {
            if (files == null)
                return RedirectToAction("Index");

            foreach (var file in files)
            {
                var path = Path.Combine(PathToAssemblies, file.FileName);
                await using var fileStream = System.IO.File.Create(path);
                await file.CopyToAsync(fileStream);
            }
            return RedirectToAction("Index");
        }

        public IActionResult RunTests()
        {
            if (!ivm.LoadedAssemblies.Any())
                return RedirectToAction("Index");

            var runner = new TestRunner(PathToAssemblies);
            runner.Run();

            var testsInfo = runner.GetTestsInfo().OrderBy(info => info.AssemblyName)
                                             .ThenBy(info => info.ClassName)
                                             .ThenBy(info => info.MethodName);

            var groupsByAssembly = testsInfo.GroupBy(info => info.AssemblyName);
            var reports = new List<AssemblyReport>();
            foreach (var assembly in groupsByAssembly)
            {
                reports.Add(CreateAssemblyReport(assembly));
            }

            ivm.AssemblyReports = reports;
            return View("Index", ivm);
        }

        private AssemblyReport CreateAssemblyReport(IGrouping<string, Info> groupByAssembly)
        {
            var reports = new List<TestReport>();
            foreach (var info in groupByAssembly)
            {
                var report = new TestReport
                {
                    ClassName = info.ClassName,
                    Name = info.MethodName
                };
                if (info is TestResultInfo)
                {
                    var result = info as TestResultInfo;
                    report.Status = result.IsPassed ? TestStatus.Passed : TestStatus.Failed;
                    report.Time = result.Time;
                    report.Message = result.FailedMessage;
                }
                else
                {
                    var result = info as IgnoredTestInfo;
                    report.Status = TestStatus.Ignored;
                    report.Message = result.Message;
                }
                reports.Add(report);
            }

            var assemblyReport = new AssemblyReport
            {
                Name = groupByAssembly.Key,
                TestReports = reports,
                NumberOfPassed = reports.Where(r => r.Status == TestStatus.Passed).Count(),
                NumberOfFailed = reports.Where(r => r.Status == TestStatus.Failed).Count(),
                NumberOfIgnored = reports.Where(r => r.Status == TestStatus.Ignored).Count(),
            };
            return assemblyReport;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
