namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class AccessPropertyTests : TestBase {
      public TaskVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new TaskVM();
         VM.InitializeFrom(new Task());
      }

      [TestMethod]
      public void SetValueGetValue_WithLocalProperty_Succeeds() {
         VM.ScreenTitle = ArbitraryString;
         Assert.AreEqual(ArbitraryString, VM.ScreenTitle);
      }

      [TestMethod]
      public void GetValue_MappedProperty_Success() {
         VM.SourceTask.Title = ArbitraryString;
         Assert.AreEqual(ArbitraryString, VM.Title);
      }

      [TestMethod]
      public void SetValue_MappedProperty_Success() {
         VM.Title = ArbitraryString;
         Assert.AreEqual(ArbitraryString, VM.SourceTask.Title);
      }

      [TestMethod]
      public void GetValue_CalculatedProperty_Success() {
         VM.SourceTask.Description = new RichText(ArbitraryString);
         Assert.AreEqual(ArbitraryString, VM.Description);
      }

      [TestMethod]
      public void SetValue_CalculatedProperty_Success() {
         VM.Description = ArbitraryString;
         Assert.AreEqual(ArbitraryString, VM.SourceTask.Description.Html);
      }
   }
}