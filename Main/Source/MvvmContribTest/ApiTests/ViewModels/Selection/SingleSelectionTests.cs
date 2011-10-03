namespace Inspiring.MvvmContribTest.ApiTests.ViewModels.Selection {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   // TODO: Write more tests!
   [TestClass]
   public class SingleSelectionTests : TestBase {
      private Department Department1 { get; set; }
      private Department Department2 { get; set; }
      private Department InactiveDepartment { get; set; }

      [TestInitialize]
      public void Setup() {
         Department1 = new Department("Department 1");
         Department2 = new Department("Department 2");
         InactiveDepartment = new Department("Inactive Department", isActive: false);
      }

      [TestMethod]
      public void GetValueOfSingleSelectionProperty_AlwaysReturnsSameInstance() {
         var vm = CreateUserVMWithItems();
         var first = vm.Department;
         var second = vm.Department;

         Assert.AreEqual(first, second);
      }

      [TestMethod]
      public void AllItems_WithoutFilter_ReturnsAllSourceItems() {
         var departments = new[] { Department1, InactiveDepartment };
         var vm = CreateUserVM(allDepartments: departments);

         AssertAllItemsAreEqual(vm, departments);
      }

      [TestMethod]
      public void AllItems_WithFilter_ReturnsFilteredItems() {
         var allDepartments = new[] { Department1, InactiveDepartment };
         var selectableDepartments = new[] { Department1 };

         var vm = CreateUserVM(allDepartments: allDepartments, filter: x => x.IsActive);

         AssertAllItemsAreEqual(vm, selectableDepartments);
      }

      [TestMethod]
      public void AllItems_WithFilteredSelectedItem_ReturnsFilteredItemsIncludingSelectedAndIsValid() {
         var allDepartments = new[] { Department1, InactiveDepartment };
         var selectableDepartments = new[] { Department1, InactiveDepartment };

         var vm = CreateUserVM(
            allDepartments: allDepartments,
            filter: x => x.IsActive,
            selectedDepartment: InactiveDepartment
         );

         AssertAllItemsAreEqual(vm, selectableDepartments);
         Assert.AreEqual(InactiveDepartment, vm.Department.SelectedItem.Source);
         Assert.IsTrue(vm.IsValid);
      }

      [TestMethod]
      public void AllItems_WithNonExistingSelectedItem_ReturnsAllItemsIncludingSelectedAndIsInvalid() {
         var allDepartments = new[] { Department1 };
         var allIncludingSelected = new[] { Department1, Department2 };

         var vm = CreateUserVM(
            allDepartments: allDepartments,
            selectedDepartment: Department2
         );

         AssertAllItemsAreEqual(vm, allIncludingSelected);
         Assert.AreEqual(Department2, vm.Department.SelectedItem.Source);
         Assert.IsFalse(vm.IsValid);
      }

      [TestMethod]
      public void SelectedItem_WhenNonExistingItemIsInitiallySelected_IsInvalid() {
         var vm = CreateUserVM(
            allDepartments: new[] { Department1 },
            selectedDepartment: Department2
         );

         ValidationAssert.IsValid(vm);

         vm.Department.Load(x => x.SelectedItem);
         ValidationAssert.IsInvalid(vm);
      }

      [TestMethod]
      public void ValidationResult_WhenExistingItemIsSelectedWhenCurrentItemDoesNotExist_BecomesValid() {
         var allDepartments = new[] { Department1 };
         var allIncludingSelected = new[] { Department1, Department2 };

         var vm = CreateUserVM(
            allDepartments: allDepartments,
            selectedDepartment: Department2
         );

         var selectionVM = vm.GetValue(x => x.Department);
         selectionVM.Load(x => x.SelectedItem);

         Assert.IsFalse(selectionVM.IsValid);

         var existingItem = selectionVM
            .AllItems
            .Single(x => x.Source == Department1);

         selectionVM.SetValue(x => x.SelectedItem, existingItem);

         ValidationAssert.IsValid(selectionVM);
      }

      [TestMethod]
      public void AllItems_WithFilteredAndNonExistingSelectedItem_ReturnsFilteredItemsIncludingSelectedAndIsInvalid() {
         var allDepartments = new[] { Department1, InactiveDepartment };
         var filteredIncludingSelected = new[] { Department1, Department2 };

         var vm = CreateUserVM(
            allDepartments: allDepartments,
            filter: x => x.IsActive,
            selectedDepartment: Department2
         );

         AssertAllItemsAreEqual(vm, filteredIncludingSelected);
         Assert.AreEqual(Department2, vm.Department.SelectedItem.Source);
         Assert.IsFalse(vm.IsValid);
      }

      [TestMethod]
      public void AllItems_WithoutItemsSource_UsesServiceLocator() {
         Department[] allItems = new[] { Department1 };

         var locator = new ServiceLocatorStub();
         locator.Register<IEnumerable<Department>>(allItems);
         ServiceLocator.SetServiceLocator(locator);

         var vm = CreateUserVM();

         AssertAllItemsAreEqual(vm, allItems);
      }

      [TestMethod]
      public void SetSelectedItem_SetsSourceProperty() {
         var vm = CreateUserVMWithItems();
         var newDepartment = vm.Department.AllItems.Last();

         vm.Department.SelectedItem = newDepartment;

         Assert.AreSame(newDepartment.Source, vm.Source.Department);
      }

      [TestMethod]
      public void SetSelectedItem_ToNull_SetsSourcePropertyToNull() {
         var vm = CreateUserVMWithItems();
         vm.Department.SelectedItem = vm.Department.AllItems.First();

         vm.Department.SelectedItem = null;

         Assert.IsNull(vm.Source.Department);
      }

      [TestMethod]
      public void EnableUndo_EnablesUndoSetValueBehavior() {
         UserVM vm = CreateUserVMWithItems();

         IViewModel department = vm.GetValue(x => x.Department);

         foreach (var property in department.Descriptor.Properties) {
            bool found = false;
            for (IBehavior b = property.Behaviors; b != null; b = b.Successor) {
               if (b.GetType().Name.Contains("UndoSetValueBehavior") ||
                   b.GetType().Name.Contains("UndoCollectionModifcationBehavior")) {
                  found = true;
                  break;
               }
            }
            Assert.IsTrue(found);
         }
      }

      [TestMethod]
      public void SetSelectedSourceItem_ThatIsNotContainedByAllSourceItems_ThrowsException() {
         var vm = CreateUserVMWithItems();
         var notContainedSourceItem = new Department("notContainedDepartment");

         AssertHelper.Throws<ArgumentException>(() => {
            vm.Department.SelectedSourceItem = notContainedSourceItem;
         });
      }

      [TestMethod]
      public void SetSelectedSourceItem_ThatIsContainedByAllSourceItems_SetsSelectedItem() {
         var vm = CreateUserVMWithItems();

         vm.Department.SelectedSourceItem = Department1;

         var expectedSelectedItem = vm
            .Department
            .AllItems
            .Single(y => y.Source.Equals(Department1));

         Assert.AreSame(expectedSelectedItem, vm.Department.SelectedItem);
      }

      [TestMethod]
      public void SetSelectedSourceItem_ToNull_SetsSelectedItemToNull() {
         var vm = CreateUserVMWithItems();

         vm.Department.SelectedSourceItem = null;

         Assert.IsNull(vm.Department.SelectedSourceItem);
      }

      [TestMethod]
      public void SetSelectedSourceItem_ToInactiveItem_SetsSelectedItemToInactiveItem() {
         var vm = CreateUserVMWithItems();
         vm.Department.SelectedSourceItem = InactiveDepartment;

         Assert.AreEqual(InactiveDepartment, vm.Source.Department);
         Assert.AreEqual(InactiveDepartment, vm.Department.SelectedSourceItem);
         Assert.AreEqual(InactiveDepartment, vm.Department.SelectedItem.Source);
      }

      [TestMethod]
      public void CreateSingleSelection_WithSelectedItem_SetsIsSelectedPropertyOnSelectedItem() {

         var vm = CreateUserVM(
            allDepartments: new[] { Department1, Department2, InactiveDepartment },
            selectedDepartment: Department1
         );

         var selectedDepartment = vm.Department.AllItems.Single(x => x.Source.Equals(Department1));

         Assert.IsTrue(selectedDepartment.GetValue(x => x.IsSelected));
      }

      [TestMethod]
      public void SetSelectedItem_WithPreviousSelectItem_SetsIsSelectedPropertiesProperly() {
         var vm = CreateUserVM(
            allDepartments: new[] { Department1, Department2, InactiveDepartment },
            selectedDepartment: Department1
         );

         var previousSelectedDepartment = vm.Department.AllItems.Single(x => x.Source.Equals(Department1));

         var currentSelectedDepartment = vm.Department.AllItems.Single(x => x.Source.Equals(Department2));

         vm.Department.SelectedItem = currentSelectedDepartment;

         Assert.IsFalse(previousSelectedDepartment.GetValue(x => x.IsSelected));
         Assert.IsTrue(currentSelectedDepartment.GetValue(x => x.IsSelected));
      }

      [TestMethod]
      public void SetIsSelectedProperty_SetsSelectedItem() {
         var vm = CreateUserVM(
           allDepartments: new[] { Department1, Department2, InactiveDepartment }
         );

         var itemToSelect = vm.Department.AllItems.Single(x => x.Source.Equals(Department1));

         itemToSelect.IsSelected = true;

         Assert.AreSame(itemToSelect, vm.Department.SelectedItem);
      }

      [TestMethod]
      public void ClearIsSelectedProperty_ClearsSelectedItem() {
         var vm = CreateUserVM(
           allDepartments: new[] { Department1, Department2, InactiveDepartment },
           selectedDepartment: Department1
         );

         Assert.IsNotNull(vm.Department.SelectedItem);

         var itemToSelect = vm.Department.AllItems.Single(x => x.Source.Equals(Department1));

         itemToSelect.IsSelected = false;

         Assert.IsNull(vm.Department.SelectedItem);
      }

      [TestMethod]
      public void GetValueOfSelectedItem_DoesNotLoadAllItems() {
         var vm = CreateUserVMWithItems();
         vm.Department.GetValue(x => x.SelectedItem);
         ViewModelAssert.IsNotLoaded(vm.Department, x => x.AllItems);
      }

      [TestMethod]
      public void SetSelectedSourceItem_DoesNotLoadAllItems() {
         var vm = CreateUserVMWithItems();
         vm.Department.Load(x => x.SelectedItem);
         vm.Department.SelectedSourceItem = Department2;
         ViewModelAssert.IsNotLoaded(vm.Department, x => x.AllItems);
      }

      [TestMethod]
      public void Revalidate_DoesNotLoadAllItems() {
         var vm = CreateUserVMWithItems();

         vm.Department.Load(x => x.SelectedItem);
         vm.Department.Revalidate(ValidationScope.SelfAndLoadedDescendants);

         ViewModelAssert.IsNotLoaded(vm.Department, x => x.AllItems);
      }

      [TestMethod]
      public void VariousOperations_ShouldNotCallItemsProviderFunction() {
         int providerCalls = 0;

         var vm = CreateUserVM(
            allDepartmentsSelector: user => {
               providerCalls++;
               return new[] { Department1, Department2 };
            }
         );

         Assert.AreEqual(0, providerCalls);

         vm.Department.Load(x => x.SelectedItem);
         Assert.AreEqual(0, providerCalls);

         vm.Department.Load(x => x.AllItems);
         Assert.AreEqual(1, providerCalls);

         vm.Department.Revalidate(ValidationScope.SelfAndAllDescendants);
         Assert.AreEqual(1, providerCalls);

         vm.Department.SelectedSourceItem = Department1;
         Assert.AreEqual(1, providerCalls);
      }

      [TestMethod]
      public void Refresh_OfSingleSelectionProperty_RefreshesAllItems() {
         var allItems = new[] { Department1 };
         var vm = CreateUserVM(allDepartmentsSelector: user => allItems);

         AssertAllItemsAreEqual(vm, allItems);

         allItems = new[] { Department1, Department2 };
         vm.RefreshDepartment();

         AssertAllItemsAreEqual(vm, allItems);
      }

      /// <summary>
      ///   Asserts that the source departments of the 'AllItems' property of the
      ///   selection VM are equal to the given source items.
      /// </summary>
      private void AssertAllItemsAreEqual(UserVM vm, IEnumerable<Department> expectedSourceItems) {
         var expected = expectedSourceItems.ToArray();
         var actual = vm.Department.AllItems.Select(x => x.Source).ToArray();

         CollectionAssert.AreEqual(expected, actual);
      }

      private UserVM CreateUserVMWithItems() {
         return CreateUserVM(
            filter: x => x.IsActive,
            allDepartments: new[] { Department1, Department2, InactiveDepartment },
            selectedDepartment: Department1
         );
      }

      private UserVM CreateUserVM(
         Func<Department, bool> filter = null,
         Department[] allDepartments = null,
         Func<User, IEnumerable<Department>> allDepartmentsSelector = null,
         Department selectedDepartment = null
      ) {
         if (allDepartments != null && allDepartmentsSelector != null) {
            throw new ArgumentException();
         }

         var sourceUser = new User();
         sourceUser.Department = selectedDepartment;

         UserVMDescriptor descriptor = VMDescriptorBuilder
            .OfType<UserVMDescriptor>()
            .For<UserVM>()
            .WithProperties((d, c) => {
               var u = c.GetPropertyBuilder(x => x.Source);

               var builder = u.SingleSelection(x => x.Department).EnableUndo();

               if (filter != null) {
                  builder = builder.WithFilter(filter);
               }

               if (allDepartments != null) {
                  builder = builder.WithItems(x => allDepartments);
               }

               if (allDepartmentsSelector != null) {
                  builder = builder.WithItems(allDepartmentsSelector);
               }

               d.Name = u.Property.MapsTo(x => x.Name);
               //d.Department = builder.Of<DepartmentVM>(DepartmentVM.ClassDescriptor);
               d.Department = builder.Of<DepartmentVM>();
            })
            .WithValidators(b => {
               b.OnlyExistingItemsAreSelected(x => x.Department);
            })
            .Build();

         var vm = new UserVM(descriptor);
         vm.InitializeFrom(sourceUser);
         return vm;
      }
   }
}