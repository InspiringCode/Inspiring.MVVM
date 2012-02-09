namespace Inspiring.MvvmTest.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ViewModelTests {
      private ViewModelMock VM { get; set; }

      private IViewModel VMInterface {
         get { return VM; }
      }

      [TestInitialize]
      public void Setup() {
         VM = new ViewModelMock();
      }

      [TestMethod]
      public void NotifyChange_WithPropertyChangeForOwnProperty_CallsOnPropertyChanged() {
         var property = PropertyStub.Build();

         VMInterface.NotifyChange(ChangeArgs
            .PropertyChanged(property, ValueStage.DisplayValue)
            .PrependViewModel(VM)
         );

         Assert.AreEqual(property, VM.LastOnPropertyChangedInvocation);
      }

      [TestMethod]
      public void NotifyChange_WithValidationResultChangeForOwnProperty_CallsValidationResultChanged() {
         var property = PropertyStub.Build();

         VMInterface.NotifyChange(ChangeArgs
            .ValidationResultChanged(property, ValueStage.Value)
            .PrependViewModel(VM)
         );

         Assert.AreEqual(property, VM.LastOnValidationStateChangedInvocation);
      }

      [TestMethod]
      public void NotifyChange_WithValidationResultChangeForOwnViewModel_CallsValidationResultChanged() {
         VMInterface.NotifyChange(ChangeArgs
            .ValidationResultChanged()
            .PrependViewModel(VM)
         );

         Assert.IsTrue(VM.OnValidationStateChangedWasCalled);
         Assert.IsNull(VM.LastOnValidationStateChangedInvocation);
      }

      [TestMethod]
      public void NotifyChange_WithPropertyChangeForDescendantProperty_DoesNothing() {
         VMInterface.NotifyChange(ChangeArgs
            .PropertyChanged(PropertyStub.Build(), ValueStage.ValidatedValue)
            .PrependViewModel(ViewModelStub.Build())
            .PrependViewModel(VM)
         );

         Assert.IsNull(VM.LastOnPropertyChangedInvocation);
         Assert.IsNull(VM.LastOnValidationStateChangedInvocation);
      }

      [TestMethod]
      public void NotifyChange_WithValidationResultChangeForDescendantProperty_DoesNothing() {
         VMInterface.NotifyChange(ChangeArgs
            .ValidationResultChanged(PropertyStub.Build(), ValueStage.Value)
            .PrependViewModel(ViewModelStub.Build())
            .PrependViewModel(VM)
         );

         Assert.IsNull(VM.LastOnPropertyChangedInvocation);
         Assert.IsNull(VM.LastOnValidationStateChangedInvocation);
      }

      [TestMethod]
      public void NotifyChange_WithPropertyChangeForDescendantViewModel_DoesNothing() {
         VMInterface.NotifyChange(ChangeArgs
            .ValidationResultChanged()
            .PrependViewModel(ViewModelStub.Build())
            .PrependViewModel(VM)
         );

         Assert.IsNull(VM.LastOnPropertyChangedInvocation);
         Assert.IsNull(VM.LastOnValidationStateChangedInvocation);
      }

      [TestMethod]
      public void NotifyChange_ForOwnProperty_RaisesPropertyChangedForErrorProperty() {
         var counter = new PropertyChangedCounter(VM, "Error");

         VMInterface.NotifyChange(ChangeArgs
            .ValidationResultChanged()
            .PrependViewModel(VM)
         );

         counter.AssertOneRaise();
      }

      [TestMethod]
      public void NotifyChange_WithValidationResultChangeForOwnProperty_RaisesPropertyChangedForIsValidProperty() {
         var counter = new PropertyChangedCounter(VM, "IsValid");

         VMInterface.NotifyChange(ChangeArgs
               .ValidationResultChanged()
               .PrependViewModel(VM)
            );

         counter.AssertOneRaise();
      }

      private class ViewModelMock : ViewModel<IVMDescriptor> {
         public IVMPropertyDescriptor LastOnPropertyChangedInvocation { get; set; }
         public IVMPropertyDescriptor LastOnValidationStateChangedInvocation { get; set; }
         public bool OnValidationStateChangedWasCalled { get; set; }

         protected override void OnPropertyChanged(IVMPropertyDescriptor property) {
            base.OnPropertyChanged(property);
            LastOnPropertyChangedInvocation = property;
         }

         protected override void OnValidationResultChanged(IVMPropertyDescriptor property) {
            base.OnValidationResultChanged(property);
            OnValidationStateChangedWasCalled = true;
            LastOnValidationStateChangedInvocation = property;
         }
      }
   }
}