namespace Inspiring.MvvmTest.ViewModels.Core.Behaviors {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class BehaviorKeysTests {
      [TestMethod]
      public void Key_CreatesKeyObjectWithExtractedFieldName() {
         Assert.IsNotNull(TestBehaviorKeys.TestKey);

         string expectedStringRepresentation = new BehaviorKey("TestKey").ToString();
         string actualStringRepresentation = TestBehaviorKeys.TestKey.ToString();

         Assert.AreEqual(expectedStringRepresentation, actualStringRepresentation);
      }

      private class TestBehaviorKeys : BehaviorKeys {
         public static readonly BehaviorKey TestKey = Key(() => TestKey);
      }
   }
}