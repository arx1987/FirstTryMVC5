using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using FirstTryMVC5.Models;

namespace FirstTryMVC5.Controllers {
  public class HomeController : Controller {
    TestContext testContext = new TestContext();
    IEnumerable<QuestionAnswer> dbLines;
    //ViewBag.AllSubjectsOn -> 0 - если не выбрана какая-то тема и 1 - если нажата кнопка "Все темы"

    [HttpGet]
    public ActionResult Index() {
      ViewBag.Subject = "Выбрать тему";
      ViewBag.DisabledCheck = "disabled";
      ViewBag.DisabledNext = "disabled";
      ViewBag.EnterButton = "testBut";
      //извлекаем данные из таблицы TestList
      dbLines = testContext.QuestionAnswers;
      //ViewBag - динамический объект
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct(); //new string[] { "1ая тема", "2ая тема", "3ья тема" };
      return View(new QuestionAnswer() { Question = "Выберите тему для повторения" });//этот вариант работает если во View: @model FirstTryMVC5.Models.QuestionAnswer 
      //return View(testContext.QuestionAnswers);/*этот вариант(вариант из видео) работает если во View писать модель как @model IEnumerable<FirstTryMVC5.Models.QuestionAnswer>*/
    }

    [HttpPost]
    [MultiButton(Name = "action", Argument = "chooseSubject")]
    public ActionResult chooseSubject(string dropdownSubject) {
      string subject = dropdownSubject;
      ViewBag.Subject = subject;
      ViewBag.AllSubjectsOn = "0";
      //извлекаем данные из таблицы TestList
      dbLines = testContext.QuestionAnswers;
      //ViewBag - динамический объект
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct();
      //количество записей в таблице которое удовлетворяет условию
      int count = dbLines.Where(p => p.Subject == subject).Count();
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
      ViewBag.RightAnswersCount = 0;
      /*----------------*/
      dbLine.LeadUp = leadUp;
      ViewBag.Autofocus = "autofocus";
      ViewBag.EnterButton = "checkAnswer";
      testContext.SaveChanges();
      //rightCurrentAnswer = myList[index].Answer;
      //ViewBag.QuestionAnswers = dbline;
      //return View(model); /*вроде бы все ок, только хз как подхватить эту переменную model в представлении если писать @model.Question - не видит*/

      //dbLine.LeadUp = rand1.Next(0, 30000);
      return View(dbLine);
    }

    [HttpPost]
    [MultiButton(Name = "action", Argument = "allSubjects")]
    public ActionResult allSubjects(QuestionAnswer model) {
      ViewBag.Subject = "Выбрать тему";
      ViewBag.Disabled = "disabled";
      ViewBag.AllSubjectsOn = "1";
      model.Subject = null;
      //извлекаем данные из таблицы TestList
      dbLines = testContext.QuestionAnswers;
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct();
      //количество записей в таблице
      int count = dbLines.Count();
      ViewBag.QuestionAnswerListCount = count;
      ViewBag.QuestionsLeft = count - 1;
      ViewBag.RightAnswersCount = 0;
      Random rand = new Random();
      int index = rand.Next(1, count);
      //установим LeadUp для этого подхода
      Random rand1 = new Random();
      int leadUp = rand1.Next(1, 30000);
      //string leadUpStr = leadUp.ToString();
      QuestionAnswer dbLine = dbLines.Skip(index - 1).Take(1).FirstOrDefault();
      //QuestionAnswer dbLine = dbLines.Where(p => p.Id == index).FirstOrDefault();
      ViewBag.Autofocus = "autofocus";
      ViewBag.EnterButton = "checkAnswer";
      dbLine.LeadUp = leadUp;
      testContext.SaveChanges();
      return View(dbLine);
    }

    [HttpPost]
    [MultiButton(Name = "action", Argument = "checkAnswer")]
    public ActionResult checkAnswer(QuestionAnswer model, string allSubjectsOn, string questionAnswerListCount, string questionsLeft, string rightAnswersCount) {
      /*логика
       получаем данные из формы, где вопрос-Question и тема - Subject соответствуют заданному пользователю вопросу из бд. Задача, получить именно эту строку вопроса из бд, сравнить ответ в бд с ответом пользователя и вывести результат
       1) получаем данные
       2) находим соответствующую строку в бд
       3) берем оттуда правильный ответ
       4) сравниваем его с ответом пользователя
       5) если answer пользователя совпадает с answer бд, то:
          - RightAnsAmount +1
          - ViewBag.IsItRightAnswer = "colorGreen";
       если answer пользователя не совпадает с answer бд,то:
          - ViewBag.IsItRightAnswer = "colorRed";
       6)сохраняем изменения в бд и выводим view*/

      //1)2)
      //переведем параметр правильных ответов в число
      int rAnsCnt = Convert.ToInt32(rightAnswersCount);
      dbLines = testContext.QuestionAnswers;
      ViewBag.AllSubjectsOn = allSubjectsOn;
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct();
      QuestionAnswer line;
      if(allSubjectsOn == "1") {
        //если тема не выбрана
        ViewBag.Subject = "Выбрать тему";
        //line = selectDBLine(dbLines, model.LeadUp);
      } else {
        //установим текущую тему в представлении, если она выбрана
        ViewBag.Subject = model.Subject;
        //line = selectDBLine(model.LeadUp, model.Subject, model.Question, dbLines);
      }
      //3) 
      line = selectDBLine(model.Id, dbLines);
      if (line != null) {
        line.AskAmount++;
        if (model.Answer == line.Answer) {
          ViewBag.IsItRightAnswer = "colorGreen";
          rAnsCnt++;
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
      /*задаем значения количеств вопросов и ответов*/
      ViewBag.QuestionAnswerListCount = questionAnswerListCount;
      ViewBag.QuestionsLeft = questionsLeft;
      ViewBag.RightAnswersCount = rAnsCnt;
      /*делаем кнопку chechAnswer не активной, чтобы лишний раз не отправлять запрос на сервер.. новой инфы ведь и так не получит*/
      ViewBag.DisabledCheck = "disabled";
      ViewBag.Autofocus = "autofocus";
      ViewBag.EnterButton = "nextQuestion";
      return View(line);
    }

    [HttpPost]
    [MultiButton(Name = "action", Argument = "nextQuestion")]
    public ActionResult nextQuestion(QuestionAnswer model, string allSubjectsOn, string questionAnswerListCount, string questionsLeft, string rightAnswersCount) {
      /*логика*/
      //переводим количество оставшихся вопросов в инт и отнимаем один
      int qLft = Convert.ToInt32(questionsLeft);
      qLft--;
      ViewBag.AllSubjectsOn = allSubjectsOn;
      dbLines = testContext.QuestionAnswers;
      QuestionAnswer line;
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct();
      /*В зависимости от того выбрана одна тема или все темы, нужны разные запросы к бд. Чтобы показать следующий вопрос*/
      //if (model.Subject == null) {
      if(allSubjectsOn == "1") { 
        //если тема не выбрана - т.е. выбраны все темы
        ViewBag.Subject = "Выбрать тему";
        line = selectDBLine(dbLines, model.LeadUp);
      } else {
        //установим текущую тему в представлении, если она выбрана
        ViewBag.Subject = model.Subject;
        line = selectDBLine(model.LeadUp, model.Subject, dbLines);
      }
      if(line.Id != -1) {
        line.AskAmount++;
        line.LeadUp = model.LeadUp;
        testContext.SaveChanges();
        ViewBag.QuestionAnswerListCount = questionAnswerListCount;
        ViewBag.QuestionsLeft = qLft;
        ViewBag.RightAnswersCount = rightAnswersCount;
      } else {
        //нужно проверить почему пустая строка, потому что запрос не верен или потому, что закончились строки
        //если строки закончились - сообщим об этом в окошке Question
        ViewBag.QuestionAnswerListCount = questionAnswerListCount;
        ViewBag.QuestionsLeft = questionAnswerListCount;
        ViewBag.RightAnswersCount = 0;
        //делаем кнопки checkAnswer и nextQuestion не активными
        ViewBag.DisabledCheck = "disabled";
        ViewBag.DisabledNext = "disabled";
        return View(new QuestionAnswer() {
          Id = -1,
          Subject = "not found",
          Question = "Вы прошли все вопросы этой темы. " +
          "\nВсего вопросов: " + questionAnswerListCount + 
          "\nПравильных ответов: " + rightAnswersCount + 
          "\nВыберите следующую тему",
          Answer = "not found",
          AskAmount = 0,
          RightAnsAmount = 0,
          LeadUp = -1
        });
      }

      ViewBag.Autofocus = "autofocus";
      ViewBag.EnterButton = "checkAnswer";

      /*делаем кнопку checkAnswer активной*/
      ViewBag.DisabledCheck = "";//не обязательная строка
      return View(line);
    }

    [HttpPost]
    [MultiButton(Name = "action", Argument = "testBut")]
    public ActionResult testBut(QuestionAnswer model, string allSubjectsOn) {
      ViewBag.AllSubjectsOn = allSubjectsOn;
      dbLines = testContext.QuestionAnswers;
      ViewBag.Subject = model.Subject;
      QuestionAnswer line = selectDBLine(model.LeadUp, model.Subject, dbLines);
      ViewBag.EnterButton = "testBut";
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
      QuestionAnswer line = dbLines.Where(u => u.LeadUp != leadUp).OrderBy(u => u.RightAnsAmount).ThenBy(u => u.AskAmount).FirstOrDefault();
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
    //для проверки правильного ответа(в параметрах передаются leadUp, тема, вопрос, ну и таблица), возвращает строку из бд
    QuestionAnswer selectDBLine(int leadUp, string subject, string question, IEnumerable<QuestionAnswer> dbLines) {
      QuestionAnswer line = dbLines.Where(u => u.Subject == subject).Where(u => u.LeadUp != leadUp).Where(u => u.Question == question).FirstOrDefault();

      if (line == null) {
        line = emptyLine();
      }
      return line;
    }

    /*функция возрвщает QuestionAnswers объект, поля которого notfound или 0 - это будет говорить о том, что строка в бд не найдена. не принимает параметров*/
    QuestionAnswer emptyLine() {
      return new QuestionAnswer() { Id = -1, Subject = "not found", Question = "not found", Answer = "not found", AskAmount = 0, RightAnsAmount = 0, LeadUp = -1 };
    }


    /*-------------------------------------end of Home page-------------------------------------------------*/

    public ActionResult AddToDb() {
      ViewBag.Subject = "Выберите тему";
      //извлекаем данные из таблицы TestList
      dbLines = testContext.QuestionAnswers;
      //ViewBag - динамический объект
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct(); //new string[] { "1ая тема", "2ая тема", "3ья тема" };
      return View();
    }

    [HttpPost]
    [MultiButton(Name = "action", Argument = "chooseSubject2")]
    public ActionResult chooseSubject2(string dropdownSubject, string Question, string Answer) {
      //добавлены параметры Question, Answer -> если забыли указать тему, но заполнили поля формы - чтобы поля не исчезали, при выборе темы
      ViewBag.Subject = dropdownSubject;
      ViewBag.Question = Question;
      ViewBag.Answer = Answer;
      //извлекаем данные из таблицы TestList
      dbLines = testContext.QuestionAnswers;
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct();
      return View();
    }


    [HttpPost]
    [MultiButton(Name = "action", Argument = "addDataToDB")]
    public ActionResult addDataToDB(QuestionAnswer questionAnswer, string dropdownSubject) {
      dbLines = testContext.QuestionAnswers;
      testContext.QuestionAnswers.Add(questionAnswer);
      testContext.SaveChanges();
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct();
      ViewBag.Subject = "Выберите тему";
      ViewBag.SendingResult = "Данные были добавлены в базу";
      ViewBag.IsItRightAnswer = "colorGreen";
      return View();
    }
    /*------------------------------end of AddToDb ------------------------------------------------------*/

    [HttpGet]
    public ActionResult AddToDbFromFile() {
      return View();
    }

    [HttpPost]
    /*public ActionResult AddToDbFromFile(IEnumerable<HttpPostedFileBase> files) {
    foreach (var file in files) {
        string filePath = Guid.NewGuid() + Path.GetExtension(file.FileName);
    file.SaveAs(Path.Combine(Server.MapPath("~/Content/UploadedFiles"), filePath));
        //Here you can write code for save this information in your database if you want
      }
      return Content("Success");
     } 
     */
  public ActionResult AddToDbFromFile(HttpPostedFileBase upload) {
      if(upload != null) { 
        //полчаем имя файла
        string fileName = System.IO.Path.GetFileName(upload.FileName);
        //сохраняем файл в папку Content/UploadedFiles
        upload.SaveAs(Server.MapPath("~/Content/UploadedFiles/" + fileName));
        //string path1 = "~/Content/UploadedFiles/" + fileName;
        string path = Server.MapPath(Url.Content("~/Content/UploadedFiles/" + fileName));
        //считаем построчно файл:
        using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8)) {
          string line = null;
          string currentSubject = null;
          while((line = sr.ReadLine()) != null) {
            if (line.StartsWith("#")) {//отбрем темы
              currentSubject = line.Substring(1);
            } else if(currentSubject != "" && currentSubject != null) {
              string[] questionAnswer = line.Split(new char[] { '~' });
              testContext.QuestionAnswers.Add(new QuestionAnswer {
                Subject = currentSubject,
                Question = questionAnswer[0],
                Answer = questionAnswer[1],
                AskAmount = 0,
                RightAnsAmount = 0,
                LeadUp = 0
              });
            }
          }
          testContext.SaveChanges();
        }
        //нужно удалить файл, который сохранился в Uploads
        System.IO.File.Delete(path);
      }
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