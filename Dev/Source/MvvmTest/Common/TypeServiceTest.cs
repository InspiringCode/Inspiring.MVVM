namespace Inspiring.MvvmTest.Common {
   using System;
   using Inspiring.Mvvm.Common;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class TypeServiceTest : TestBase {
      [TestMethod]
      public void CanAssignNull_ReferenceType() {
         Assert.IsTrue(TypeService.CanAssignNull(typeof(object)));
         Assert.IsTrue(TypeService.CanAssignNull(typeof(Nullable<int>)));

         Assert.IsFalse(TypeService.CanAssignNull(typeof(int)));
         Assert.IsFalse(TypeService.CanAssignNull(typeof(TestStruct)));
      }

      [TestMethod]
      public void IsNullableType() {
         Assert.IsTrue(TypeService.IsNullableType(typeof(Nullable<int>)));
         Assert.IsFalse(TypeService.IsNullableType(typeof(object)));
      }

      private struct TestStruct {
      }
   }
}