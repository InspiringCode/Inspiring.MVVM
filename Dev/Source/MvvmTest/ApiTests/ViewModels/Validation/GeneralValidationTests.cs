﻿namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class GeneralValidationTests : ValidationTestBase {
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
         var expected = CreateValidationState(ErrorMessage);
         DomainAssert.AreEqual(expected, VM.TitleValidationState);
      }

      private void AssertTitleValid() {
         DomainAssert.AreEqual(ValidationResult.Valid, VM.TitleValidationState);
      }

      public class TaskVM : ViewModel<TaskVMDescriptor> {
         public static TaskVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<TaskVMDescriptor>()
            .For<TaskVM>()
            .WithProperties((d, c) => {
               var vm = c.GetPropertyBuilder();

               d.Title = vm.Property.Of<string>();
            })
            .WithValidators(c => {
               c.Check(x => x.Title).Custom((task, value, args) => {
                  if (String.IsNullOrEmpty(value)) {
                     args.AddError(ErrorMessage);
                  }
               });
            })
            .Build();

         public TaskVM()
            : base(ClassDescriptor) {
         }

         public ValidationResult TitleValidationState {
            get { return Kernel.GetValidationState(Descriptor.Title); }
         }

         public string Title {
            get { return GetValue(Descriptor.Title); }
            set { SetValue(Descriptor.Title, value); }
         }
      }

      public class TaskVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Title { get; set; }
      }
   }
}