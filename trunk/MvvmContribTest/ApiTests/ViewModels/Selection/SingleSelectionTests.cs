namespace Inspiring.MvvmContribTest.ApiTests.ViewModels.Selection {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.Stubs;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   // TODO: Add test for inactive item is currently selected.
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

         Assert.AreSame(newDepartment.DepartmentSource, vm.UserSource.Department);
      }

      /// <summary>
      ///   Asserts that the source departments of the 'AllItems' property of the
      ///   selection VM are equal to the given source items.
      /// </summary>
      private void AssertAllItemsAreEqual(UserVM vm, IEnumerable<Department> expectedSourceItems) {
         var expected = expectedSourceItems.ToArray();
         var actual = vm.Department.AllItems.Select(x => x.DepartmentSource).ToArray();

         CollectionAssert.AreEqual(expected, actual);
      }

      private UserVM CreateUserVMWithItems() {
         return CreateUserVM(x => x.IsActive, new[] { Department1, Department2, InactiveDepartment });
      }

      private UserVM CreateUserVM(
         Func<Department, bool> filter = null,
         Department[] allDepartments = null,
         Func<User, IEnumerable<Department>> allDepartmentsSelector = null
      ) {
         if (allDepartments != null && allDepartmentsSelector != null) {
            throw new ArgumentException();
         }

         var sourceUser = new User();

         UserVMDescriptor descriptor = VMDescriptorBuilder
            .For<UserVM>()
            .CreateDescriptor(c => {
               var u = c.GetPropertyBuilder(x => x.UserSource);

               var builder = u.SingleSelection(x => x.Department);

               if (filter != null) {
                  builder = builder.WithFilter(filter);
               }

               if (allDepartments != null) {
                  builder = builder.WithItems(x => allDepartments);
               }

               if (allDepartmentsSelector != null) {
                  builder = builder.WithItems(allDepartmentsSelector);
               }

               return new UserVMDescriptor {
                  Name = u.Property.MapsTo(x => x.Name),
                  Department = builder.Of<DepartmentVM>(DepartmentVM.Descriptor)
               };
            })
            .Build();

         var vm = new UserVM(descriptor);
         vm.InitializeFrom(sourceUser);
         return vm;
      }
   }
}