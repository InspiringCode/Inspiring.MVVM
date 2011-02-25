namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionValidationTests {
      [TestMethod]
      public void ItemValidation_ExecutesItemValueValidator() {
         var firstItem = new ProjectVM { Title = "Project 1" };
         var secondItem = new ProjectVM { Title = "Project 2" };

         var vm = new EmployeeVM();
         vm.Projects.Add(firstItem);
         vm.Projects.Add(secondItem);

         var args = new ValidatorArguments<string>();
         vm.ValueArgs = args;

         firstItem.Revalidate();

         Assert.AreEqual(1, args.InvocationCount, "Validator was not called once.");

         Assert.AreSame(firstItem.Title, args.Item);
         CollectionAssert.AreEqual(
            new[] { firstItem.Title, secondItem.Title },
            args.Items.ToArray()
         );

         Assert.AreSame(vm, args.ValidationArgs.OwnerVM);
         Assert.AreSame(firstItem, args.ValidationArgs.TargetVM);
      }

      [TestMethod]
      public void ItemValidation_ExecutesItemValidator() {
         var firstItem = new ProjectVM { Title = "Project 1" };
         var secondItem = new ProjectVM { Title = "Project 2" };

         var vm = new EmployeeVM();
         vm.Projects.Add(firstItem);
         vm.Projects.Add(secondItem);

         var args = new ValidatorArguments<ProjectVM>();
         vm.ItemArgs = args;

         firstItem.Revalidate();

         Assert.IsTrue(args.InvocationCount >= 1); // TODO: Better way? Validates twice because it property validation in item.
      }

      [TestMethod]
      public void ItemAddition_ExecutesCollectionValidator() {
         var vm = new EmployeeVM();
         var item = new ProjectVM();

         vm.Projects.Add(item);
         Assert.IsTrue(vm.ItemArgs.InvocationCount >= 1); // TODO: Check why this is executed twice?
      }

      [TestMethod]
      public void ItemAddition_ExecutesItemValidator() {
         var vm = new EmployeeVM();
         var item = new ProjectVM();

         vm.Projects.Add(item);
         Assert.IsTrue(item.WasValidated);
      }

      [TestMethod]
      public void ItemRemoval_ExecutesCollectionValidator() {
         var vm = new EmployeeVM();
         var item = new ProjectVM();

         vm.Projects.Add(item);
         vm.ItemArgs = new ValidatorArguments<ProjectVM>();

         vm.Projects.Remove(item);

         Assert.AreEqual(1, vm.ItemArgs.InvocationCount);
      }

      [TestMethod]
      public void ItemRemoval_ExecutesItemValidator() {
         var vm = new EmployeeVM();
         var item = new ProjectVM();
         
         vm.Projects.Add(item);
         item.WasValidated = false;

         vm.Projects.Remove(item);

         Assert.IsTrue(item.WasValidated);
      }

      [TestMethod]
      public void ClearCollection_ExecutesCollectionValidator() {
         var vm = new EmployeeVM();
         var item = new ProjectVM();

         vm.Projects.Add(item);
         vm.ItemArgs = new ValidatorArguments<ProjectVM>();

         vm.Projects.Clear();

         Assert.AreEqual(1, vm.ItemArgs.InvocationCount);
      }

      [TestMethod]
      public void SetItem_ExecutesCollectionValidatorForPreviousAndCurrentItem() {
         var vm = new EmployeeVM();
         var previousItem = new ProjectVM();
         var item = new ProjectVM();
         
         vm.Projects.Add(previousItem);
         vm.ItemArgs = new ValidatorArguments<ProjectVM>();

         vm.Projects[0] = item;

         CollectionAssert.AreEqual(
            new[] { previousItem, item, item }, // TODO: Check why new item is validated twice?
            vm.ItemArgs.TargetVMHistory
         );
      }

      private sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();

               d.Projects = v.Collection.Of<ProjectVM>(ProjectVM.ClassDescriptor);
            })
            .WithValidators(c => {
               c.CheckCollection(x => x.Projects, x => x.Title).Custom<ProjectVM>((item, items, property, args) => {
                  var vm = (EmployeeVM)args.OwnerVM;

                  vm.ValueArgs.InvocationCount++;
                  vm.ValueArgs.Item = item.GetValue(property);
                  vm.ValueArgs.Items = items.Select(x => x.GetValue(property)).ToArray();
                  vm.ValueArgs.ValidationArgs = args;
                  vm.ValueArgs.TargetVMHistory.Add(args.TargetVM);
               });

               c.CheckCollection(x => x.Projects).Custom((item, items, args) => {
                  var vm = (EmployeeVM)args.OwnerVM;

                  vm.ItemArgs.InvocationCount++;
                  vm.ItemArgs.Item = item;
                  vm.ItemArgs.Items = items;
                  vm.ItemArgs.ValidationArgs = args;
                  vm.ItemArgs.TargetVMHistory.Add(args.TargetVM);
               });
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {

            ValueArgs = new ValidatorArguments<string>();
            ItemArgs = new ValidatorArguments<ProjectVM>();
         }

         public IVMCollection<ProjectVM> Projects {
            get { return GetValue(Descriptor.Projects); }
         }

         public ValidatorArguments<string> ValueArgs { get; set; }
         public ValidatorArguments<ProjectVM> ItemArgs { get; set; }

         //public int ItemValueValidation_InvocationCount { get; set; }
         //public string ItemValueValidation_Value { get; set; }
         //public IEnumerable<string> ItemValueValidation_Values { get; set; }
         //public ValidationArgs ItemValueValidation_Args { get; set; }

         //public int ItemValidation_InvocationCount { get; set; }
         //public List<IViewModel> ItemValidation_TargetVMHistory { get; private set; }
      }

      private sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
      }

      private class ValidatorArguments<T> {
         public ValidatorArguments() {
            TargetVMHistory = new List<IViewModel>();
         }

         public int InvocationCount { get; set; }
         public T Item { get; set; }
         public IEnumerable<T> Items { get; set; }
         public ValidationArgs ValidationArgs { get; set; }
         public List<IViewModel> TargetVMHistory { get; set; }
      }
   }
}