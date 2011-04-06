namespace Inspiring.MvvmContribTest.ViewModels {
   using System;
   using System.ComponentModel;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class EnumSelectionTest : TestBase {
      private Person _person;
      private PersonVM _vm;
      private PersonStatus[] _allStatus = new PersonStatus[] {
         PersonStatus.None,
         PersonStatus.Active,
         PersonStatus.Inactive,
         PersonStatus.Dismissed
      };



      [TestInitialize]
      public void Setup() {
         EnumLocalizer.AddLocalizationResource(EnumLocalizations.ResourceManager);

         _person = new Person();
         _person.CurrentStatus = default(PersonStatus);

         _vm = new PersonVM();
         _vm.InitializeFrom(_person);
      }

      [TestMethod]
      public void CheckSourceOfItems() {
         var allSourceItems = GetAllItems().Select(x => x.Source).ToArray();
         CollectionAssert.AreEqual(_allStatus, allSourceItems);
      }

      [TestMethod]
      public void CheckDefaultValue() {
         var selectedItem = GetSelectedItem().Source;
         Assert.AreEqual(default(PersonStatus), selectedItem);
      }

      [TestMethod]
      public void UpdateSelectedItem() {
         var thirdItem = GetAllItems()[2];
         GetSelection().SelectedItem = thirdItem;
         Assert.AreEqual(PersonStatus.Inactive, _person.CurrentStatus);
      }

      [TestMethod]
      public void CheckCaptions() {
         string[] captions = GetAllItems().Select(x => GetCaption(x)).ToArray();
         CollectionAssert.AreEqual(
            new string[] { 
               EnumLocalizations.PersonStatus_None, 
               EnumLocalizations.PersonStatus_Active,
               EnumLocalizations.PersonStatus_Inactive,
               EnumLocalizations.PersonStatus_Dismissed
            },
            captions
         );
      }

      [TestMethod]
      public void CheckWithNonExistingItem() {
         _person.CurrentStatus = (PersonStatus)99;
         _vm.Refresh(PersonVM.ClassDescriptor.Status);
         PersonStatus[] status = GetAllItems().Select(x => x.Source).ToArray();
         CollectionAssert.AreEqual(
            new PersonStatus[] { 
               PersonStatus.None,
               PersonStatus.Active,
               PersonStatus.Inactive,
               PersonStatus.Dismissed,
               (PersonStatus)99
            },
            status
         );
         Assert.IsFalse(_vm.IsValid);
      }

      [TestMethod]
      public void CheckFilter() {
         PersonStatus[] status = GetAllFilteredItems().Select(x => x.Source).ToArray();
         CollectionAssert.AreEqual(
            new PersonStatus[] { 
               PersonStatus.None,
               PersonStatus.Active,
               PersonStatus.Inactive
            },
            status
         );
         Assert.IsTrue(_vm.IsValid);
      }

      [TestMethod]
      public void CheckFilterWithSelectedItem() {
         _person.CurrentStatus = PersonStatus.Dismissed;
         _vm.Refresh(PersonVM.ClassDescriptor.FilteredStatus);
         PersonStatus[] status = GetAllFilteredItems().Select(x => x.Source).ToArray();
         CollectionAssert.AreEqual(
            new PersonStatus[] { 
               PersonStatus.None,
               PersonStatus.Active,
               PersonStatus.Inactive,
               PersonStatus.Dismissed
            },
            status
         );
         Assert.IsTrue(_vm.IsValid);
      }

      [TestMethod]
      public void CheckNullableSelection() {
         Nullable<PersonStatus>[] allItems = GetAllNullableItems().Select(x => x.Source).ToArray();
         CollectionAssert.AreEqual(
            new Nullable<PersonStatus>[] {
               PersonStatus.None,
               PersonStatus.Active,
               PersonStatus.Inactive,
               PersonStatus.Dismissed
            },
            allItems
         );
      }

      [TestMethod]
      public void CheckNullSelectionOfNullable() {
         GetNullableSelection().SelectedItem = null;
         Assert.AreEqual(null, _person.CurrentNullableStatus);
      }

      [TestMethod]
      public void CheckNullableCaptions() {
         string[] captions = GetAllNullableItems().Select(x => GetCaptionOfNullable(x)).ToArray();
         CollectionAssert.AreEqual(
            new string[] { 
               EnumLocalizations.PersonStatus_None, 
               EnumLocalizations.PersonStatus_Active,
               EnumLocalizations.PersonStatus_Inactive,
               EnumLocalizations.PersonStatus_Dismissed
            },
            captions
         );
      }

      [TestMethod]
      public void CheckNullableCaptionsSelectedItemIsNull() {
         GetNullableSelection().SelectedItem = null;
         string[] captions = GetAllNullableItems().Select(x => GetCaptionOfNullable(x)).ToArray();
         CollectionAssert.AreEqual(
            new string[] { 
               EnumLocalizations.PersonStatus_None, 
               EnumLocalizations.PersonStatus_Active,
               EnumLocalizations.PersonStatus_Inactive,
               EnumLocalizations.PersonStatus_Dismissed
            },
            captions
         );
      }

      private IVMCollection<SelectionItemVM<Nullable<PersonStatus>>> GetAllNullableItems() {
         return GetNullableSelection().AllItems;
      }

      private SingleSelectionVM<Nullable<PersonStatus>> GetNullableSelection() {
         return _vm.InvokeGetValue(PersonVM.ClassDescriptor.NullableStatus);
      }

      private string GetCaptionOfNullable(SelectionItemVM<Nullable<PersonStatus>> item) {
         return (string)TypeDescriptor.GetProperties(item)["Caption"].GetValue(item);
      }

      private SingleSelectionVM<PersonStatus> GetSelection() {
         return _vm.InvokeGetValue(PersonVM.ClassDescriptor.Status);
      }

      private SingleSelectionVM<PersonStatus> GetFilteredSelection() {
         return _vm.InvokeGetValue(PersonVM.ClassDescriptor.FilteredStatus);
      }

      private IVMCollection<SelectionItemVM<PersonStatus>> GetAllItems() {
         return GetSelection().AllItems;
      }

      private IVMCollection<SelectionItemVM<PersonStatus>> GetAllFilteredItems() {
         return GetFilteredSelection().AllItems;
      }

      private SelectionItemVM<PersonStatus> GetSelectedItem() {
         return GetSelection().SelectedItem;
      }

      private string GetCaption(SelectionItemVM<PersonStatus> item) {
         return (string)TypeDescriptor.GetProperties(item)["Caption"].GetValue(item);
      }

      private sealed class PersonVM : DefaultViewModelWithSourceBase<PersonVMDescriptor, Person> {
         public static readonly PersonVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<PersonVMDescriptor>()
            .For<PersonVM>()
            .WithProperties((d, c) => {
               var vm = c.GetPropertyBuilder();
               var p = c.GetPropertyBuilder(x => x.Source);

               d.Status = p.EnumSelection(x => x.CurrentStatus);

               d.NullableStatus = p.EnumSelection(x => x.CurrentNullableStatus);

               d.FilteredStatus = p
                  .SingleSelection(x => x.CurrentStatus)
                  .WithItems(x => GetEnumValues<PersonStatus>())
                  .WithFilter(x => x != PersonStatus.Dismissed)
                  .WithCaption(x => EnumLocalizer.GetCaption(x));
            })
            .Build();

         public PersonVM()
            : base(ClassDescriptor) {
         }

         public T InvokeGetValue<T>(IVMPropertyDescriptor<T> property) {
            return GetValue(property);
         }

         public new void Refresh(IVMPropertyDescriptor property) {
            base.Refresh(property);
         }

         private static TEnum[] GetEnumValues<TEnum>() {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
         }
      }

      private sealed class PersonVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<SingleSelectionVM<PersonStatus>> Status { get; set; }
         public IVMPropertyDescriptor<SingleSelectionVM<PersonStatus>> FilteredStatus { get; set; }
         public IVMPropertyDescriptor<SingleSelectionVM<Nullable<PersonStatus>>> NullableStatus { get; set; }
      }

      private class Person {
         public PersonStatus CurrentStatus { get; set; }
         public Nullable<PersonStatus> CurrentNullableStatus { get; set; }
      }
   }
}