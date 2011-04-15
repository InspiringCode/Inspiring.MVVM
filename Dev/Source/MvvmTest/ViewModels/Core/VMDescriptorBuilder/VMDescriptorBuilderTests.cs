namespace Inspiring.MvvmTest.ViewModels.Core {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

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
         Assert.IsTrue(ContainsBehavior<TypeDescriptorBehavior>(d));
      }

      private bool ContainsBehavior<T>(VMDescriptor descriptor) where T : IBehavior {
         T behavior;
         return descriptor.Behaviors.TryGetBehavior<T>(out behavior);
      }

      private class TestVM : ViewModel<TestDescriptor> { }

      private class TestDescriptor : VMDescriptor { }
   }
}