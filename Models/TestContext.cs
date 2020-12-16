using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace FirstTryMVC5.Models {
  public class TestContext : DbContext {
    public DbSet<CQuestionAnswer> CQuestionAnswers { get; set; }
    public DbSet<MVCQuestionAnswer> MVCQuestionAnswers { get; set; }
    public DbSet<EnQuestionAnswer> EnQuestionAnswers { get; set; }
  }
}