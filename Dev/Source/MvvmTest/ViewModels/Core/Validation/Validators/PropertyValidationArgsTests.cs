namespace Inspiring.MvvmTest.ViewModels.Core.Validation.Validators {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class PropertyValidationArgsTests : ValidationArgsFixture {
      [TestMethod]
      public void Create_RequestWithTwoViewModelsAndProperty_SetsPropertiesCorrectly() {
         var employeeName = "Test name";
         var owner = Mock<IViewModel>();
         var target = new EmployeeVM(employeeName);
         var validator = Mock<IValidator>();

         var args = CreateArgs(owner, target);

         Assert.AreEqual(owner, args.Owner);
         Assert.AreEqual(target, args.Target);
         Assert.AreEqual(EmployeeVM.ClassDescriptor.Name, args.TargetProperty);
         Assert.AreEqual(employeeName, args.Value);
      }

      [TestMethod]
      public void Create_RequestWithSingleViewModelAndProperty_SetsOwnerAndTargetToSameViewModel() {
         var targetAndOwner = new EmployeeVM();
         var validator = Mock<IValidator>();

         var args = CreateArgs(owner: targetAndOwner, target: targetAndOwner);

         Assert.AreEqual(targetAndOwner, args.Owner);
         Assert.AreEqual(targetAndOwner, args.Target);
      }

      [TestMethod]
      public void Create_DoesNotAccessTargetProperty() {
         var target = new EmployeeVM();
         var validator = Mock<IValidator>();

         var args = CreateArgs(target, target);

         Assert.AreEqual(
            0,
            target.NamePropertyAccesses,
            "The creation of args should not access the target property."
         );
      }

      [TestMethod]
      public void AddError_CreatesAndAddsCorrectError() {
         var target = new EmployeeVM();
         var validator = Mock<IValidator>();
         var step = ValidationStep.Value;

         var message = "Test error message";
         var details = new Object();

         var args = CreateArgs(target, target, step, validator);
         args.AddError(message, details);

         var expectedError = new ValidationError(
            validator,
            ValidationTarget.ForError(
               step,
               target,
               null,
               EmployeeVM.ClassDescriptor.Name
            ),
            message, 
            details
         );

         ValidationAssert.HasErrors(args.Result, ValidationAssert.FullErrorComparer, expectedError);
      }

      private static PropertyValidationArgs<IViewModel, EmployeeVM, string> CreateArgs(
         IViewModel owner,
         EmployeeVM target,
         ValidationStep step = ValidationStep.Value,
         IValidator validator = null         
      ) {
         validator = validator ?? Mock<IValidator>();

         var path = Path.Empty.Append(owner);

         if (target != owner) {
            path = path.Append(target);
         }

         path = path.Append(EmployeeVM.ClassDescriptor.Name);

         return PropertyValidationArgs<IViewModel, EmployeeVM, string>.Create(
            validator,
            new ValidationRequest(step, path)
         );
      }
   }
}