namespace Inspiring.MvvmTest.ViewModels.Core {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class VMDescriptorBuilderTests {
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

      private class TestDescriptor : VMDescriptor { }
   }
}