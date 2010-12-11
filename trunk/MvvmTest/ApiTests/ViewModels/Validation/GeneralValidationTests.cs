namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class GeneralValidationTests : TestBase {
      private const string ErrorMessage = "Test";

      public TaskVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new TaskVM();
      }

      [TestMethod]
      public void GetValidationState_AfterPropertyBecameValid_ReturnsSucess() {
         VM.Title = ArbitraryString;
         AssertTitleValid();
      }

      [TestMethod]
      public void GetValidationState_AfterPropertyBecameInvalid_ReturnsError() {
         VM.Title = ArbitraryString;
         VM.Title = String.Empty;
         AssertTitleInvalid();
      }

      private void AssertTitleInvalid() {
         var expected = new ValidationState();
         expected.Errors.Add(new ValidationError(ErrorMessage));

         DomainAssert.AreEqual(expected, VM.TitleValidationState);
      }

      private void AssertTitleValid() {
         DomainAssert.AreEqual(ValidationState.Valid, VM.TitleValidationState);
      }

      public class TaskVM : ViewModel<TaskVMDescriptor> {
         public static TaskVMDescriptor Descriptor = VMDescriptorBuilder
            .For<TaskVM>()
            .CreateDescriptor(c => {
               var vm = c.GetPropertyFactory();

               return new TaskVMDescriptor {
                  Title = vm.Local().Property<string>()
               };
            })
            .WithValidations((d, c) => {
               c.Check(x => x.Title).Custom((task, value, args) => {
                  if (String.IsNullOrEmpty(value)) {
                     args.Errors.Add(new ValidationError(ErrorMessage));
                  }
               });
            })
            .Build();

         public TaskVM()
            : base(Descriptor) {
         }

         public ValidationState TitleValidationState {
            get { return Kernel.GetValidationState(Descriptor.Title); }
         }

         public string Title {
            get { return GetValue(Descriptor.Title); }
            set { SetValue(Descriptor.Title, value); }
         }
      }

      public class TaskVMDescriptor : VMDescriptor {
         public VMProperty<string> Title { get; set; }
      }
   }
}