using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using FirstTryMVC5.Models;

namespace FirstTryMVC5.Controllers {
  public class HomeController : Controller {
    TestContext testContext = new TestContext();
    string rightCurrentAnswer;
    IEnumerable<QuestionAnswer> dbLines;

    [HttpGet]
    public ActionResult Index() {
      ViewBag.Subject = "Выбрать тему";
      ViewBag.DisabledCheck = "disabled";
      ViewBag.DisabledNext = "disabled";
      //извлекаем данные из таблицы TestList
      dbLines = testContext.QuestionAnswers;
      //ViewBag - динамический объект
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct(); //new string[] { "1ая тема", "2ая тема", "3ья тема" };
      return View(new QuestionAnswer());
    }

    [HttpPost]
    [MultiButton(Name = "action", Argument = "chooseSubject")]
    public ActionResult chooseSubject(string dropdownSubject) {
      string subject = dropdownSubject;
      ViewBag.Subject = subject;
      //извлекаем данные из таблицы TestList
      dbLines = testContext.QuestionAnswers;
      //ViewBag - динамический объект
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct();
      //количество записей в таблице которое удовлетворяет условию
      int count = dbLines.Select(p => p.Subject == subject).Count();
      ViewBag.QuestionAnswerListCount = count;//всего вопросов
      /*--------суть этого блочка вернуть рандомно строку из бд, которая соответствует теме-------*/
      Random rand = new Random();
      int index = rand.Next(1, count);
      //установим LeadUp для этого подхода
      Random rand1 = new Random();
      int leadUp = rand1.Next(1, 30000);
      //string leadUpStr = leadUp.ToString();
      QuestionAnswer dbLine = dbLines.Where(p => p.Subject == subject).Skip(index-1).Take(1).FirstOrDefault();
      //QuestionAnswer dbLine = dbLines.Where(p => p.Id == index).FirstOrDefault();
      ViewBag.QuestionsLeft = count-1;
      /*----------------*/
      dbLine.LeadUp = leadUp;
      testContext.SaveChanges();
      //rightCurrentAnswer = myList[index].Answer;
      //ViewBag.QuestionAnswers = dbline;
      //return View(model); /*вроде бы все ок, только хз как подхватить эту переменную model в представлении если писать @model.Question - не видит*/

      //dbLine.LeadUp = rand1.Next(0, 30000);
      return View(dbLine);
    }

    [HttpPost]
    [MultiButton(Name = "action", Argument = "allSubjects")]
    public ActionResult allSubjects() {
      ViewBag.Subject = "Выбрать тему";
      ViewBag.Disabled = "disabled";
      //извлекаем данные из таблицы TestList
      dbLines = testContext.QuestionAnswers;
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct();
      //количество записей в таблице
      int count = dbLines.Count();
      ViewBag.QuestionAnswerListCount = count;
      Random rand = new Random();
      int index = rand.Next(1, count);
      //установим LeadUp для этого подхода
      Random rand1 = new Random();
      int leadUp = rand1.Next(1, 30000);
      //string leadUpStr = leadUp.ToString();
      QuestionAnswer dbLine = dbLines.Skip(index - 1).Take(1).FirstOrDefault();
      //QuestionAnswer dbLine = dbLines.Where(p => p.Id == index).FirstOrDefault();
      dbLine.LeadUp = leadUp;
      testContext.SaveChanges();
      return View(dbLine);
    }

    [HttpPost]
    [MultiButton(Name = "action", Argument = "checkAnswer")]
    public ActionResult checkAnswer(QuestionAnswer model) {
      /*логика*/
      /*1)получаем данные с клиента: id строки из базы данных и Answer пользователя
       2)по полученному Id, находим эту строку в бд
       3) записываем в бд: AskAmount +1
       4) вытаскиваем из бд Answer и сравниваем с Answer полученным с клиента
       5) если answer пользователя совпадает с answer бд, то:
          - RightAnsAmount +1
          - ViewBag.IsItRightAnswer = "colorGreen";
       если answer пользователя не совпадает с answer бд,то:
          - ViewBag.IsItRightAnswer = "colorRed";
       6)сохраняем изменения в бд и выводим view*/
       
      //1)2)
      dbLines = testContext.QuestionAnswers;
      QuestionAnswer line;
      if(model.Subject == null) {
        line = selectDBLine(dbLines, model.LeadUp);
      } else {
        line = selectDBLine(model.LeadUp, model.Subject, dbLines);
      }
      //3) 
      if(line != null) {
        line.AskAmount++;
        if (model.Answer == line.Answer) {
          ViewBag.IsItRightAnswer = "colorGreen";
          line.RightAnsAmount++;
        }
        else {
          ViewBag.IsItRightAnswer = "colorRed";
        }
        //4)5)
        ViewBag.UserAnswer = model.Answer;
        //ViewBag.IsItRightAnswer = (userAnswer == rightCurrentAnswer) ? "colorGreen" : "colorRed";
        //6)
        ViewBag.RightAnswer = line.Answer;
        testContext.SaveChanges();
      } else {
        ViewBag.RightAnswer = "ошибка в программе, строка не найдена в бд";
      }
     
      /*делаем кнопку chechAnswer не активной, чтобы лишний раз не отправлять запрос на сервер.. новой инфы ведь и так не получит*/
      ViewBag.ViewBag.DisabledCheck = "disabled";
      return View(line);
    }

    [HttpPost]
    [MultiButton(Name = "action", Argument = "nextQuestion")]
    public ActionResult nextQuestion(QuestionAnswer model) {
      /*логика*/
      dbLines = testContext.QuestionAnswers;
      QuestionAnswer line;
      if (model.Subject == null) {
        line = selectDBLine(dbLines, model.LeadUp);
      } else {
        line = selectDBLine(model.LeadUp, model.Subject, dbLines);
      }
      if(line != null) {
        line.AskAmount++;
        line.LeadUp = model.LeadUp;
        testContext.SaveChanges();
      } else {
        line = emptyLine();
      }
      /*делаем кнопку chechAnswer активной*/
      ViewBag.ViewBag.DisabledCheck = "";//не обязательная строка
      return View(line);
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MultiButtonAttribute : ActionNameSelectorAttribute {
      public string Name { get; set; }
      public string Argument { get; set; }

      public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInf) {
        return controllerContext.HttpContext.Request[Name] != null && controllerContext.HttpContext.Request[Name] == Argument;
      }
    }

    /*функция для получения строки из таблицы. возвращает объект QuestionAnswers, в качестве параметров принимает таблицу бд и какие-то ее колнки*/
    //поиск по id
    QuestionAnswer selectDBLine(int id, IEnumerable<QuestionAnswer> dbLines) {
      QuestionAnswer line = dbLines.Where(u => u.Id == id).FirstOrDefault();
      if(line == null) {
        line = emptyLine();
      }
      return line;
    }
    //когда выбраны все темы, нужно выбрать строку из всей таблицы
    QuestionAnswer selectDBLine(IEnumerable<QuestionAnswer> dbLines, int leadUp) {
      QuestionAnswer line = dbLines.Where(u => u.LeadUp != leadUp).OrderBy(u => u.RightAnsAmount).ThenBy(u => u.AskAmount).First();
      if(line == null) {
        line = emptyLine();
      }
      return line;
    }
    //когда выбрана тема, нужно отобрать строку в теме
    QuestionAnswer selectDBLine(int leadUp, string subject, IEnumerable<QuestionAnswer> dbLines) {
     QuestionAnswer line = dbLines.Where(u => u.Subject == subject).Where(u => u.LeadUp != leadUp).OrderBy(u => u.RightAnsAmount).ThenBy(u => u.AskAmount).FirstOrDefault();
        //line = dbLines.Where(u => u.Subject == subject).Where(u => u.Id == id).FirstOrDefault();
        //var query1 = from u in dbLines
        //            where u.LeadUp != leadUp
        //            where u.Subject == subject
        //            where u.Id == id
        //            select u;
        //return new QuestionAnswers();
      /* если line = null, то тест закончен, нет больше доступных строк в базе, которые ты еще не прошел, а занчит, нужно предложить пройти этот тест сначала или начать другие тесты*/
      if (line == null) {
        line = emptyLine();
      }
      return line;
    }

    /*функция возрвщает QuestionAnswers объект, поля которого notfound или 0 - это будет говорить о том, что строка в бд не найдена. не принимает параметров*/
    QuestionAnswer emptyLine() {
      return new QuestionAnswer() { Id = -1, Subject = "not found", Question = "not found", Answer = "not found", AskAmount = 0, RightAnsAmount = 0, LeadUp = -1 };
    }


    /*end of Home page*/


    public ActionResult About() {
      ViewBag.Message = "Your application description page.";

      return View();
    }

    public ActionResult Contact() {
      ViewBag.Message = "Your contact page.";

      return View();
    }

    protected override void Dispose(bool disposing) {
      testContext.Dispose();
      base.Dispose(disposing);
    }
  }
}