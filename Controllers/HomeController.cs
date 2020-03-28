using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FirstTryMVC5.Models;

namespace FirstTryMVC5.Controllers {
  public class HomeController : Controller {
    TestContext testContext = new TestContext();
    public ActionResult Index() {
      //извлекаем данные из таблицы TestList
      IEnumerable<QuestionAnswer> dbLine = testContext.TestsList;
      //ViewBag - динамический объект
      ViewBag.QuestionAnswer = dbLine;
      return View();
    }


    public ActionResult About() {
      ViewBag.Message = "Your application description page.";

      return View();
    }

    public ActionResult Contact() {
      ViewBag.Message = "Your contact page.";

      return View();
    }
  }
}