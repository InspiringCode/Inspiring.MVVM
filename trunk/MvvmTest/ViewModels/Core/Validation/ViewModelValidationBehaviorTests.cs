namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ViewModelValidationBehaviorTests {
      [TestMethod]
      public void OnChanged_RootValidator_GetsCalled() {
         var behavior = new ViewModelValidationBehavior();
         var validator = new ValidatorMock();
         var employeeVM = new Mock<IViewModel>().Object;

         behavior.AddValidation(VMPropertyPath.Empty, validator);

         behavior.OnChanged(
            new ViewModelBehaviorContextStub(employeeVM),
            new ChangeArgs(ChangeType.PropertyChanged, employeeVM),
            new InstancePath(employeeVM)
         );

         Assert.AreEqual(1, validator.InvocationCount);
         Assert.AreSame(employeeVM, validator.Args.ChangedVM);
         Assert.AreSame(employeeVM, validator.Args.ValidationTarget);
         Assert.IsTrue(validator.ChangedVMPath.IsEmpty);
      }

      [TestMethod]
      public void OnChanged_ChildValidator_GetsNotCalled() {
         var behavior = new ViewModelValidationBehavior();
         var validator = new ValidatorMock();
         var employeeVM = new Mock<IViewModel>();
         var addressProperty = new Mock<IVMProperty>().Object;

         behavior.AddValidation(new VMPropertyPath(addressProperty), validator);

         behavior.OnChanged(
            new ViewModelBehaviorContextStub(employeeVM.Object),
            new ChangeArgs(ChangeType.PropertyChanged, employeeVM.Object),
            new InstancePath(employeeVM.Object)
         );

         Assert.AreEqual(0, validator.InvocationCount);
      }

      [TestMethod]
      public void OnChildChanged_RootValidator_GetsCalled() {
         var behavior = new ViewModelValidationBehavior();
         var validator = new ValidatorMock();
         var employeeVM = new Mock<IViewModel>().Object;
         var addressVM = new Mock<IViewModel>().Object;

         behavior.AddValidation(VMPropertyPath.Empty, validator);

         behavior.OnChanged(
            new ViewModelBehaviorContextStub(employeeVM),
            new ChangeArgs(ChangeType.PropertyChanged, addressVM),
            new InstancePath(employeeVM, addressVM)
         );

         Assert.AreEqual(1, validator.InvocationCount);
         Assert.AreSame(addressVM, validator.Args.ChangedVM);
         Assert.AreSame(employeeVM, validator.Args.ValidationTarget);

         AssertHelper.AreEqual(CreateSteps(addressVM), validator.ChangedVMPath.Steps, StepsAreEqual);
      }

      [TestMethod]
      public void OnChildChanged_ChildValidatorWithMatchingPath_GetsCalled() {
         var behavior = new ViewModelValidationBehavior();
         var validator = new ValidatorMock();
         var employeeVM = new Mock<IViewModel>();
         var addressVM = new Mock<IViewModel>();
         var addressProperty = new Mock<IVMProperty>().Object;

         employeeVM.Setup(x => x.GetValue(addressProperty)).Returns(addressVM.Object);

         behavior.AddValidation(new VMPropertyPath(addressProperty), validator);

         behavior.OnChanged(
            new ViewModelBehaviorContextStub(employeeVM.Object),
            new ChangeArgs(ChangeType.PropertyChanged, addressVM.Object),
            new InstancePath(employeeVM.Object, addressVM.Object)
         );

         Assert.AreEqual(1, validator.InvocationCount);
         Assert.AreSame(addressVM.Object, validator.Args.ChangedVM);
         Assert.AreSame(addressVM.Object, validator.Args.ValidationTarget);
         Assert.IsTrue(validator.ChangedVMPath.IsEmpty);
      }

      [TestMethod]
      public void OnChildChanged_ChildValidatorWithNonMatchingPath_GetsNotCalled() {

      }

      private static InstancePathStep[] CreateSteps(params IViewModel[] steps) {
         return steps.Select(x => new InstancePathStep(x)).ToArray();
      }

      private static bool StepsAreEqual(InstancePathStep x, InstancePathStep y) {
         if (Object.ReferenceEquals(x, y)) {
            return true;
         }

         return
            x != null &&
            y != null &&
            Object.ReferenceEquals(x.VM, y.VM) &&
            Object.ReferenceEquals(x.ParentCollection, y.ParentCollection);
      }

      private class ValidatorMock : ViewModelValidator {
         public int InvocationCount { get; private set; }
         public ViewModelValidationArgs Args { get; private set; }
         public InstancePath ChangedVMPath { get; private set; }


         public override void Validate(ViewModelValidationArgs args) {
            throw new NotImplementedException();
         }
         //public override void Validate(ViewModelValidationArgs args) {
         //   throw new NotImplementedException();
         //}
      }
   }
}