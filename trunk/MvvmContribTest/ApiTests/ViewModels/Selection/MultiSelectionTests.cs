namespace Inspiring.MvvmContribTest.ApiTests.ViewModels.Selection {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.Stubs;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class MultiSelectionTests : TestBase {
      private Group Group1 { get; set; }
      private Group Group2 { get; set; }
      private Group InactiveGroup { get; set; }

      [TestInitialize]
      public void Setup() {
         Group1 = new Group("Group 1");
         Group2 = new Group("Group 2");
         InactiveGroup = new Group("Inactive Group", isActive: false);
      }

      [TestMethod]
      public void AllItems_WithoutFilter_ReturnsAllSourceItems() {
         var groups = new[] { Group1, InactiveGroup };
         var vm = CreateUserVM(allGroups: groups);

         AssertAllItemsAreEqual(vm, groups);
      }

      [TestMethod]
      public void AllItems_WithFilter_ReturnsFilteredItems() {
         var allGroups = new[] { Group1, InactiveGroup };
         var selectableGroups = new[] { Group1 };

         var vm = CreateUserVM(allGroups: allGroups, filter: x => x.IsActive);

         AssertAllItemsAreEqual(vm, selectableGroups);
      }

      [TestMethod]
      public void AllItems_WithoutItemsSource_UsesServiceLocator() {
         Group[] allItems = new[] { Group1 };

         var locator = new ServiceLocatorStub();
         locator.Register<IEnumerable<Group>>(allItems);
         ServiceLocator.SetServiceLocator(locator);

         var vm = CreateUserVM();

         AssertAllItemsAreEqual(vm, allItems);
      }

      [TestMethod]
      public void AppendSelectedItem_ModifiesSourceCollection() {
         UserVM vm = CreateUserVM(
            allGroups: new[] { Group1, Group2, InactiveGroup },
            selectedGroups: new[] { Group1 }
         );

         var additionalItem = vm
            .Groups
            .AllItems
            .Single(x => x.GroupSource == Group2);

         var selectedItems = new List<GroupVM>(vm.Groups.SelectedItems);
         selectedItems.Add(additionalItem);
         SetSelectedItems(vm, selectedItems);

         AssertSelectedItemsAreEqual(vm, selectedItems);
         AssertSelectedSourceItemsAreEqual(vm, selectedItems);
      }

      [TestMethod]
      public void PrependSelectedItem_ModifiesSourceCollection() {
         UserVM vm = CreateUserVM(
            allGroups: new[] { Group1, Group2, InactiveGroup },
            selectedGroups: new[] { Group1 }
         );

         var additionalItem = vm
            .Groups
            .AllItems
            .Single(x => x.GroupSource == Group2);

         var selectedItems = new List<GroupVM>(vm.Groups.SelectedItems);
         selectedItems.Insert(0, additionalItem);
         SetSelectedItems(vm, selectedItems);

         AssertSelectedItemsAreEqual(vm, selectedItems);
         AssertSelectedSourceItemsAreEqual(vm, selectedItems);
      }

      [TestMethod]
      public void RemoveFirstSelectedItem_ModifiesSourceCollection() {
         UserVM vm = CreateUserVMWithItems();

         var selectedItems = new List<GroupVM>(vm.Groups.SelectedItems);
         selectedItems.RemoveAt(0);
         SetSelectedItems(vm, selectedItems);

         AssertSelectedItemsAreEqual(vm, selectedItems);
         AssertSelectedSourceItemsAreEqual(vm, selectedItems);
      }

      [TestMethod]
      public void RemoveLastSelectedItem_ModifiesSourceCollection() {
         UserVM vm = CreateUserVMWithItems();

         var selectedItems = new List<GroupVM>(vm.Groups.SelectedItems);
         selectedItems.RemoveAt(selectedItems.Count - 1);
         SetSelectedItems(vm, selectedItems);

         AssertSelectedItemsAreEqual(vm, selectedItems);
         AssertSelectedSourceItemsAreEqual(vm, selectedItems);
      }

      [TestMethod]
      public void ClearSelectedItems_ModifiesSourceCollection() {
         UserVM vm = CreateUserVMWithItems();

         var selectedItems = new List<GroupVM>(vm.Groups.SelectedItems);
         selectedItems.Clear();
         SetSelectedItems(vm, selectedItems);

         AssertSelectedItemsAreEqual(vm, selectedItems);
         AssertSelectedSourceItemsAreEqual(vm, selectedItems);
      }

      [TestMethod]
      public void SetSelectedItemsToNull_ClearsSourceCollection() {
         UserVM vm = CreateUserVMWithItems();

         SetSelectedItems(vm, null);

         AssertSelectedItemsAreEqual(vm, new GroupVM[] { });
         AssertSelectedSourceItemsAreEqual(vm, new GroupVM[] { });
      }

      /// <summary>
      ///   Asserts that the source groups of the 'AllItems' property of the
      ///   selection VM are equal to the given source items.
      /// </summary>
      private void AssertAllItemsAreEqual(UserVM vm, IEnumerable<Group> expectedSourceItems) {
         var expected = expectedSourceItems.ToArray();
         var actual = vm.Groups.AllItems.Select(x => x.GroupSource).ToArray();

         CollectionAssert.AreEqual(expected, actual);
      }

      /// <summary>
      ///   Asserts that the 'SelectedItems' collection of the selection VM contains
      ///   the given group VMs.
      /// </summary>
      private void AssertSelectedItemsAreEqual(UserVM vm, ICollection<GroupVM> expectedSelectedItems) {
         var expected = expectedSelectedItems.ToArray();
         var actual = vm.Groups.SelectedItems.ToArray();

         CollectionAssert.AreEqual(expected, actual);
      }

      /// <summary>
      ///   Asserts that the 'Groups' property of the 'User' source object contains
      ///   the same groups as the source groups of the given group VMs.
      /// </summary>
      private void AssertSelectedSourceItemsAreEqual(UserVM vm, ICollection<GroupVM> expectedSelectedItems) {
         var expected = expectedSelectedItems.Select(x => x.GroupSource).ToArray();
         var actual = vm.UserSource.Groups.ToArray();

         CollectionAssert.AreEqual(expected, actual);
      }

      private void SetSelectedItems(UserVM vm, IEnumerable<GroupVM> selectedItems) {
         // HACK: Refactor descriptor concept?
         IViewModel selection = vm.Groups;
         var selectionDescriptor = (MultiSelectionVMDescriptor<Group, GroupVM>)selection.Descriptor;
         selection.SetDisplayValue(selectionDescriptor.SelectedItems, selectedItems);
      }

      private UserVM CreateUserVMWithItems() {
         return CreateUserVM(
            allGroups: new[] { Group1, Group2, InactiveGroup },
            selectedGroups: new[] { Group1, Group2 }
         );
      }

      private UserVM CreateUserVM(
         Func<Group, bool> filter = null,
         Group[] allGroups = null,
         Func<User, IEnumerable<Group>> allGroupsSelector = null,
         params Group[] selectedGroups
      ) {
         if (allGroups != null && allGroupsSelector != null) {
            throw new ArgumentException();
         }

         var sourceUser = new User(selectedGroups);

         UserVMDescriptor descriptor = VMDescriptorBuilder
            .For<UserVM>()
            .CreateDescriptor(c => {
               var u = c.GetPropertyBuilder(x => x.UserSource);

               var builder = u.MultiSelection(x => x.Groups);

               if (filter != null) {
                  builder = builder.WithFilter(filter);
               }

               if (allGroups != null) {
                  builder = builder.WithItems(x => allGroups);
               }

               if (allGroupsSelector != null) {
                  builder = builder.WithItems(allGroupsSelector);
               }

               return new UserVMDescriptor {
                  Name = u.Property.MapsTo(x => x.Name),
                  Groups = builder.Of<GroupVM>(GroupVM.Descriptor)
               };
            })
            .Build();

         var vm = new UserVM(descriptor);
         vm.InitializeFrom(sourceUser);
         return vm;
      }
   }
}