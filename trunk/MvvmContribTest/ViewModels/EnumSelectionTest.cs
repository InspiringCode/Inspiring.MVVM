namespace Inspiring.MvvmContribTest.ViewModels {
   using System.ComponentModel;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class EnumSelectionTest {
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
         var allSourceItems = GetAllItems().Select(x => x.SourceItem).ToArray();
         CollectionAssert.AreEqual(_allStatus, allSourceItems);
      }

      [TestMethod]
      public void CheckDefaultValue() {
         var selectedItem = GetSelectedItem().SourceItem;
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

      private SingleSelectionVM<PersonStatus, SelectionItemVM<PersonStatus>> GetSelection() {
         return _vm.InvokeGetValue(PersonVM.Descriptor.Status);
      }

      private VMCollection<SelectionItemVM<PersonStatus>> GetAllItems() {
         return GetSelection().AllItems;
      }

      private SelectionItemVM<PersonStatus> GetSelectedItem() {
         return GetSelection().SelectedItem;
      }

      private string GetCaption(SelectionItemVM<PersonStatus> item) {
         return (string)TypeDescriptor.GetProperties(item)["Caption"].GetValue(item);
      }

      private sealed class PersonVM : ViewModel<PersonVMDescriptor>, ICanInitializeFrom<Person> {
         public static readonly PersonVMDescriptor Descriptor = VMDescriptorBuilder
            .For<PersonVM>()
            .CreateDescriptor(c => {
               var vm = c.GetPropertyFactory();
               var p = c.GetPropertyFactory(x => x.Person);

               return new PersonVMDescriptor {
                  Status = vm.EnumSelection().Mapped(x => x.Person.CurrentStatus)
               };
            })
            .Build();

         public PersonVM()
            : base(Descriptor) {
         }

         public Person Person { get; private set; }

         public T InvokeGetValue<T>(VMPropertyBase<T> property) {
            return GetValue(property);
         }

         public void InitializeFrom(Person source) {
            Person = source;
         }
      }

      private sealed class PersonVMDescriptor : VMDescriptor {
         public SingleSelectionProperty<PersonStatus> Status { get; set; }
      }

      private class Person {
         public PersonStatus CurrentStatus { get; set; }
      }
   }

   internal class StatusSelectionItemVMDescriptor : VMDescriptor {
      public VMProperty<string> Name { get; set; }

   }
}