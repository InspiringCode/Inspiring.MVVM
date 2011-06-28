namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   [TestClass]
   public class ViewModelPropertyValidationTests: ValidationTestBase {
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
      
      private sealed class ParentVM : ViewModel<ParentVMDescriptor> {
         public static readonly ParentVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ParentVMDescriptor>()
            .For<ParentVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Child = v.VM.Of<ChildVM>();
            })
            .Build();

         public ParentVM()
            : base(ClassDescriptor) {
         }
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