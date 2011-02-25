namespace Inspiring.MvvmTest.ApiTests.ViewModels.Domain {
   /// <summary>
   ///   The area of a user story or task (e.g. GUI, Domain, DB).
   /// </summary>
   public sealed class Area {
      public Area(string caption) {
         Caption = caption;
      }

      public string Caption { get; set; }
   }
}
