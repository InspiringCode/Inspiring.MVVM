namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ViewModelPropertyValidationTests : ValidationTestBase {
      [TestMethod]
      public void SetValue_ToNewChild_RevalidatesOldAndNewChild() {
         var vm = new ParentVM();
         var oldChild = new ChildVM();
         var newChild = new ChildVM();
         vm.SetValue(x => x.Child, oldChild);

         oldChild.ChildPropertyErrorMessage = "Old validation error";
         newChild.ChildPropertyErrorMessage = "New validation error";

         vm.SetValue(x => x.Child, newChild);

         ValidationAssert.ErrorMessages(oldChild.ValidationResult, oldChild.ChildPropertyErrorMessage);
         ValidationAssert.ErrorMessages(newChild.ValidationResult, newChild.ChildPropertyErrorMessage);
      }

      [TestMethod]
      public void SetValue_RemovesValidationErrorsOfAncestorValidatorsFromOldChild() {
         string parentErrorMessage = "Parent error";
         
         var vm = new ParentVM();
         var oldChild = new ChildVM();
         var newChild = new ChildVM();
                  
         vm.ChildPropertyErrorMessage = parentErrorMessage;

         vm.SetValue(x => x.Child, oldChild);
         ValidationAssert.ErrorMessages(oldChild.ValidationResult, parentErrorMessage);

         vm.SetValue(x => x.Child, newChild);

         ValidationAssert.IsValid(oldChild);
      }

      [TestMethod] // regression test
      public void SetValue_WhenOldAndNewChildGetInvalid_ParentResultContainsOnlyErrorOfNewChild() {
         var parent = new ParentVM();

         var oldChild = new ChildVM();
         var newChild = new ChildVM();

         parent.SetValue(x => x.Child, oldChild);

         oldChild.ChildPropertyErrorMessage = "Old validation error";
         newChild.ChildPropertyErrorMessage = "New validation error";
         parent.SetValue(x => x.Child, newChild);

         ValidationAssert.ErrorMessages(
            parent.ValidationResult,
            ParentVM.ChildInvalidMessage,
            newChild.ChildPropertyErrorMessage
         );
      }

      [TestMethod] // regression test
      public void SetValue_WhenOldChildGetsInvalidAndNewChildIsValid_ParentValidationResultIsValid() {
         var parent = new ParentVM();

         var oldChild = new ChildVM();
         var newChild = new ChildVM();

         parent.SetValue(x => x.Child, oldChild);

         oldChild.ChildPropertyErrorMessage = "Old validation error";
         parent.SetValue(x => x.Child, newChild);

         ValidationAssert.IsValid(parent.ValidationResult);
      }

      [TestMethod] // regression test
      public void SetValue_WhenOldChildGetsSameErrorAsNewChild_ParentValidationResultContainsErrorOfNewChild() {
         var parent = new ParentVM();

         var oldChild = new ChildVM();
         var newChild = new ChildVM();

         parent.SetValue(x => x.Child, oldChild);

         string sameValidationError = "Same validation error";
         oldChild.ChildPropertyErrorMessage = sameValidationError;
         newChild.ChildPropertyErrorMessage = sameValidationError;

         parent.SetValue(x => x.Child, newChild);

         ValidationError oldError = oldChild
            .ValidationResult
            .Errors
            .Single();

         ValidationError newError = newChild
            .ValidationResult
            .Errors
            .Single();

         Assert.AreNotSame(oldError, newError);

         bool newErrorIsContained = parent
            .ValidationResult
            .Errors
            .Any(x => Object.ReferenceEquals(x, newError));

         bool oldErrorIsContained = parent
            .ValidationResult
            .Errors
            .Any(x => Object.ReferenceEquals(x, oldError));

         Assert.IsFalse(oldErrorIsContained);
         Assert.IsTrue(newErrorIsContained);
      }

      private sealed class ParentVM : ViewModel<ParentVMDescriptor> {
         public const string ChildInvalidMessage = "Child is invalid";

         public static readonly ParentVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ParentVMDescriptor>()
            .For<ParentVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Child = v.VM.Of<ChildVM>();
            })
            .WithValidators(b => {
               b.PropagateChildErrors(ChildInvalidMessage);
               b.ValidateDescendant(x => x.Child)
                  .Check(x => x.ChildProperty).Custom(args => {
                     var message = args.Owner.ChildPropertyErrorMessage;
                     if (message != null) {
                        args.AddError(message);
                     }
                  });
            })
            .Build();

         public ParentVM()
            : base(ClassDescriptor) {
         }

         public string ChildPropertyErrorMessage { get; set; }
      }

      private sealed class ParentVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ChildVM> Child { get; set; }
      }

      private sealed class ChildVM : ViewModel<ChildVMDescriptor> {
         public static readonly ChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ChildVMDescriptor>()
            .For<ChildVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.ChildProperty = v.Property.Of<string>();
            })
            .WithValidators(b => {
               b.PropagateChildErrors("child is invalid");
               b.Check(x => x.ChildProperty).Custom(args => {
                  var message = args.Owner.ChildPropertyErrorMessage;
                  if (message != null) {
                     args.AddError(message);
                  }
               });
            })
            .Build();

         public ChildVM()
            : base(ClassDescriptor) {
         }

         public string ChildPropertyErrorMessage { get; set; }
      }

      private sealed class ChildVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> ChildProperty { get; set; }
      }
   }
}