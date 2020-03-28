using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace FirstTryMVC5.Models {
  public class DBInitializer : DropCreateDatabaseAlways<TestContext> {
    protected override void Seed(TestContext context) {
      context.TestsList.Add(new QuestionAnswer() { Subject = "1-ая тема", Question = "первый вопрос", Answer = "правильный ответ на первый вопрос", AskAmount = 0, RightAnsAmount = 0 });
      context.TestsList.Add(new QuestionAnswer() { Subject = "1-ая тема", Question = "второй вопрос", Answer = "правильный ответ на второй вопрос", AskAmount = 0, RightAnsAmount = 0 });
      context.TestsList.Add(new QuestionAnswer() { Subject = "1-ая тема", Question = "третий вопрос", Answer = "правильный ответ на третий вопрос", AskAmount = 0, RightAnsAmount = 0 });
      context.TestsList.Add(new QuestionAnswer() { Subject = "2-ая тема", Question = "первый вопрос", Answer = "правильный ответ на первый вопрос", AskAmount = 0, RightAnsAmount = 0 });
      context.TestsList.Add(new QuestionAnswer() { Subject = "2-ая тема", Question = "Второй вопрос", Answer = "правильный ответ на первый вопрос", AskAmount = 0, RightAnsAmount = 0 });
      context.TestsList.Add(new QuestionAnswer() { Subject = "2-ая тема", Question = "третий вопрос", Answer = "правильный ответ на третий вопрос", AskAmount = 0, RightAnsAmount = 0 });
      context.TestsList.Add(new QuestionAnswer() { Subject = "3-я тема", Question = "первый вопрос", Answer = "правильный ответ на первый вопрос", AskAmount = 0, RightAnsAmount = 0 });
      context.TestsList.Add(new QuestionAnswer() { Subject = "3-я тема", Question = "второй вопрос", Answer = "правильный ответ на второй вопрос", AskAmount = 0, RightAnsAmount = 0 });
      context.TestsList.Add(new QuestionAnswer() { Subject = "3-я тема", Question = "третий вопрос", Answer = "правильный ответ на третий вопрос", AskAmount = 0, RightAnsAmount = 0 });
      base.Seed(context);
    }
  }
}