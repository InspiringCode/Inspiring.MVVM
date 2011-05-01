namespace Inspiring.MvvmTest.ApiTests.ViewModels.Refresh {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ViewModelPropertyRefreshTests : RefreshFixture {
      private RootVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new RootVM();
      }

      [TestMethod]
      public void Refresh_OfInstanceProperty_CallsRefreshOnChildValue() {
         var child = new ChildVM();
         VM.SetValue(x => x.InstanceProperty, child);

         VM.Refresh(x => x.InstanceProperty);
         Assert.IsTrue(child.WasRefreshed);
      }

      [TestMethod]
      public void Refresh_OfWrapperProperty_SetsSourceOnChildAndRefreshesChild() {
         VM.WrapperPropertySource = new ChildSource();
         var childVM = VM.GetValue(x => x.WrapperProperty);

         var newSource = new ChildSource();
         VM.WrapperPropertySource = newSource;

         VM.Refresh(x => x.WrapperProperty);
         Assert.AreEqual(newSource, childVM.Source);
         Assert.IsTrue(childVM.WasRefreshed);
      }

      [TestMethod]
      public void Refresh_OfDelegateProperty_ResetsPropertyValueByCallingGetterDelegate() {
         var previousChildVM = VM.GetValue(x => x.DelegateProperty);
         var newChildVM = new ChildVM();
         VM.DelegatePropertyResult = newChildVM;

         VM.Refresh(x => x.DelegateProperty);
         Assert.AreEqual(newChildVM, VM.GetValue(x => x.DelegateProperty));
      }

      [TestMethod]
      public void Refresh_OfDelegateProperty_CallsNotifyChangeWhenViewModelInstanceHasChanged() {
         VM.SetValue(x => x.DelegateProperty, new ChildVM());
         VM.DelegatePropertyResult = new ChildVM();
         VM.NotifyChangeInvocations.Clear();

         VM.Refresh(x => x.DelegateProperty);

         var expectedArgs = ChangeArgs
            .PropertyChanged(RootVM.ClassDescriptor.DelegateProperty)
            .PrependViewModel(VM);

         DomainAssert.AreEqual(
            new[] { expectedArgs },
            VM.NotifyChangeInvocations
         );
      }

      [TestMethod]
      public void Refresh_OfDelegateProperty_DoesNotCallNotifyChangeIfViewModelInstanceHasNotChanged() {
         VM.SetValue(x => x.DelegateProperty, new ChildVM());
         VM.NotifyChangeInvocations.Clear();

         VM.Refresh(x => x.DelegateProperty);

         DomainAssert.AreEqual(new ChangeArgs[0], VM.NotifyChangeInvocations);
      }

      [TestMethod]
      public void Refresh_OfDelegateProperty_DoesNotRefreshReturnedChildVM() {
         var childVM = new ChildVM();
         VM.SetValue(x => x.DelegateProperty, childVM);

         VM.Refresh(x => x.DelegateProperty);
         Assert.IsFalse(childVM.WasRefreshed);
      }

      [TestMethod]
      public void Refresh_OfUnloadedWrapperProperty_DoesNothing() {
         VM.Refresh(x => x.WrapperProperty);
         Assert.IsFalse(VM.IsLoaded(x => x.WrapperProperty));
      }

      [TestMethod]
      public void Refresh_OfUnloadedDelegateProperty_DoesNothing() {
         VM.Refresh(x => x.DelegateProperty);
         Assert.IsFalse(VM.IsLoaded(x => x.DelegateProperty));
      }

      private sealed class RootVM : TestViewModel<RootVMDescriptor> {
         public static readonly RootVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<RootVMDescriptor>()
            .For<RootVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.InstanceProperty = v.VM.Of<ChildVM>();
               d.WrapperProperty = v.VM.Wraps(x => x.WrapperPropertySource).With<ChildVM>();
               d.DelegateProperty = v.VM.DelegatesTo(
                  x => x.DelegatePropertyResult,
                  (x, val) => x.DelegatePropertyResult = val
               );
            })
            .Build();

         public RootVM()
            : base(ClassDescriptor) {
         }

         public ChildSource WrapperPropertySource { get; set; }
         public ChildVM DelegatePropertyResult { get; set; }
      }

      private sealed class RootVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ChildVM> InstanceProperty { get; set; }
         public IVMPropertyDescriptor<ChildVM> WrapperProperty { get; set; }
         public IVMPropertyDescriptor<ChildVM> DelegateProperty { get; set; }
      }
   }
}