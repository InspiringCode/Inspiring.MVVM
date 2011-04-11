namespace Inspiring.MvvmTest.Common {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.Common;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class TypeServiceTests : TestBase {
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

      [TestMethod]
      public void GetFriendlyName_ForNonGenericType_ReturnsTypeName() {
         var t = typeof(Object);
         Assert.AreEqual("Object", TypeService.GetFriendlyName(t));
      }

      [TestMethod]
      public void GetFriendlyName_ForGenericType_ReturnsNameInCharpSyntax() {
         var t = typeof(List<Nullable<TestStruct>>);
         Assert.AreEqual("List<Nullable<TestStruct>>", TypeService.GetFriendlyName(t));
      }

      [TestMethod]
      public void GetFriendlyTypeName_OfInstance_ReturnsTypeName() {
         var instance = new Object();
         Assert.AreEqual("Object", TypeService.GetFriendlyTypeName(instance));
      }

      [TestMethod]
      public void GetFriendlyTypeName_OfNull_ReturnsDescriptiveString() {
         Assert.AreEqual("<NULL>", TypeService.GetFriendlyTypeName(null));
      }

      private struct TestStruct {
      }
   }
}