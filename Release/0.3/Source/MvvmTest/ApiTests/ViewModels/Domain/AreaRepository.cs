namespace Inspiring.MvvmTest.ApiTests.ViewModels.Domain {
   public static class AreaRepository {
      public static Area[] GetAreas() {
         return new Area[] {
            new Area("GUI"),
            new Area("Domain"),
            new Area("DB")
         };
      }
   }
}
