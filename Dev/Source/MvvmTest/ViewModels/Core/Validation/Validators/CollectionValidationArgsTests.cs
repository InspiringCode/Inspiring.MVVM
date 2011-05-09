namespace Inspiring.MvvmTest.ViewModels.Core.Validation.Validators {
   using System;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionValidationArgsTests : ValidationArgsFixture {
      [TestMethod]
      public void Create_PathWithViewModelPlusCollection_SetPropertiesCorrectly() {
         var owner = new EmployeeListVM();
         var args = CreateArgs(owner);

         Assert.AreEqual(owner, args.Owner);
         Assert.AreEqual(owner.GetValue(x => x.Employees), args.Items);
      }

      [TestMethod]
      public void Create_PathWithViewModelPlusCollectionPlusProperty_SetPropertiesCorrectly() {
         var owner = new EmployeeListVM();
         var args = CreateArgsWithProperty(owner);

         Assert.AreEqual(owner, args.Owner);
         Assert.AreEqual(owner.GetValue(x => x.Employees), args.Items);
         Assert.AreEqual(EmployeeVM.ClassDescriptor.Name, args.TargetProperty);
      }

      [TestMethod]
      public void AddError_ArgsWithoutProperty_CreatesAndAddsCorrectError() {
         var item = new EmployeeVM();
         var owner = new EmployeeListVM(item);
         var validator = Mock<IValidator>();

         var message = "Test error message";
         var details = new Object();

         var args = CreateArgs(owner);
         args.AddError(item, message, details);

         var expectedError = new ValidationError(
            validator,
            ValidationTarget.ForError(
               ValidationStep.ViewModel,
               item,
               owner.GetValue(x => x.Employees)
            ),
            message,
            details
         );

         ValidationAssert.HasErrors(args.Result, ValidationAssert.FullErrorComparer, expectedError);
      }

      [TestMethod]
      public void AddError_ArgsWithProperty_CreatesAndAddsCorrectError() {
         var item = new EmployeeVM();
         var owner = new EmployeeListVM(item);
         var validator = Mock<IValidator>();
         var step = ValidationStep.Value;

         var message = "Test error message";
         var details = new Object();

         var args = CreateArgsWithProperty(owner, step, validator);
         args.AddError(item, message, details);

         var expectedError = new ValidationError(
            validator,
            ValidationTarget.ForError(
               step,
               item,
               owner.GetValue(x => x.Employees),
               EmployeeVM.ClassDescriptor.Name
            ),
            message,
            details
         );

         ValidationAssert.HasErrors(args.Result, ValidationAssert.FullErrorComparer, expectedError);
      }

      private static CollectionValidationArgs<IViewModel, EmployeeVM> CreateArgs(
         EmployeeListVM owner,
         IValidator validator = null
      ) {
         validator = validator ?? Mock<IValidator>();

         var path = Path.Empty
            .Append(owner)
            .Append(owner.GetValue(x => x.Employees));

         return CollectionValidationArgs<IViewModel, EmployeeVM>.Create(
            validator,
            new ValidationRequest(ValidationStep.ViewModel, path)
         );
      }

      private static CollectionValidationArgs<IViewModel, EmployeeVM, string> CreateArgsWithProperty(
         EmployeeListVM owner,
         ValidationStep step = ValidationStep.Value,
         IValidator validator = null
      ) {
         validator = validator ?? Mock<IValidator>();

         var path = Path.Empty
            .Append(owner)
            .Append(owner.GetValue(x => x.Employees))
            .Append(EmployeeVM.ClassDescriptor.Name);

         return CollectionValidationArgs<IViewModel, EmployeeVM, string>.Create(
            validator,
            new ValidationRequest(step, path)
         );
      }

      private sealed class EmployeeListVM : ViewModel<EmployeeListVMDescriptor> {
         public static readonly EmployeeListVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeListVMDescriptor>()
            .For<EmployeeListVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Employees = v.Collection.Of<EmployeeVM>(EmployeeVM.ClassDescriptor);
            })
            .Build();

         public EmployeeListVM(params EmployeeVM[] items)
            : base(ClassDescriptor) {

            var coll = GetValue(Descriptor.Employees);
            items.ForEach(coll.Add);
         }
      }

      private sealed class EmployeeListVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<EmployeeVM>> Employees { get; set; }
      }
   }
}