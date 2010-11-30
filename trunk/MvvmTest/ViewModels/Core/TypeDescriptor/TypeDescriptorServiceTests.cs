namespace Inspiring.MvvmTest.ViewModels.Core.TypeDescriptor {
   using System.ComponentModel;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class TypeDescriptorServiceTests {
      [TestMethod]
      public void PropertyDescriptors_ReturnsVMDescriptorProperties() {
         // Arrange
         string propertyName = "Test";

         VMDescriptorStub descriptor = new VMDescriptorStub();
         descriptor.AddProperty(propertyName, typeof(string));

         // Act
         TypeDescriptorBehavior svc = new TypeDescriptorBehavior();

         PropertyDescriptor returnedDescriptor = svc
            .PropertyDescriptors
            .Cast<PropertyDescriptor>()
            .Single();

         // Assert
         Assert.AreEqual(propertyName, returnedDescriptor.Name);
      }

      [TestMethod]
      public void PropertyDescriptors_GetTwice_ReturnsSameCollection() {
         // Arrange
         string propertyName = "Test";

         VMDescriptorStub descriptor = new VMDescriptorStub();
         descriptor.AddProperty(propertyName, typeof(string));

         TypeDescriptorBehavior svc = new TypeDescriptorBehavior();

         // Act
         var first = svc.PropertyDescriptors;
         var second = svc.PropertyDescriptors;

         Assert.AreSame(first, second);
      }
   }
}
