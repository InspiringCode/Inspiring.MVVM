namespace Inspiring.MvvmTest.ViewModels.Core.Validation.Validators {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using ValidationArgs = Inspiring.Mvvm.ViewModels.Core.ValidationArgs; // TODO: Remove...

   [TestClass]
   public class DelegateValidatorTests : TestBase {
      [TestMethod]
      public void Execute_PassesRequestAndSelfToArgsFactory() {
         ValidationRequest actualRequest = null;
         IValidator actualValidator = null;

         Func<IValidator, ValidationRequest, TestArgs> argsFactory = (v, r) => {
            actualRequest = r;
            actualValidator = v;
            return new TestArgs();
         };

         var request = CreateAnyRequest();
         var validator = new DelegateValidator<TestArgs>(argsFactory, a => { });

         validator.Execute(request);

         Assert.AreEqual(request, actualRequest);
         Assert.AreEqual(validator, actualValidator);
      }

      [TestMethod]
      public void Validate_CallsDelegateWithArgsReturnedByFactory() {
         TestArgs args = new TestArgs();
         TestArgs actualArgs = null;

         Func<IValidator, ValidationRequest, TestArgs> argsFactory = (v, r) => {
            return args;
         };

         Action<TestArgs> validatorAction = a => {
            actualArgs = a;
         };

         var validator = new DelegateValidator<TestArgs>(argsFactory, validatorAction);
         validator.Execute(CreateAnyRequest());

         Assert.AreEqual(args, actualArgs);
      }

      private ValidationRequest CreateAnyRequest() {
         return new ValidationRequest(ValidationStep.Value, Path.Empty);
      }

      private class TestArgs : ValidationArgs {
         public TestArgs()
            : base(Mock<IValidator>()) {
         }
      }
   }
}