namespace Inspiring.MvvmTest.ViewModels.Core.Validation.Validators {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Core.Validation.Validators;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class PropertyValidationArgsTests : ValidationArgsFixture {
      [TestMethod]
      public void Create_RequestWithTwoViewModelsAndProperty_SetsPropertiesCorrectly() {
         var employeeName = "Test name";
         var owner = Mock<IViewModel>();
         var target = new EmployeeVM(employeeName);

         var args = CreateArgs(owner, target);

         Assert.AreEqual(owner, args.Owner);
         Assert.AreEqual(target, args.Target);
         Assert.AreEqual(EmployeeVM.ClassDescriptor.Name, args.TargetProperty);
         Assert.AreEqual(employeeName, args.Value);
      }

      [TestMethod]
      public void Create_RequestWithSingleViewModelAndProperty_SetsOwnerAndTargetToSameViewModel() {
         var targetAndOwner = new EmployeeVM();

         var args = CreateArgs(owner: targetAndOwner, target: targetAndOwner);

         Assert.AreEqual(targetAndOwner, args.Owner);
         Assert.AreEqual(targetAndOwner, args.Target);
      }

      [TestMethod]
      public void Create_DoesNotAccessTargetProperty() {
         var target = new EmployeeVM();
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

         var message = "Test error message";
         var details = new Object();

         var args = CreateArgs(target, target);
         args.AddError(message, details);

         var expectedError = new ValidationError(args.Validator, args.Target, args.TargetProperty, message, details);
         AssertErrors(args, expectedError);
      }

      private static PropertyValidationArgs<IViewModel, EmployeeVM, string> CreateArgs(
         IViewModel owner,
         EmployeeVM target
      ) {
         var path = Path.Empty.Append(owner);

         if (target != owner) {
            path = path.Append(target);
         }

         path = path.Append(EmployeeVM.ClassDescriptor.Name);

         return PropertyValidationArgs<IViewModel, EmployeeVM, string>.Create(
            Mock<IValidator>(),
            CreateRequest(path)
         );
      }
   }
}