using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace FirstTryMVC5.Models {
  public class TestContext : DbContext {
    public DbSet<QuestionAnswer> QuestionAnswers { get; set; }
  }
}