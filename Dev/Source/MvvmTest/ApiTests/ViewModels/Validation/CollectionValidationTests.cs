namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using ProjectVM = Inspiring.MvvmTest.ApiTests.ViewModels.ProjectVM;

   [TestClass]
   public class CollectionValidationTests : ValidationTestBase {
      private ListVM List { get; set; }

      [TestInitialize]
      public void Setup() {
         List = new ListVM();
      }

      [TestMethod]
      public void ItemInsertion_PerformsCollectionValidationForAllItems() {
         var previouslyValid = CreateItem("Valid item");
         List.Items.Add(previouslyValid);

         var error = Error("Previously valid item error")
            .For(previouslyValid, x => x.CollectionProperty);

         List.ItemsWithInvalidColletionProperty.Add(previouslyValid, error);

         var newItem = CreateItem("New item");
         List.Items.Add(newItem);

         ValidationAssert.Errors(error);
         ValidationAssert.IsValid(newItem);
      }

      [TestMethod]
      public void ItemInsertion_PerformsFullValidationForInsertedItem() {
         var newItem = CreateItem("New item");

         var error = Error("Item property error").For(newItem, x => x.ItemProperty);
         newItem.ItemPropertyError = error;

         List.Items.Add(newItem);

         ValidationAssert.Errors(error);
      }

      [TestMethod]
      public void ItemInserted_PerformsFullValidationForAllCurrentlyInvalidItems() {
         var currentlyInvalidItem = CreateItem("Currently invalid item");
         currentlyInvalidItem.ItemPropertyError = Error("Currently invalid error").Anonymous();
         List.Items.Add(currentlyInvalidItem);

         var currentlyValidItem = CreateItem("Currently valid item");
         List.Items.Add(currentlyValidItem);

         currentlyInvalidItem.ItemPropertyError = null;
         currentlyValidItem.ItemPropertyError = Error("Effectless error").Anonymous();

         var newItem = CreateItem("New item");
         List.Items.Add(newItem);

         ValidationAssert.IsValid(currentlyInvalidItem);
         ValidationAssert.IsValid(currentlyValidItem);
      }

      [TestMethod]
      public void ItemRemoval_PerformsCollectionValidationForAllItems() {
         /*
          * add valid item
          * make collection validation invalid for already added
          * add new item
          * assert collection error of old item
          * 
          * 
          */
      }

      [TestMethod]
      public void ItemRemoval_PerformsFullValidationForRemovedItem() {

      }

      [TestMethod]
      public void ItemRemoval_PerformsFullValidationForAllPreviouslyInvalidItems() {

      }

      private ItemVM CreateItem(string name) {
         return new ItemVM(name);
      }

      private class ListVM : ViewModel<ListVMDescriptor> {
         public static readonly ListVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ListVMDescriptor>()
            .For<ListVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Items = v.Collection.Of<ItemVM>(ItemVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.CheckCollection(x => x.Items, x => x.CollectionProperty)
                  .Custom<ItemVM>(args => {
                     foreach (var invalidItem in args.Owner.ItemsWithInvalidColletionProperty) {
                        args.AddError(invalidItem.Key, invalidItem.Value.Message);
                     }
                  });

               b.CheckCollection(x => x.Items)
                  .Custom(args => {
                     foreach (var invalidItem in args.Owner.ItemsWithInvalidViewModelValidation) {
                        args.AddError(invalidItem.Key, invalidItem.Value.Message);
                     }
                  });
            })
            .Build();

         public ListVM()
            : base(ClassDescriptor) {
            ItemsWithInvalidColletionProperty = new Dictionary<ItemVM, ValidationError>();
            ItemsWithInvalidViewModelValidation = new Dictionary<ItemVM, ValidationError>();
         }

         public Dictionary<ItemVM, ValidationError> ItemsWithInvalidColletionProperty { get; private set; }
         public Dictionary<ItemVM, ValidationError> ItemsWithInvalidViewModelValidation { get; private set; }

         public IVMCollection<ItemVM> Items {
            get { return GetValue(Descriptor.Items); }
         }
      }

      private class ListVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ItemVM>> Items { get; set; }
      }

      private class ItemVM : ViewModel<ItemVMDescriptor> {
         public static readonly ItemVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ItemVMDescriptor>()
            .For<ItemVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.ItemProperty = v.Property.Of<string>();
               d.CollectionProperty = v.Property.Of<string>();
            })
            .WithValidators(b => {
               b.Check(x => x.ItemProperty).Custom(args => {
                  var error = args.Owner.ItemPropertyError;

                  if (error != null) {
                     args.AddError(error.Message);
                  }
               });
            })
            .Build();

         private string _name;

         public ItemVM(string name)
            : base(ClassDescriptor) {
            _name = name;
         }

         public ValidationError ItemPropertyError { get; set; }

         public override string ToString() {
            return _name;
         }
      }

      private class ItemVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> ItemProperty { get; set; }
         public IVMPropertyDescriptor<string> CollectionProperty { get; set; }
      }
   }


   [TestClass]
   public class CollectionValidationTestsOld : TestBase {
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

         Assert.AreSame(vm, args.ValidationArgs.Owner);
         //throw new NotImplementedException();
         //Assert.AreSame(firstItem, args.ValidationArgs.Target);
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
               c.CheckCollection(x => x.Projects, x => x.Title).Custom<ProjectVM>(args => {
                  var vm = args.Owner;

                  vm.ValueArgs.InvocationCount++;
                  //vm.ValueArgs.Item = item.GetValue(property);
                  vm.ValueArgs.Items = args.Items.Select(x => x.GetValue(args.TargetProperty)).ToArray();
                  //vm.ValueArgs.ValidationArgs = args;
                  //vm.ValueArgs.TargetVMHistory.Add(args.TargetVM);
               });

               c.CheckCollection(x => x.Projects).Custom(args => {
                  var vm = args.Owner;

                  vm.ItemArgs.InvocationCount++;
                  //vm.ItemArgs.Item = item;
                  vm.ItemArgs.Items = args.Items;
                  //vm.ItemArgs.ValidationArgs = args;
                  //vm.ItemArgs.TargetVMHistory.Add(args.TargetVM);
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
         public ValidationArgs<IViewModel> ValidationArgs { get; set; }
         public List<IViewModel> TargetVMHistory { get; set; }
      }
   }
}