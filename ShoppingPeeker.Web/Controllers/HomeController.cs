using System;
using System.Web;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingPeeker.Web.Mvc;

using ShoppingPeeker.Web.ViewModels;


namespace ShoppingPeeker.Web.Controllers
{
    public class HomeController : BaseMvcController
    {
        public IActionResult Index()
        {
			//var r = this.HttpContext.Request;
			//var id = Request.GetQuery<string>("id");
			//var ip = Request.GetIP();
			
			var dataContainer = new BusinessViewModelContainer<HomePageViewModel>();
            var viewModel = new HomePageViewModel();
            dataContainer.Data = viewModel;

            return View(dataContainer);
        }
        public IActionResult Help()
        {
            ViewData["Message"] = "this is help page";

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "this is about page";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
