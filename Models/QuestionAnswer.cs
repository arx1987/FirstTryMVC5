using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstTryMVC5.Models {
  public class QuestionAnswer {
    public int Id { get; set; }
    public string Subject { get; set; }
    public string Question { get; set; }
    public string Answer { get; set; }
    public int AskAmount { get; set; }
    public int RightAnsAmount { get; set; }
  }
}