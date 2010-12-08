namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class SimpleValidationTests : TestBase {
      private const string ErrorMessage = "Test";

      public ValidatingTaskVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new ValidatingTaskVM();
         VM.InitializeFrom(new Task());
      }

      [TestMethod]
      public void GetValidationState_OnInitiallyInvalidVM_ReturnsError() {
         AssertTitleInvalid();
      }

      [TestMethod]
      public void GetValidationState_AfterPropertyBecameInvalid_ReturnsError() {
         VM.Title = ArbitraryString;
         AssertTitleValid();
      }

      [TestMethod]
      public void GetValidationState_AfterPropertyBecameValid_ReturnsSuccess() {
         VM.Title = ArbitraryString;
         VM.Title = String.Empty;
         AssertTitleInvalid();
      }

      private void AssertTitleInvalid() {
         Assert.AreEqual(1, VM.TitleValidateState, "Validation state of 'Title' should contain exactly one validation error.");
         Assert.AreEqual(ErrorMessage, VM.TitleValidateState.Errors.Single(), "The single validation error of 'Title' does not contain the expected error.");
      }

      private void AssertTitleValid() {
         Assert.IsTrue(VM.TitleValidateState.IsValid, "Validation state of 'Title' should be valid.");
      }

      public class ValidatingTaskVM : TaskVM {
         public static new TaskVMDescriptor Descriptor = VMDescriptorBuilder
            .For<ValidatingTaskVM>()
            .CreateDescriptor(c => {
               var vm = c.GetPropertyFactory();
               var t = c.GetPropertyFactory(x => x.SourceTask);

               return new TaskVMDescriptor {
                  Title = t.Mapped(x => x.Title).Property(),
                  Description = vm.Local().Property<string>(),
                  ScreenTitle = vm.Local().Property<string>()
               };
            })
            .WithValidations((d, c) => {
               c.Check(d.Title).Custom((task, value, args) => {
                  if (String.IsNullOrEmpty(value)) {
                     args.Errors.Add(new ValidationError(ErrorMessage));
                  }
               });
            })
            .Build();

         public ValidatingTaskVM()
            : base(Descriptor) {
            Revalidate(ValidationScope.SelfOnly, ValidationMode.DiscardInvalidValues);
         }

         public ValidationState TitleValidateState {
            get { return Kernel.GetValidationState(Descriptor.Title); }
         }
      }
   }
}