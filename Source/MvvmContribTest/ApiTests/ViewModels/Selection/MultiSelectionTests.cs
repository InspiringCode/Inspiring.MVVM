namespace Inspiring.MvvmContribTest.ApiTests.ViewModels.Selection {
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Linq;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.Resources;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class MultiSelectionTests : TestBase {
      private Group Group1 { get; set; }
      private Group Group2 { get; set; }
      private Group Group3 { get; set; }
      private Group InactiveGroup { get; set; }

      [TestInitialize]
      public void Setup() {
         Group1 = new Group("Group 1");
         Group2 = new Group("Group 2");
         Group3 = new Group("Group 3");
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
      public void AllItems_WithFilteredSelectedItems_ReturnsFilteredItemsIncludingSelectedAndIsValid() {
         var allGroups = new[] { Group1, InactiveGroup };
         var selectableGroups = new[] { Group1, InactiveGroup };

         var vm = CreateUserVM(
            allGroups: allGroups,
            filter: x => x.IsActive,
            selectedGroups: InactiveGroup
         );

         AssertAllItemsAreEqual(vm, selectableGroups);
         CollectionAssert.AreEqual(
            new[] { InactiveGroup },
            vm.Groups.SelectedSourceItems.ToArray()
         );
         Assert.IsTrue(vm.IsValid);
      }

      [TestMethod]
      public void AllItems_WithNonExistingSelectedItems_ReturnsAllItemsIncludingSelectedAndIsInvalid() {
         var allGroups = new[] { Group1 };
         var selectableGroups = new[] { Group1, Group2 };

         var vm = CreateUserVM(
            allGroups: allGroups,
            selectedGroups: Group2
         );

         AssertAllItemsAreEqual(vm, selectableGroups);
         CollectionAssert.AreEqual(
            new[] { Group2 },
            vm.Groups.SelectedSourceItems.ToArray()
         );

         var lazyLoadDummy = vm.Groups.SelectedItems;

         Assert.IsFalse(vm.IsValid);
      }

      [TestMethod]
      public void AllItems_WithFilteredAndNonExistingSelectedItems_ReturnsFilteredItemsIncludingSelectedAndIsInvalid() {
         var allGroups = new[] { Group1, InactiveGroup };
         var selectableGroups = new[] { Group1, Group2 };

         var vm = CreateUserVM(
            allGroups: allGroups,
            filter: x => x.IsActive,
            selectedGroups: Group2
         );

         AssertAllItemsAreEqual(vm, selectableGroups);
         CollectionAssert.AreEqual(
            new[] { Group2 },
            vm.Groups.SelectedSourceItems.ToArray()
         );

         var lazyLoadDummy = vm.Groups.SelectedItems;
         Assert.IsFalse(vm.IsValid);
      }

      [TestMethod]
      public void ValidationResult_WhenExistingItemIsSelectedWhenCurrentItemDoesNotExist_BecomesValid() {
         var allGroups = new[] { Group1, InactiveGroup };
         var selectableGroups = new[] { Group1, Group2 };

         var vm = CreateUserVM(
            allGroups: allGroups,
            filter: x => x.IsActive,
            selectedGroups: Group2
         );
         
         var selectionVM = vm.GetValue(x => x.Groups);
         selectionVM.Load(x => x.SelectedItems);

         Assert.IsFalse(selectionVM.IsValid);

         var existingItem = selectionVM
            .AllItems
            .Single(x => x.Source == Group1);

         selectionVM.SelectedItems.Clear();
         ValidationAssert.IsValid(selectionVM);
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
            .Single(x => x.Source == Group2);

         var selectedItems = new List<SelectableItemVM<Group, GroupVM>>(vm.Groups.SelectedItems);
         selectedItems.Add(additionalItem);

         SetSelectedItems(vm, selectedItems);

         AssertSelectedItemsAreEqual(vm, selectedItems);
         AssertSelectedSourceItemsAreEqual(vm, selectedItems.Select(x => x.VM));
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
            .Single(x => x.Source == Group2);

         var selectedItems = new List<SelectableItemVM<Group, GroupVM>>(vm.Groups.SelectedItems);
         selectedItems.Insert(0, additionalItem);
         SetSelectedItems(vm, selectedItems);

         AssertSelectedItemsAreEqual(vm, selectedItems);
         AssertSelectedSourceItemsAreEqual(vm, selectedItems.Select(x => x.VM));
      }

      [TestMethod]
      public void RemoveFirstSelectedItem_ModifiesSourceCollection() {
         UserVM vm = CreateUserVMWithItems();

         var selectedItems = new List<SelectableItemVM<Group, GroupVM>>(vm.Groups.SelectedItems);
         selectedItems.RemoveAt(0);
         SetSelectedItems(vm, selectedItems);

         AssertSelectedItemsAreEqual(vm, selectedItems);
         AssertSelectedSourceItemsAreEqual(vm, selectedItems.Select(x => x.VM));
      }

      [TestMethod]
      public void RemoveLastSelectedItem_ModifiesSourceCollection() {
         UserVM vm = CreateUserVMWithItems();

         var selectedItems = new List<SelectableItemVM<Group, GroupVM>>(vm.Groups.SelectedItems);
         selectedItems.RemoveAt(selectedItems.Count - 1);
         SetSelectedItems(vm, selectedItems);

         AssertSelectedItemsAreEqual(vm, selectedItems);
         AssertSelectedSourceItemsAreEqual(vm, selectedItems.Select(x => x.VM));
      }

      [TestMethod]
      public void ClearSelectedItems_ModifiesSourceCollection() {
         UserVM vm = CreateUserVMWithItems();

         var selectedItems = new List<object>(vm.Groups.SelectedItems);
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


      [TestMethod]
      public void UpdateFromSource_RaisesExpectedEvents() {
         var allGroups = new List<Group> { Group1 };
         var selectedGroups = new List<Group> { Group1 };

         UserVM vm = CreateUserVM(
            allGroupsSelector: x => allGroups,
            selectedGroupsSelector: x => selectedGroups
         );

         Assert.AreEqual(1, vm.Groups.SelectedItems.Count); // Trigger initial load

         allGroups = new List<Group> { Group2 };
         selectedGroups = new List<Group> { Group2 };

         string eventSequence = "|";

         vm.Groups.PropertyChanged += (_, e) => {
            if (e.PropertyName == "SelectedItems") {
               eventSequence += ("PropertyChanged" + "|");
            }
         };

         ((IBindingList)vm.Groups.AllItems).ListChanged += (_, e) => {
            eventSequence += ("ListChanged" + "|");
         };

         vm.RefreshGroups();

         Assert.IsTrue(eventSequence.Contains("|ListChanged|PropertyChanged|")); // TODO: Investigate why so many events are raised.
      }

      [TestMethod]
      public void RefreshOfMultiSelectionProperty_RefreshesAllItemsAndSelectedItems() {
         var allGroups = new List<Group> { Group1 };
         var selectedGroups = new List<Group> { Group1 };

         UserVM vm = CreateUserVM(
            allGroupsSelector: x => allGroups,
            selectedGroupsSelector: x => selectedGroups
         );

         Assert.AreEqual(1, vm.Groups.SelectedItems.Count); // Trigger initial load

         allGroups = new List<Group> { Group2 };
         selectedGroups = new List<Group> { Group2 };

         vm.RefreshGroups();

         AssertAllItemsAreEqual(vm, allGroups);
         CollectionAssert.AreEqual(
            selectedGroups,
            vm.Groups.SelectedItems.Select(x => x.Source).ToArray()
         );
      }

      [TestMethod]
      public void UpdateFromSource() {
         UserVM vm = CreateUserVMWithItems();
         vm.RefreshGroups();
      }

      [TestMethod]
      public void EnableUndo_EnablesUndoSetValueBehavior() {
         UserVM vm = CreateUserVMWithItems();

         var department = vm.GetValue(x => x.Groups);

         var relevantProperties = new[] { 
            department.GetProperty(x => x.AllItems), 
            department.GetProperty(x => x.SelectedItems)
         };

         foreach (var property in relevantProperties) {
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
      public void SetSelectedSourceItems_SetsSelectedItems() {
         UserVM vm = CreateUserVMWithItems();

         var newSelectedSourceItems = new Group[] { Group2, Group3 };

         vm.Groups.SelectedSourceItems = newSelectedSourceItems;

         var expectedSelectedItems = vm.Groups
            .AllItems
            .Where(x => x.Source.Equals(Group2) || x.Source.Equals(Group3))
            .ToArray();

         CollectionAssert.AreEquivalent(expectedSelectedItems, vm.Groups.SelectedItems);
      }

      [TestMethod]
      public void SetSelectedSourceItems_ToNull_ClearsSelectedItems() {
         UserVM vm = CreateUserVMWithItems();

         vm.Groups.SelectedSourceItems = null;

         Assert.AreEqual(0, vm.Groups.SelectedItems.Count);
      }

      [TestMethod]
      public void SetSelectedSourceItems_ThatAreNotContainedByAllSourceItems_ThrowsException() {
         UserVM vm = CreateUserVMWithItems();

         var notContainedGroup = new Group("notContainedGroup");
         var newSelectedSourceItems = new Group[] { notContainedGroup, Group3 };

         AssertHelper.Throws<ArgumentException>(() => {
            vm.Groups.SelectedSourceItems = newSelectedSourceItems;
         });
      }

      [TestMethod]
      public void CreateMultiSelection_WithSelectedItems_SetsIsSelectedProperty() {
         UserVM vm = CreateUserVMWithItems();

         var selectedGroup1 = vm.Groups.AllItems.Single(x => x.Source.Equals(Group1));
         var selectedGroup2 = vm.Groups.AllItems.Single(x => x.Source.Equals(Group2));

         Assert.IsTrue(selectedGroup1.GetValue(x => x.IsSelected));
         Assert.IsTrue(selectedGroup2.GetValue(x => x.IsSelected));
      }

      [TestMethod]
      public void SetSelectedItemsToNull_ClearsIsSelectedProperty() {
         UserVM vm = CreateUserVMWithItems();

         vm.Groups.SelectedItems.Clear();

         vm.Groups.AllItems.ForEach(x => Assert.IsFalse(x.GetValue(p => p.IsSelected)));
      }

      [TestMethod]
      public void AddSelectedItems_SetsIsSelectedProperty() {
         UserVM vm = CreateUserVMWithItems();

         var additionalItem3 = vm
           .Groups
           .AllItems
           .Single(x => x.Source == Group3);

         vm.Groups.SelectedItems.Add(additionalItem3);

         var selectedItem3 = vm.Groups.SelectedItems.Single(x => x.Source.Equals(Group3));

         Assert.IsTrue(selectedItem3.GetValue(x => x.IsSelected));
      }

      [TestMethod]
      public void SetIsSelectedProperty_AddsItemToSelectedItems() {
         UserVM vm = CreateUserVMWithItems();

         var additionalItem3 = vm
           .Groups
           .AllItems
           .Single(x => x.Source == Group3);

         additionalItem3.IsSelected = true;

         Assert.AreSame(additionalItem3, vm.Groups.SelectedItems.Single(x => x.Source.Equals(Group3)));
      }

      [TestMethod]
      public void ClearIsSelectedProperty_RemovesItemFromSelectedItems() {
         UserVM vm = CreateUserVMWithItems();

         var additionalItem2 = vm
           .Groups
           .SelectedItems
           .Single(x => x.Source == Group2);

         additionalItem2.IsSelected = false;

         Assert.IsFalse(vm.Groups.SelectedItems.Contains(additionalItem2));
      }

      [TestMethod]
      public void InitialEmptySourceItems_RefreshesAllItemsWhenSourceItemIsAdded() {
         List<Group> allGroups = new List<Group>();

         var vm = CreateUserVM(
            allGroupsList: allGroups
         );

         allGroups.Add(Group1);

         var groups = vm.GetValue(x => x.Groups);

         Assert.IsTrue(groups
            .AllItems
            .Any(x => x.Source.Equals(Group1))
         );
      }

      [TestMethod]
      public void DataErrorInfoForSelectedItems_ReturnsAggregatedErrorMessages() {
         string itemPropertyError = "Item Property error.";
         string itemViewModelError = "Item VM error.";
         string viewModelError = "Selection VM error.";

         Group firstGroup = new Group("First group");
         Group secondGroup = new Group("Second group");

         var vm = CreateUserVM(
            allGroups: new[] { firstGroup, secondGroup },
            validatorBuilder: b => {
               b.ValidateDescendant(x => x.Groups)
                  .ValidateDescendant(x => x.AllItems)
                  .ValidateDescendant(x => x.VM)
                  .Check(x => x.Name)
                  .Custom(args => args.AddError(itemPropertyError));

               b.ValidateDescendant(x => x.Groups)
                  .ValidateDescendant(x => x.AllItems)
                  .ValidateDescendant(x => x.VM)
                  .CheckViewModel(args => args.AddError(itemViewModelError));

               b.ValidateDescendant(x => x.Groups)
                  .CheckViewModel(args => args.AddError(viewModelError));
            }
         );

         vm.Revalidate(ValidationScope.SelfAndAllDescendants);

         IDataErrorInfo errorInfo = vm.Groups;
         string actualMessage = errorInfo["SelectedItems"];

         string expectedMessage =
            Localized.MultiSelectionCompositeValidationError + Environment.NewLine +
            Localized.MultiSelectionCompositeValidationErrorPropertyLine.FormatWith(
               firstGroup.Name,
               itemPropertyError + Localized.MultiSelectionCompositeValidationErrorSeparator + itemViewModelError
            ) + Environment.NewLine +
            Localized.MultiSelectionCompositeValidationErrorPropertyLine.FormatWith(
               secondGroup.Name,
               itemPropertyError + Localized.MultiSelectionCompositeValidationErrorSeparator + itemViewModelError
            ) + Environment.NewLine +
            Localized.MultiSelectionCompositeValidationErrorViewModelLine.FormatWith(viewModelError);

         Assert.AreEqual(expectedMessage, actualMessage);
      }

      [TestMethod]
      public void DataErrorInfoForSelectedItems_ReturnsNoErrorMessage() {
         Group firstGroup = new Group("First group");
         Group secondGroup = new Group("Second group");

         var vm = CreateUserVM(
            allGroups: new[] { firstGroup, secondGroup }
         );

         vm.Revalidate(ValidationScope.SelfAndAllDescendants);

         IDataErrorInfo errorInfo = vm.Groups;
         string actualMessage = errorInfo["SelectedItems"];

         Assert.IsNull(actualMessage);
      }

      [TestMethod]
      public void GetValueOfSelectedItems_DoesNotLoadAllItems() {
         var vm = CreateUserVMWithItems();
         vm.Groups.GetValue(x => x.SelectedItems);
         ViewModelAssert.IsNotLoaded(vm.Groups, x => x.AllItems);
      }

      [TestMethod]
      public void SetSelectedSourceItems_DoesNotLoadAllItems() {
         var vm = CreateUserVMWithItems();
         vm.Groups.Load(x => x.SelectedItems);
         vm.Groups.SelectedSourceItems = new[] { Group1, Group3 };
         ViewModelAssert.IsNotLoaded(vm.Groups, x => x.AllItems);
      }

      [TestMethod]
      public void Revalidate_DoesNotLoadAllItems() {
         var vm = CreateUserVMWithItems();

         vm.Groups.Load(x => x.SelectedItems);
         vm.Groups.Revalidate(ValidationScope.SelfAndLoadedDescendants);

         ViewModelAssert.IsNotLoaded(vm.Groups, x => x.AllItems);
      }

      /// <summary>
      ///   Asserts that the source groups of the 'AllItems' property of the
      ///   selection VM are equal to the given source items.
      /// </summary>
      private void AssertAllItemsAreEqual(UserVM vm, IEnumerable<Group> expectedSourceItems) {
         var expected = expectedSourceItems.ToArray();
         var actual = vm.Groups.AllItems.Select(x => x.Source).ToArray();

         CollectionAssert.AreEqual(expected, actual);
      }

      /// <summary>
      ///   Asserts that the 'SelectedItems' collection of the selection VM contains
      ///   the given group VMs.
      /// </summary>
      private void AssertSelectedItemsAreEqual(UserVM vm, IEnumerable expectedSelectedItems) {
         var expected = expectedSelectedItems.Cast<object>().ToArray();
         var actual = vm.Groups.SelectedItems.ToArray();

         CollectionAssert.AreEqual(expected, actual);
      }

      /// <summary>
      ///   Asserts that the 'Groups' property of the 'User' source object contains
      ///   the same groups as the source groups of the given group VMs.
      /// </summary>
      private void AssertSelectedSourceItemsAreEqual(UserVM vm, IEnumerable expectedSelectedItems) {
         var expected = expectedSelectedItems.Cast<GroupVM>().Select(x => x.Source).ToArray();
         var actual = vm.Source.Groups.ToArray();

         CollectionAssert.AreEqual(expected, actual);
      }

      private void SetSelectedItems(UserVM vm, IEnumerable selectedItems) {
         // HACK: Refactor descriptor concept?
         IViewModel selection = vm.Groups;
         //var selectionDescriptor = (MultiSelectionVMDescriptor<Group, GroupVM>)selection.Descriptor;
         var selectionDescriptor = (MultiSelectionVMDescriptor<Group, SelectableItemVM<Group, GroupVM>>)selection.Descriptor;
         selection.SetDisplayValue(selectionDescriptor.SelectedItems, selectedItems);
      }

      private UserVM CreateUserVMWithItems(Action<RootValidatorBuilder<UserVM, UserVM, UserVMDescriptor>> validatorBuilder = null) {
         return CreateUserVM(
            allGroups: new[] { Group1, Group2, Group3, InactiveGroup },
            selectedGroups: new[] { Group1, Group2 },
            validatorBuilder: validatorBuilder
         );
      }

      private UserVM CreateUserVM(
         Func<Group, bool> filter = null,
         Group[] allGroups = null,
         List<Group> allGroupsList = null,
         Func<User, IEnumerable<Group>> allGroupsSelector = null,
         Func<User, ICollection<Group>> selectedGroupsSelector = null,
         Action<RootValidatorBuilder<UserVM, UserVM, UserVMDescriptor>> validatorBuilder = null,
         params Group[] selectedGroups
      ) {
         if (allGroups != null && allGroupsSelector != null) {
            throw new ArgumentException();
         }

         validatorBuilder = validatorBuilder ?? (b => {
            b.ValidateDescendant(x => x.Groups).CheckCollection(x => x.SelectedItems).Custom(args => {
               
            });
            b.OnlyExistingItemsAreSelected(x => x.Groups);
         });

         var sourceUser = new User(selectedGroups);

         var descriptorBuilder = VMDescriptorBuilder
            .OfType<UserVMDescriptor>()
            .For<UserVM>()
            .WithProperties((d, c) => {
               var u = c.GetPropertyBuilder(x => x.Source);

               var builder = selectedGroupsSelector != null ?
                  u.MultiSelection(selectedGroupsSelector) :
                  u.MultiSelection(x => x.Groups);

               if (filter != null) {
                  builder = builder.WithFilter(filter);
               }

               if (allGroups != null) {
                  builder = builder.WithItems(x => allGroups);
               }

               if (allGroupsList != null) {
                  builder = builder.WithItems(x => allGroupsList);
               }

               if (allGroupsSelector != null) {
                  builder = builder.WithItems(allGroupsSelector);
               }

               builder = builder.EnableUndo();

               d.Name = u.Property.MapsTo(x => x.Name);
               d.Groups = builder.Of<GroupVM>();
            });

         if (validatorBuilder != null) {
            descriptorBuilder = descriptorBuilder.WithValidators(validatorBuilder);
         }

         UserVMDescriptor descriptor = descriptorBuilder.Build();

         var vm = new UserVM(descriptor);
         vm.InitializeFrom(sourceUser);
         return vm;
      }
   }
}