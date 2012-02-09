namespace Inspiring.MvvmTest.ViewModels.Core.Properties.ViewModelProperty {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class WrapperViewModelAccessorBehaviorTests : ViewModelPropertyAccessorFixture {
      private ValueAccessorStub<ChildSource> SourceAccessor { get; set; }
      private WrapperViewModelAccessorBehavior<ChildVM, ChildSource> Behavior { get; set; }
      private BehaviorContextStub Context { get; set; }

      [TestInitialize]
      public void Setup() {
         SourceAccessor = new ValueAccessorStub<ChildSource>();
         Behavior = new WrapperViewModelAccessorBehavior<ChildVM, ChildSource>();

         Context = PropertyStub
            .WithBehaviors(Behavior, SourceAccessor, new ServiceLocatorValueFactoryBehavior<ChildVM>())
            .GetContext();
      }

      [TestMethod]
      public void SetValue_ToNull_SetsSourceValueToNull() {
         SourceAccessor.Value = new ChildSource();
         Behavior.SetValue(Context, null);
         Assert.IsNull(SourceAccessor.Value);
      }

      [TestMethod]
      public void Refresh_DoesNotRaiseNotifyChange() {
         SourceAccessor.Value = new ChildSource();
         Behavior.GetValue(Context);
         Behavior.Refresh(Context, new RefreshOptions());
         Assert.IsFalse(Context.NotifyChangeInvocations.Any());
      }
   }
}