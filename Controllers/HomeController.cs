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
    //чтобы выбрать какюу-то определенную таблику можно написать
    //var dbMVCLines = testContext.MVCQuestionAnswers;
    //или IEnumerable<QuestionAnswer> dbLines = testContext.QuestionAnswers;

    [HttpGet]
    public ActionResult Index() {
      return View();
    }

    [HttpGet]
    public ActionResult Index1(string pickTest) {
      /*будем использовать различные модели в зависимости от того, какой параметр приходит со странички Index. Пока что такие варианты: C#, MVC, English.*/
      //switch (pickTest) {
      //  case "C#":
      //    dbLines = testContext.CQuestionAnswers; ;
      //    break;
      //  case "MVC":
      //    IEnumerable<MVCQuestionAnswer> dbLines;
      //    break;
      //  case "English":
      //    IEnumerable<EnQuestionAnswer> dbLines;
      //    break;
      //}
      //if (pickTest == "C#") { dbLines = testContext.CQuestionAnswers; }
      //else if (pickTest == "MVC") { dbLines = testContext.MVCQuestionAnswers; }
      //else { dbLines = testContext.EnQuestionAnswers ; }
      if (pickTest == null) {
        return View("Index");
      }
      dbLines = getRightModel(pickTest);
      ViewBag.GetModel = pickTest;
      ViewBag.Subject = "Выбрать тему";
      ViewBag.DisabledCheck = "disabled";
      ViewBag.DisabledNext = "disabled";
      ViewBag.EnterButton = "testBut";
      //извлекаем данные из таблицы TestList
      //dbLines = testContext.QuestionAnswers;
      //ViewBag - динамический объект
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct(); //new string[] { "1ая тема", "2ая тема", "3ья тема" };
      return View(new QuestionAnswer() { Question = "Выберите тему для повторения" });//этот вариант работает если во View: @model FirstTryMVC5.Models.QuestionAnswer 
      //return View(testContext.QuestionAnswers);/*этот вариант(вариант из видео) работает если во View писать модель как @model IEnumerable<FirstTryMVC5.Models.QuestionAnswer>*/
    }

    [HttpPost]
    [MultiButton(Name = "action", Argument = "chooseSubject")]
    public ActionResult chooseSubject(string dropdownSubject, string getModel) {
      //if (model == "C#") { dbLines = testContext.CQuestionAnswers; }
      //else if (model == "MVC") { dbLines = testContext.MVCQuestionAnswers; }
      //else { dbLines = testContext.EnQuestionAnswers; }
      string subject = dropdownSubject;
      ViewBag.Subject = subject;
      ViewBag.AllSubjectsOn = "0";
      //извлекаем данные из таблицы
      //dbLines = testContext.CQuestionAnswers;
      dbLines = getRightModel(getModel);
      ViewBag.GetModel = getModel;
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
      QuestionAnswer dbLine = dbLines.Where(p => p.Subject == subject).Skip(index - 1).Take(1).FirstOrDefault();
      //QuestionAnswer dbLine = dbLines.Where(p => p.Id == index).FirstOrDefault();
      ViewBag.QuestionsLeft = count - 1;
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
    public ActionResult allSubjects(QuestionAnswer model, string getModel) {
      ViewBag.Subject = "Выбрать тему";
      ViewBag.Disabled = "disabled";
      ViewBag.AllSubjectsOn = "1";
      model.Subject = null;
      //извлекаем данные из таблицы TestList
      //dbLines = testContext.CQuestionAnswers;
      dbLines = getRightModel(getModel);
      ViewBag.GetModel = getModel;
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
    public ActionResult checkAnswer(QuestionAnswer model, string allSubjectsOn, string questionAnswerListCount, string questionsLeft, string rightAnswersCount, string getModel) {
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
      //dbLines = testContext.CQuestionAnswers;
      dbLines = getRightModel(getModel);
      ViewBag.GetModel = getModel;
      ViewBag.AllSubjectsOn = allSubjectsOn;
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct();
      QuestionAnswer line;
      if (allSubjectsOn == "1") {
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
        string usrAnsTrimToLower = (model.Answer == null) ? "" : model.Answer.ToLower().Trim();
        bool isRightAnswer = false;
        string[] rightAnswers = line.Answer.Split('~');
        foreach (string s in rightAnswers) {
          if (usrAnsTrimToLower == s.ToLower().Trim())
            isRightAnswer = true;
        }
        if (isRightAnswer) {
          ViewBag.IsItRightAnswer = "colorGreen";
          rAnsCnt++;
          line.RightAnsAmount++;
          ViewBag.RightAnswer = model.Answer;
        }
        else {
          ViewBag.IsItRightAnswer = "colorRed";
          ViewBag.RightAnswer = rightAnswers[0];
        }
        //4)5)
        ViewBag.UserAnswer = model.Answer;
        //ViewBag.IsItRightAnswer = (userAnswer == rightCurrentAnswer) ? "colorGreen" : "colorRed";
        //6)

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
    public ActionResult nextQuestion(QuestionAnswer model, string allSubjectsOn, string questionAnswerListCount, string questionsLeft, string rightAnswersCount, string getModel) {
      /*логика*/
      //переводим количество оставшихся вопросов в инт и отнимаем один
      int qLft = Convert.ToInt32(questionsLeft);
      qLft--;
      ViewBag.AllSubjectsOn = allSubjectsOn;
      //dbLines = testContext.CQuestionAnswers;
      dbLines = getRightModel(getModel);
      ViewBag.GetModel = getModel;
      QuestionAnswer line;
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct();
      /*В зависимости от того выбрана одна тема или все темы, нужны разные запросы к бд. Чтобы показать следующий вопрос*/
      //if (model.Subject == null) {
      if (allSubjectsOn == "1") {
        //если тема не выбрана - т.е. выбраны все темы
        ViewBag.Subject = "Выбрать тему";
        line = selectDBLine(dbLines, model.LeadUp);
      } else {
        //установим текущую тему в представлении, если она выбрана
        ViewBag.Subject = model.Subject;
        line = selectDBLine(model.LeadUp, model.Subject, dbLines);
      }
      if (line.Id != -1) {
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
      dbLines = testContext.CQuestionAnswers;
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

    /*метод для определения таблицы контекста данных*/
    public IEnumerable<QuestionAnswer> getRightModel(string model) {
      if (model == "C#") { dbLines = testContext.CQuestionAnswers; }
      else if (model == "MVC") { dbLines = testContext.MVCQuestionAnswers; }
      else { dbLines = testContext.EnQuestionAnswers; }
      return dbLines;
    }

    /*метод для добавления в бд данных в соответствующую таблицу*/
    void addToDbsTable(QuestionAnswer questionAnswer, string model) {
      if (model == "C#") { testContext.CQuestionAnswers.Add((CQuestionAnswer)questionAnswer); }
      else if (model == "MVC") { testContext.MVCQuestionAnswers.Add((MVCQuestionAnswer)questionAnswer); }
      else { testContext.EnQuestionAnswers.Add((EnQuestionAnswer)questionAnswer); }
    }

    /*метод для добавлени в бд данных с файла в соответствующую таблицу*/
    void addToDbsTableFromFile(string currentSubject, string[] questionAnswer, string line, string model) {
      if (model == "C#") { testContext.CQuestionAnswers.Add(
         new CQuestionAnswer {
           Subject = currentSubject,
           Question = questionAnswer[0],
           Answer = line.Replace(questionAnswer[0] + "~", ""),//questionAnswer[1],
          AskAmount = 0,
           RightAnsAmount = 0,
           LeadUp = 0
         });
      }
      else if (model == "MVC") { testContext.MVCQuestionAnswers.Add(
        new MVCQuestionAnswer {
          Subject = currentSubject,
          Question = questionAnswer[0],
          Answer = line.Replace(questionAnswer[0] + "~", ""),//questionAnswer[1],
          AskAmount = 0,
          RightAnsAmount = 0,
          LeadUp = 0
        });
      }
      else { testContext.EnQuestionAnswers.Add(
        new EnQuestionAnswer {
          Subject = currentSubject,
          Question = questionAnswer[0],
          Answer = line.Replace(questionAnswer[0] + "~", ""),//questionAnswer[1],
          AskAmount = 0,
          RightAnsAmount = 0,
          LeadUp = 0
        });
      }
      //testContext.CQuestionAnswers.Add(new CQuestionAnswer {
      //  Subject = currentSubject,
      //  Question = questionAnswer[0],
      //  Answer = line.Replace(questionAnswer[0]+"~", ""),//questionAnswer[1],
      //  AskAmount = 0,
      //  RightAnsAmount = 0,
      //  LeadUp = 0
      //});
    }

    /*функция для получения строки из таблицы. возвращает объект QuestionAnswers, в качестве параметров принимает таблицу бд и какие-то ее колонки*/
    //поиск по id
    QuestionAnswer selectDBLine(int id, IEnumerable<QuestionAnswer> dbLines) {
      QuestionAnswer line = dbLines.Where(u => u.Id == id).FirstOrDefault();
      if (line == null) {
        line = emptyLine();
      }
      return line;
    }
    //когда выбраны все темы, нужно выбрать строку из всей таблицы
    QuestionAnswer selectDBLine(IEnumerable<QuestionAnswer> dbLines, int leadUp) {
      //получаем количество записей в таблице, которые !=leadUp
      int count = dbLines.Where(u => u.LeadUp != leadUp).Count();
      QuestionAnswer line;
      if (count >= 1) {
        //выбираем рандомно номер записи
        Random rand = new Random();
        int index = rand.Next(1, count);
        //Пропускаем количество записей count-1, и запись(строку таблицы) с номером count возвращаем
        line = dbLines.Where(u => u.LeadUp != leadUp).Skip(index - 1).Take(1).FirstOrDefault();
        //QuestionAnswer line = dbLines.Where(u => u.LeadUp != leadUp).OrderBy(u => u.RightAnsAmount).ThenBy(u => u.AskAmount).FirstOrDefault();
      } else {
        line = null;
      }
      if (line == null) {
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
    public ActionResult AddToDb(string getModel) {
      if (getModel == null) {
        return View("Index");
      }
      ViewBag.Subject = "Выберите тему";
      //извлекаем данные из таблицы TestList
      //dbLines = testContext.CQuestionAnswers;
      dbLines = getRightModel(getModel);
      ViewBag.GetModel = getModel;
      //ViewBag - динамический объект
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct(); //new string[] { "1ая тема", "2ая тема", "3ья тема" };
      return View();
    }

    [HttpPost]
    [MultiButton(Name = "action", Argument = "chooseSubject2")]
    public ActionResult chooseSubject2(string dropdownSubject, string Question, string Answer, string getModel) {
      //добавлены параметры Question, Answer -> если забыли указать тему, но заполнили поля формы - чтобы поля не исчезали, при выборе темы
      ViewBag.Subject = dropdownSubject;
      ViewBag.Question = Question;
      ViewBag.Answer = Answer;
      //извлекаем данные из таблицы TestList
      //dbLines = testContext.CQuestionAnswers;
      dbLines = getRightModel(getModel);
      ViewBag.GetModel = getModel;
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct();
      return View();
    }


    [HttpPost]
    [MultiButton(Name = "action", Argument = "addDataToDB")]
    public ActionResult addDataToDB(QuestionAnswer questionAnswer, string dropdownSubject, string getModel) {
      //dbLines = testContext.CQuestionAnswers;
      dbLines = getRightModel(getModel);
      ViewBag.GetModel = getModel;
      //добавим данные в соответствующую таблицу
      //testContext.CQuestionAnswers.Add(questionAnswer);
      addToDbsTable(questionAnswer, getModel);
      testContext.SaveChanges();
      //получим из бд все темы
      ViewBag.AllSubjects = dbLines.Select(p => p.Subject).Distinct();
      ViewBag.Subject = "Выберите тему";
      ViewBag.SendingResult = "Данные были добавлены в базу";
      ViewBag.IsItRightAnswer = "colorGreen";
      return View();
    }
    /*------------------------------end of AddToDb ------------------------------------------------------*/
    /*-----start ChangeDBLine----------*/
    [HttpGet]
    public ActionResult FindDBLine(string getModel) {
      if (getModel == null) {
        return View("Index");
      }
      ViewBag.GetModel = getModel;
      ViewBag.FindDBLineBut = "Найти";
      ViewBag.DisplayDiv = "none";
      return View();
    }

    [HttpPost]
    public ActionResult FindDBLine(string findQuestion, string getModel) {
      if (getModel == null) {
        return View("Index");
      }
      ViewBag.GetModel = getModel;
      if (findQuestion == null) {
        ViewBag.GetModel = getModel;
        ViewBag.FindDBLineBut = "Найти";
        ViewBag.DisplayDiv = "none";
        return View();
      } else {
        dbLines = getRightModel(getModel);
        //блок поиска в бд в определенной таблице getModel
        ViewBag.SearchResults = dbLines.Where(u => u.Question.Contains(findQuestion));
        //если в этой таблице не было найдено совпадений с findQueston
        if (ViewBag.SearchResults == null) {
          ViewBag.ThereIsNoResultInDB = "В Таблице " + getModel + " не было найдено совпадений со строкой " + findQuestion;
          ViewBag.DisplayDiv = "none";
        } else {
          ViewBag.DisplayDiv = "block";
          return View();
        }
        //

      }
      return View();
    }
    /*-----end of FindDBLine---------*/
    /*-----start of EditDBLine---------*/
    [HttpGet]
    public ActionResult EditDBLine(int? id, string subject, string question, string answer, string getModel) {
      if (getModel == null || id == null) {
        return View("Index");
      }
      ViewBag.GetModel = getModel;
      ViewBag.Id = id;
      ViewBag.Subject = subject;
      ViewBag.Question = question;
      ViewBag.Answer = answer;
      return View();
    } 

    [HttpPost]
    public ActionResult EditDBLine(string getModel) {
      if (getModel == null) {
        return View("Index");
      }
      ViewBag.GetModel = getModel;
      int id;
      if (!(Int32.TryParse(Request.Form["id"], out id))) {
        ViewBag.ThereIsNotResultInDb = "Изменения не были сохранены, так как такая строка не найдена";
        ViewBag.GetModel = getModel;
        ViewBag.FindDBLineBut = "Найти";
        ViewBag.DisplayDiv = "none";
        return View("FindDBLine");
      } else {
        //dbLines = getRightModel(getModel);
        //QuestionAnswer line = selectDBLine(Convert.ToInt32(Request.Form["id"]), dbLines );
        string question = Request.Form["Question"];
        string answer = Request.Form["Answer"];
        string subject = Request.Form["Subject"];
        if (getModel == "C#") {
          CQuestionAnswer line = testContext.CQuestionAnswers.Where(u => u.Id == id).FirstOrDefault();
          line.Answer = answer;
          line.Question = question;
          line.Subject = subject;
          testContext.Entry(line).State = System.Data.Entity.EntityState.Modified;
          testContext.SaveChanges();
        } else if(getModel == "MVC") {
          MVCQuestionAnswer line = testContext.MVCQuestionAnswers.Where(u => u.Id == id).FirstOrDefault();
          line.Answer = answer;
          line.Question = question;
          line.Subject = subject;
          testContext.Entry(line).State = System.Data.Entity.EntityState.Modified;
          testContext.SaveChanges();
        } else {
          EnQuestionAnswer line = testContext.EnQuestionAnswers.Where(u => u.Id == id).FirstOrDefault();
          line.Answer = answer;
          line.Question = question;
          line.Subject = subject;
          testContext.Entry(line).State = System.Data.Entity.EntityState.Modified;
          testContext.SaveChanges();
        }
        ViewBag.ThereIsNotResultInDb = "Изменения были сохранены";
        ViewBag.GetModel = getModel;
        ViewBag.FindDBLineBut = "Найти";
        ViewBag.DisplayDiv = "none";
        return View("FindDBLine");
      }
    }
    /*-----end of EditDBLine---------*/

    [HttpGet]
    public ActionResult AddToDbFromFile(string getModel) {
      if (getModel == null) {
        return View("Index");
      }      
      ViewBag.GetModel = getModel;
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
  public ActionResult AddToDbFromFile(HttpPostedFileBase upload, string getModel) {
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
            if (line.StartsWith("#")) {//отберем темы
              currentSubject = line.Substring(1);
            } else if(currentSubject != "" && currentSubject != null) {
              string[] questionAnswer = line.Split(new char[] { '~' });
              addToDbsTableFromFile(currentSubject, questionAnswer, line, getModel);
              //testContext.CQuestionAnswers.Add(new CQuestionAnswer {
              //  Subject = currentSubject,
              //  Question = questionAnswer[0],
              //  Answer = line.Replace(questionAnswer[0]+"~", ""),//questionAnswer[1],
              //  AskAmount = 0,
              //  RightAnsAmount = 0,
              //  LeadUp = 0
              //});
            }
          }
          testContext.SaveChanges();
        }
        //нужно удалить файл, который сохранился в Uploads
        System.IO.File.Delete(path);
        ViewBag.DataHaveBeenAdded = "Данные были успешно добавлены в таблицу " + getModel;
      } else {
        ViewBag.DataHaveBeenAdded = "Данные не были добавлены в таблицу " + getModel;
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