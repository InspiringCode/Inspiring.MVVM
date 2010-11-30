namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class AccessPropertyTests {
      private const string ArbitraryString = "Test";

      public TaskVM VM { get; set; }
      
      [TestInitialize]
      public void Setup() {
         VM = new TaskVM();
         VM.InitializeFrom(new Task());
      }

      [TestMethod]
      public void GetValue_LocalProperty_Success() {
         VM.ScreenTitle = ArbitraryString;
         Assert.AreEqual(ArbitraryString, VM.ScreenTitle);
      }

      [TestMethod]
      public void SetValue_LocalProperty_Success() {
         VM.ScreenTitle = ArbitraryString;
         Assert.AreEqual(ArbitraryString, VM.ScreenTitle);
      }
   }
}