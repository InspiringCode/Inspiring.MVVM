namespace Inspiring.MvvmTest.Common {
   using Inspiring.Mvvm.Common;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class HashCodeServiceTests : TestBase {
      [TestMethod]
      public void CanCalculateHashCode() {
         TestClass o1 = new TestClass {
            SomeIntProperty = 1,
            SomeStringProperty = "Test",
            SomeNotBusinessProperty = 2
         };
         TestClass o2 = new TestClass {
            SomeIntProperty = 2,
            SomeStringProperty = "Test",
            SomeNotBusinessProperty = 2
         };
         TestClass o3 = new TestClass {
            SomeIntProperty = 1,
            SomeStringProperty = "Test3",
            SomeNotBusinessProperty = 2
         };
         TestClass o4 = new TestClass {
            SomeIntProperty = 1,
            SomeStringProperty = "Test",
            SomeNotBusinessProperty = 3
         };

         int hash1 = HashCodeService.CalculateHashCode(o1, o1.SomeIntProperty, o1.SomeStringProperty);
         int hash2 = HashCodeService.CalculateHashCode(o2, o2.SomeIntProperty, o2.SomeStringProperty);
         int hash3 = HashCodeService.CalculateHashCode(o3, o3.SomeIntProperty, o3.SomeStringProperty);
         int hash4 = HashCodeService.CalculateHashCode(o4, o4.SomeIntProperty, o4.SomeStringProperty);

         Assert.AreNotEqual(hash1, hash2);
         Assert.AreNotEqual(hash1, hash3);
         Assert.AreNotEqual(hash2, hash3);
         Assert.AreEqual(hash1, hash4);
         Assert.AreNotEqual(hash2, hash4);
         Assert.AreNotEqual(hash3, hash4);
      }

      private class TestClass {
         public int SomeIntProperty { get; set; }
         public string SomeStringProperty { get; set; }
         public int SomeNotBusinessProperty { get; set; }
      }
   }
}