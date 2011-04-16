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
      public void NotifyChange_ForOwnProperty_CallsOnPropertyChanged() {
         var property = PropertyStub.Build();

         VMInterface.NotifyChange(ChangeArgs
            .PropertyChanged(property)
            .PrependViewModel(VM)
         );

         Assert.AreEqual(property, VM.LastOnPropertyChangedInvocation);
      }

      [TestMethod]
      public void NotifyChange_ForOwnViewModel_CallsValidationStateChanged() {
         var property = PropertyStub.Build();

         VMInterface.NotifyChange(ChangeArgs
            .ValidationStateChanged(property)
            .PrependViewModel(VM)
         );

         Assert.AreEqual(property, VM.LastOnValidationStateChangedInvocation);
      }

      [TestMethod]
      public void NotifyChange_ForOwnProperty_RaisesPropertyChangedForErrorProperty() {
         var counter = new PropertyChangedCounter(VM, "Error");

         VMInterface.NotifyChange(ChangeArgs
            .ValidationStateChanged()
            .PrependViewModel(VM)
         );

         counter.AssertOneRaise();
      }

      private class ViewModelMock : ViewModel<VMDescriptorBase> {
         public IVMPropertyDescriptor LastOnPropertyChangedInvocation { get; set; }
         public IVMPropertyDescriptor LastOnValidationStateChangedInvocation { get; set; }

         protected override void OnPropertyChanged(IVMPropertyDescriptor property) {
            base.OnPropertyChanged(property);
            LastOnPropertyChangedInvocation = property;
         }

         protected override void OnValidationStateChanged(IVMPropertyDescriptor property) {
            base.OnValidationStateChanged(property);
            LastOnValidationStateChangedInvocation = property;
         }
      }
   }
}