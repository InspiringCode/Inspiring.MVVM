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
         return CreateUserVM(x => x.IsActive, new[] { Department1, Department2, InactiveDepartment });
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
               d.Department = builder.Of<DepartmentVM>(DepartmentVM.ClassDescriptor);
            })
            .Build();

         var vm = new UserVM(descriptor);
         vm.InitializeFrom(sourceUser);
         return vm;
      }
   }
}