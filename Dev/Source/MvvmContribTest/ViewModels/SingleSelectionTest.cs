﻿//namespace Inspiring.MvvmContribTest.ViewModels {
//   using System;
//   using System.Collections.Generic;
//   using System.Linq;
//   using Inspiring.Mvvm.ViewModels;
//   using Inspiring.MvvmTest;
//   using Microsoft.VisualStudio.TestTools.UnitTesting;
//   using System.ComponentModel;
//   using Inspiring.Mvvm.ViewModels.Core;

//   [TestClass]
//   public class SingleSelectionTest : TestBase {
//      private PersonVM _vm;
//      private Person _person;
//      private PersonStatus _selectableStatus1;
//      private PersonStatus _selectableStatus2;
//      private PersonStatus _unselectableStatus1;
//      private PersonStatus _unselectableStatus2;
//      private List<PersonStatus> _allStatus;

//      [TestInitialize]
//      public void Setup() {
//         _selectableStatus1 = new PersonStatus {
//            Name = "1",
//            Description = "Selectable 1",
//            IsSelectable = true
//         };

//         _selectableStatus2 = new PersonStatus {
//            Name = "2",
//            Description = "Selectable 2",
//            IsSelectable = true
//         };

//         _unselectableStatus1 = new PersonStatus {
//            Name = "1",
//            Description = "Unselectable 1",
//            IsSelectable = false
//         };

//         _unselectableStatus2 = new PersonStatus {
//            Name = "2",
//            Description = "Unselectable 2",
//            IsSelectable = false
//         };

//         _allStatus = new List<PersonStatus> {
//            _selectableStatus1,
//            _selectableStatus2,
//            _unselectableStatus1,
//            _unselectableStatus2
//         };

//         _person = new Person();
//         _person.CurrentStatus = _selectableStatus1;

//         _vm = new PersonVM(_allStatus);
//         _vm.InitializeFrom(_person);
//      }

//      [TestMethod]
//      public void CheckAllItems() {
//         var actualSourceItems = GetAllItems(PersonVM.Descriptor.StatusUnfiltered)
//            .Select(x => x.Source)
//            .ToArray();

//         var expected = _allStatus;

//         CollectionAssert.AreEquivalent(expected, actualSourceItems);
//      }

//      [TestMethod]
//      public void CheckAllItemsFiltered() {
//         var actualSourceItems = GetAllItems(PersonVM.Descriptor.Status)
//            .Select(x => x.Source)
//            .ToArray();

//         var expected = new PersonStatus[] { _selectableStatus1, _selectableStatus2 };

//         CollectionAssert.AreEquivalent(expected, actualSourceItems);
//      }

//      [TestMethod]
//      public void CheckAllItemsFilteredWithFilteredCurrentItem() {
//         _person.CurrentStatus = _unselectableStatus1;

//         var actualSourceItems = GetAllItems(PersonVM.Descriptor.Status)
//            .Select(x => x.Source)
//            .ToArray();

//         var expected = new PersonStatus[] { _selectableStatus1, _selectableStatus2, _unselectableStatus1 };

//         CollectionAssert.AreEquivalent(expected, actualSourceItems);
//      }

//      [TestMethod]
//      public void CheckCurrentItem() {
//         Assert.AreSame(_selectableStatus1, GetSelectedItem(PersonVM.Descriptor.Status).Source);
//      }

//      [TestMethod]
//      public void CheckCurrentItemNull() {
//         _person.CurrentStatus = null;
//         Assert.IsNull(GetSelectedItem(PersonVM.Descriptor.Status));
//         Assert.IsNull(GetSelectedItem(PersonVM.Descriptor.StatusUnfiltered));
//      }

//      [TestMethod]
//      public void UpdateCurrentItem() {
//         var singleSelection = _vm.InvokeGetValue(PersonVM.Descriptor.Status);
//         var secondItemVM = singleSelection.AllItems.ElementAt(1);
//         singleSelection.SelectedItem = secondItemVM;
//         Assert.AreSame(secondItemVM.Source, _person.CurrentStatus);
//      }

//      [TestMethod]
//      public void UpdateCurrentItemToNull() {
//         var singleSelection = _vm.InvokeGetValue(PersonVM.Descriptor.Status);
//         singleSelection.SelectedItem = null;
//         Assert.IsNull(_person.CurrentStatus);
//      }

//      [TestMethod]
//      public void CheckForChangedNotification() {
//         var singleSelection = _vm.InvokeGetValue(PersonVM.Descriptor.Status);
//         var secondItemVM = singleSelection.AllItems.ElementAt(1);
//         AssertHelper.AssertPropertyChangedEvent(
//            singleSelection,
//            x => x.SelectedItem,
//            secondItemVM,
//            () => { singleSelection.SelectedItem = secondItemVM; }
//         );
//      }

//      [TestMethod]
//      public void Refresh() {
//         var singleSelection = _vm.InvokeGetValue(PersonVM.Descriptor.Status);

//         Assert.AreSame(
//            _selectableStatus1,
//            GetSelectedItem(PersonVM.Descriptor.Status).Source
//         );

//         _person.CurrentStatus = _selectableStatus2;
//         _allStatus.Remove(_selectableStatus1);

//         var selectedItemCounter = new PropertyChangedCounter(singleSelection, "SelectedItem");
//         var allItemsCounter = new PropertyChangedCounter(singleSelection, "AllItems");
//         bool listChangedWasInvoked = false;
//         ((IBindingList)singleSelection.AllItems).ListChanged += (_, __) => {
//            listChangedWasInvoked = true;
//         };

//         singleSelection.Refresh();

//         allItemsCounter.AssertNoRaise(); // BindingList has list changed notification
//         selectedItemCounter.AssertOneRaise();
//         Assert.IsTrue(listChangedWasInvoked);

//         Assert.AreSame(
//            _selectableStatus2,
//            GetSelectedItem(PersonVM.Descriptor.Status).Source
//         );

//         var actualSourceItems = GetAllItems(PersonVM.Descriptor.Status)
//          .Select(x => x.Source)
//          .ToArray();

//         var expected = new PersonStatus[] { _selectableStatus2 };

//         CollectionAssert.AreEquivalent(expected, actualSourceItems);

//         throw new NotImplementedException();
//         //Assert.IsTrue(singleSelection.IsValid(false));
//      }

//      private IVMCollection<SelectionItemVM<PersonStatus>> GetAllItems(
//         SingleSelectionProperty<PersonStatus> singleSelectionProperty
//      ) {
//         return _vm.InvokeGetValue(singleSelectionProperty).AllItems;
//      }

//      private SelectionItemVM<PersonStatus> GetSelectedItem(
//         SingleSelectionProperty<PersonStatus> singleSelectionProperty
//      ) {
//         return _vm.InvokeGetValue(singleSelectionProperty).SelectedItem;
//      }

//      private sealed class PersonVM : ViewModel<PersonVMDescriptor>, ICanInitializeFrom<Person> {
//         public static readonly PersonVMDescriptor ClassDescriptor = VMDescriptorBuilder
//            .OfType<>().For<PersonVM>()
//            .WithProperties((d, c) => {
//               var vm = c.GetPropertyBuilder();
//               var p = c.GetPropertyBuilder(x => x.Person);

//               return new PersonVMDescriptor {
//                  Status = vm.SingleSelection()
//                     .WithItems(x => x.AllStatus, x => x.IsSelectable)
//                     .WithSelection(x => x.Person.CurrentStatus)
//                     .Of(i => new StatusSelectionItemVMDescriptor {
//                        Name = i.Property.MapsTo(x => x.Name),
//                        Description = i.Property.MapsTo(x => x.Description)
//                     }),
//                  StatusUnfiltered = vm.SingleSelection()
//                     .WithItems(x => x.AllStatus)
//                     .WithSelection(x => x.Person.CurrentStatus)
//                     .Of(i => new StatusSelectionItemVMDescriptor {
//                        Name = i.Property.MapsTo(x => x.Name),
//                        Description = i.Property.MapsTo(x => x.Description)
//                     })
//               };
//            })
//            .Build();

//         public PersonVM(IEnumerable<PersonStatus> allStatus)
//            : base() {
//            AllStatus = allStatus;
//         }

//         public Person Person { get; private set; }

//         public T InvokeGetValue<T>(VMPropertyBase<T> property) {
//            return GetValue(property);
//         }

//         //public SingleSelectionVM<PersonStatus, SelectionItemVM<PersonStatus>> UnfilteredStatusHelper {
//         //   get { return GetValue(Descriptor.StatusUnfiltered); }
//         //}
//         //public SingleSelectionVM<PersonStatus, SelectionItemVM<PersonStatus>> StatusHelper {
//         //   get { return GetValue(Descriptor.Status); }
//         //}

//         private IEnumerable<PersonStatus> AllStatus { get; set; }

//         public void InitializeFrom(Person source) {
//            Person = source;
//         }
//      }

//      private sealed class PersonVMDescriptor : VMDescriptor {
//         public SingleSelectionProperty<PersonStatus> Status { get; set; }
//         public SingleSelectionProperty<PersonStatus> StatusUnfiltered { get; set; }
//      }

//      private class Person {
//         public PersonStatus CurrentStatus { get; set; }
//      }

//      private class PersonStatus {
//         public string Name { get; set; }
//         public string Description { get; set; }
//         public bool IsSelectable { get; set; }
//      }

//      private class StatusSelectionItemVMDescriptor : VMDescriptor {
//         public VMProperty<string> Name { get; set; }
//         public VMProperty<string> Description { get; set; }
//      }
//   }
//}