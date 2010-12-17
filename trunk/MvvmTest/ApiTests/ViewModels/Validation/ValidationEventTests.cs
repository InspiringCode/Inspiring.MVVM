﻿namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationEventTests {
      private TaskVM VM { get; set; }
      private PropertyChangedCounter ValidationStateChangedEvent { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new TaskVM();
         VM.ChangeValidationStateToValid();

         ValidationStateChangedEvent = new PropertyChangedCounter(VM, "Item[]");
      }

      [TestMethod]
      public void ValidationStateChange_FromValidToInvalid_CallsValidationStateChanged() {
         VM.ChangeValidationStateToInvalid();
         ValidationStateChangedEvent.AssertOneRaise();
      }

      [TestMethod]
      public void ValidationStateChange_FromInvalidToValid_CallsValidationStateChanged() {
         VM.ChangeValidationStateToInvalid();
         ValidationStateChangedEvent.Reset();

         VM.ChangeValidationStateToValid();
         ValidationStateChangedEvent.AssertOneRaise();
      }

      [TestMethod]
      public void ValidationStateChange_FromValidToValid_DoesNotCallValidationStateChanged() {
         VM.ChangeValidationStateToValid();
         ValidationStateChangedEvent.AssertNoRaise();
      }

      [TestMethod]
      public void ValidationStateChange_FromInvalidToInvalid_DoesNotCallValidationStateChanged() {
         VM.ChangeValidationStateToInvalid();
         ValidationStateChangedEvent.Reset();

         VM.ChangeValidationStateToInvalid();
         ValidationStateChangedEvent.AssertNoRaise();
      }

      public sealed class TaskVM : ViewModel<TaskVMDescriptor> {
         public static readonly TaskVMDescriptor Descriptor = VMDescriptorBuilder
            .For<TaskVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyBuilder();

               return new TaskVMDescriptor {
                  Title = v.Local.Property<string>()
               };
            })
            .WithValidations((d, c) => {
               c.Check(x => x.Title).Custom((vm, val, args) => {
                  if (vm.ReturnError) {
                     args.Errors.Add(new ValidationError("Validation error"));
                  }
               });
            })
            .Build();

         public TaskVM()
            : base(Descriptor) {
         }

         private bool ReturnError { get; set; }

         public void ChangeValidationStateToValid() {
            ReturnError = false;
            Revalidate();
         }

         public void ChangeValidationStateToInvalid() {
            ReturnError = true;
            Revalidate();
         }

         private void Revalidate() {
            Revalidate(ValidationScope.SelfOnly, ValidationMode.DiscardInvalidValues);
         }
      }

      public sealed class TaskVMDescriptor : VMDescriptor {
         public VMProperty<string> Title { get; set; }
      }
   }
}