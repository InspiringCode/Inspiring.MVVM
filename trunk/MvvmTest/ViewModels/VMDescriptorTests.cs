namespace Inspiring.MvvmTest.ViewModels {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class VMDescriptorTests {
      [TestMethod]
      public void Properties_NoProperties_ReturnsEmptyCollection() {
         var descriptor = new EmptyDescriptor();

         Assert.IsNotNull(descriptor.Properties);
         Assert.IsFalse(descriptor.Properties.Any());
      }

      [TestMethod]
      public void Properties_SimpleProperty_ReturnsCollection() {
         var descriptor = new TestDescriptor() {
            SimpleProperty = new VMProperty<string>()
         };

         CollectionAssert.AreEquivalent(
            new VMPropertyBase[] { descriptor.SimpleProperty },
            descriptor.Properties.ToArray()
         );
      }

      [TestMethod]
      public void GetService_TypeDescriptorService_ReturnsSameInstanceTwice() {
         var descriptor = new EmptyDescriptor();
         var first = descriptor.GetService<TypeDescriptorViewModelBehavior>();
         var second = descriptor.GetService<TypeDescriptorViewModelBehavior>();

         Assert.IsNotNull(first);
         Assert.AreSame(first, second);
      }

      [TestMethod]
      public void GetService_ViewModelValidatorHolder_ReturnsSameInstanceTwice() {
         var descriptor = new EmptyDescriptor();
         var first = descriptor.GetService<ViewModelValidatorHolder>();
         var second = descriptor.GetService<ViewModelValidatorHolder>();

         Assert.IsNotNull(first);
         Assert.AreSame(first, second);
      }

      [TestMethod]
      public void GetService_FieldDefinitionCollection_ReturnsSameInstanceTwice() {
         var descriptor = new EmptyDescriptor();
         var first = descriptor.GetService<FieldDefinitionCollection>();
         var second = descriptor.GetService<FieldDefinitionCollection>();

         Assert.IsNotNull(first);
         Assert.AreSame(first, second);
      }

      [TestMethod]
      public void InitializePropertyNames_SimpleProperty_PropertyNameIsAssigned() {
         var descriptor = new TestDescriptor() {
            SimpleProperty = new VMProperty<string>()
         };

         descriptor.InitializePropertyNames();

         Assert.AreEqual("SimpleProperty", descriptor.SimpleProperty.PropertyName);
      }


      private class EmptyDescriptor : VMDescriptor {
      }

      private class TestDescriptor : VMDescriptor {
         public VMProperty<string> SimpleProperty { get; set; }
      }
   }
}
