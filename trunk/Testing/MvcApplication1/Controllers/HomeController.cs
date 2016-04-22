using System;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index() {
			throw new Exception("Test Exception Module");
		}
	}
}