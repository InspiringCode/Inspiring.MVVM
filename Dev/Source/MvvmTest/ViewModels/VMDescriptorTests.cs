namespace Inspiring.MvvmTest.ViewModels {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class VMDescriptorTests : TestBase {
      [TestMethod]
      public void Properties_NoProperties_ReturnsEmptyCollection() {
         var descriptor = new EmptyDescriptor();

         Assert.IsNotNull(descriptor.Properties);
         Assert.IsFalse(descriptor.Properties.Any());
      }

      [TestMethod]
      public void Properties_SimpleProperty_ReturnsCollection() {
         var descriptor = new TestDescriptor() {
            SimpleProperty = new VMPropertyDescriptor<string>()
         };

         CollectionAssert.AreEquivalent(
            new IVMPropertyDescriptor[] { descriptor.SimpleProperty },
            descriptor.Properties.ToArray()
         );
      }

      [TestMethod]
      public void InitializePropertyNames_SimpleProperty_PropertyNameIsAssigned() {
         var descriptor = new TestDescriptor() {
            SimpleProperty = new VMPropertyDescriptor<string>()
         };

         descriptor.InitializePropertyNames();

         Assert.AreEqual("SimpleProperty", descriptor.SimpleProperty.PropertyName);
      }


      private class EmptyDescriptor : VMDescriptor {
      }

      private class TestDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> SimpleProperty { get; set; }
      }
   }
}
