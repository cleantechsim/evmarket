using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CleanTechSim.MainPage.Models;
using CleanTechSim.MainPage.Helpers;

namespace mainpage.Controllers
{
    public class HomeController : Controller
    {
        public const string EV_ADOPTION_ID = "evAdoption";

        public IActionResult Index()
        {
            IndexModel model = new IndexModel(
                PreparedDataPoints.VerifyAndCompute(
                    EV_ADOPTION_ID,
                    StaticData.EvAdoption));

            return View(model);
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
