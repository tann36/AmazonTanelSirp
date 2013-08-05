using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

using AmazonTanelSirp.Code.AmazonHelper;

namespace AmazonTanelSirp.Controllers
{
    public class HomeController : Controller
    {


		public ActionResult Index()
        {
			

            return View();
        }
		
		public ViewResult Search(string keywords, int page)
		{
			//AmazonHelper ahelper = new AmazonHelper();
            AmazonSearchResults results = AmazonHelper.Search(keywords, 13, page - 1);
			
			ViewBag.results = results.Items;
			ViewBag.totalItems = results.TotalItems;
			ViewBag.keywords = keywords;
			ViewBag.page = page;
			Debug.WriteLine("total items: " + results.TotalItems);

			return View("Index");
		}
    }
}
