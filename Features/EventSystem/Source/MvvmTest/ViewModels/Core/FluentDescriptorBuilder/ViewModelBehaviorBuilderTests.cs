namespace Inspiring.MvvmTest.ViewModels.Core.FluentDescriptorBuilder {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ViewModelBehaviorBuilderTests {
      [TestMethod]
      public void EnableUndo_InsertsUndoSetValueBehavior() {
         var descriptor = VMDescriptorBuilder
            .OfType<TestDescriptor>()
            .For<TestVM>()
            .WithProperties((d, b) => {
               d.TestProperty = b
                  .GetPropertyBuilder()
                  .Property
                  .Of<object>();
            })
            .WithViewModelBehaviors(b => {
               b.EnableUndo();
            })
            .Build();

         Assert.IsNotNull(descriptor.TestProperty);
         Assert.IsTrue(ContainsBehavior<UndoSetValueBehavior<object>>(descriptor.TestProperty));
      }

      private bool ContainsBehavior<T>(IVMPropertyDescriptor property) where T : IBehavior {
         T behavior;
         return property.Behaviors.TryGetBehavior<T>(out behavior);
      }

      private class TestDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<object> TestProperty { get; set; }
      }
   }
}
