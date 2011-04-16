namespace Inspiring.MvvmTest.ViewModels.Core.Validation.Validators {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ViewModelValidationArgsTests : ValidationArgsFixture {
      [TestMethod]
      public void Create_PathWithTwoViewModel_SetsPropertiesCorrectly() {
         var owner = Mock<IViewModel>();
         var target = new EmployeeVM();

         var args = CreateArgs(owner, target);

         Assert.AreEqual(owner, args.Owner);
         Assert.AreEqual(target, args.Target);
      }

      [TestMethod]
      public void Create_PathWithSingleViewModel_SetsOwnerAndTargetToSameViewModel() {
         var ownerAndTarget = new EmployeeVM();

         var args = CreateArgs(ownerAndTarget, ownerAndTarget);

         Assert.AreEqual(ownerAndTarget, args.Owner);
         Assert.AreEqual(ownerAndTarget, args.Target);
      }

      [TestMethod]
      public void AddError_CreatesAndAddsCorrectError() {
         var target = new EmployeeVM();

         var message = "Test error message";
         var details = new Object();

         var args = CreateArgs(target, target);
         args.AddError(message, details);

         var expectedError = new ValidationError(args.Validator, args.Target, message, details);
         AssertErrors(args, expectedError);
      }

      private static ViewModelValidationArgs<IViewModel, EmployeeVM> CreateArgs(
         IViewModel owner,
         EmployeeVM target
      ) {
         var path = Path.Empty.Append(owner);

         if (target != owner) {
            path = path.Append(target);
         }

         return ViewModelValidationArgs<IViewModel, EmployeeVM>.Create(
            Mock<IValidator>(),
            CreateRequest(path)
         );
      }
   }
}