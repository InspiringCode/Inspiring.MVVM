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

         var message = "Test error message";
         var details = new Object();

         var args = CreateArgs(owner);
         args.AddError(item, message, details);

         var expectedError = new ValidationError(args.Validator, item, message, details);
         AssertErrors(args, expectedError);
      }

      [TestMethod]
      public void AddError_ArgsWithProperty_CreatesAndAddsCorrectError() {
         var item = new EmployeeVM();
         var owner = new EmployeeListVM(item);

         var message = "Test error message";
         var details = new Object();

         var args = CreateArgsWithProperty(owner);
         args.AddError(item, message, details);

         var expectedError = new ValidationError(args.Validator, item, EmployeeVM.ClassDescriptor.Name, message, details);
         AssertErrors(args, expectedError);
      }

      private static CollectionValidationArgs<IViewModel, EmployeeVM> CreateArgs(
         EmployeeListVM owner
      ) {
         var path = Path.Empty
            .Append(owner)
            .Append(owner.GetValue(x => x.Employees));

         return CollectionValidationArgs<IViewModel, EmployeeVM>.Create(
            Mock<IValidator>(),
            CreateRequest(path)
         );
      }

      private static CollectionValidationArgs<IViewModel, EmployeeVM, string> CreateArgsWithProperty(
         EmployeeListVM owner
      ) {
         var path = Path.Empty
            .Append(owner)
            .Append(owner.GetValue(x => x.Employees))
            .Append(EmployeeVM.ClassDescriptor.Name);

         return CollectionValidationArgs<IViewModel, EmployeeVM, string>.Create(
            Mock<IValidator>(),
            CreateRequest(path)
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