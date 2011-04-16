namespace Inspiring.MvvmTest.ViewModels.Core.FluentDescriptorBuilder {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class VMDescriptorBuilderTests {
      [TestMethod]
      public void Build_AssignsPropertyNames() {
         var descriptor = VMDescriptorBuilder
            .OfType<TestDescriptor>()
            .For<TestVM>()
            .WithProperties((d, b) => {
               d.TestProperty = b
                  .GetPropertyBuilder()
                  .Property
                  .Of<object>();
            })
            .Build();

         Assert.IsNotNull(descriptor.TestProperty);
         Assert.AreEqual("TestProperty", descriptor.TestProperty.PropertyName);
      }

      [TestMethod]
      public void Build_InsertsProperViewModelBehaviors() {
         TestDescriptor d = VMDescriptorBuilder
            .OfType<TestDescriptor>()
            .For<TestVM>()
            .WithProperties((_, __) => { })
            .Build();

         Assert.IsNotNull(d);

         Assert.IsTrue(ContainsBehavior<LoadOrderBehavior>(d));
         Assert.IsTrue(ContainsBehavior<TypeDescriptorProviderBehavior>(d));
      }

      private bool ContainsBehavior<T>(VMDescriptor descriptor) where T : IBehavior {
         T behavior;
         return descriptor.Behaviors.TryGetBehavior<T>(out behavior);
      }

      private class TestVM : ViewModel<TestDescriptor> { }

      private class TestDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<object> TestProperty { get; set; }
      }
   }
}