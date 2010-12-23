namespace Inspiring.MvvmTest.ViewModels.Core.Builder {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using System;

   [TestClass]
   public class BehaviorsTest {
      [TestMethod]
      public void CustomBehaviorIntegrationTest() {
         var descriptor = VMDescriptorBuilder
            .OfType<TestVMDescriptor>()
            .For<TesTVM>()
            .WithProperties((d, c) => {
               var f = c.GetPropertyBuilder();
               d.Property = f.Property.DelegatesTo(x => "");
            })
            .WithBehaviors(c => {
               throw new NotImplementedException();
               //c.Custom(d.Property, VMBehaviorKey.PropertyValueCache);
            })
            .Build();

         RefreshableValueCacheBehavior<string> b;
         Assert.IsTrue(descriptor.Property.Behaviors.TryGetBehavior(out b));
      }

      [TestMethod] // Maybe delete
      public void CustomBehaviorIntegrationTest_Disconnect() {
         var descriptor = VMDescriptorBuilder
            .OfType<TestVMDescriptor>()
            .For<TesTVM>()
            .WithProperties((d, c) => {
               var f = c.GetPropertyBuilder();

               d.Property = f.Property.DelegatesTo(x => "");
            })
            .WithBehaviors(c => {
               throw new NotImplementedException();
               //c.Disconnect(d.Property);
            })
            .Build();

         RefreshableValueCacheBehavior<string> b;
         Assert.IsTrue(descriptor.Property.Behaviors.TryGetBehavior(out b));
      }

      private class TesTVM : ViewModel<TestVMDescriptor> { }

      private class TestVMDescriptor : VMDescriptor {
         public VMProperty<string> Property { get; set; }
      }
   }
}