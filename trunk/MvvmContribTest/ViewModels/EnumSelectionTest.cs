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

      private SingleSelectionVM<PersonStatus> GetSelection() {
         return _vm.InvokeGetValue(PersonVM.ClassDescriptor.Status);
      }

      private IVMCollection<SelectionItemVM<PersonStatus>> GetAllItems() {
         return GetSelection().AllItems;
      }

      private SelectionItemVM<PersonStatus> GetSelectedItem() {
         return GetSelection().SelectedItem;
      }

      private string GetCaption(SelectionItemVM<PersonStatus> item) {
         return (string)TypeDescriptor.GetProperties(item)["Caption"].GetValue(item);
      }

      private sealed class PersonVM : ViewModel<PersonVMDescriptor>, IHasSourceObject<Person> {
         public static readonly PersonVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<PersonVMDescriptor>()
            .For<PersonVM>()
            .WithProperties((d, c) => {
               var vm = c.GetPropertyBuilder();
               var p = c.GetPropertyBuilder(x => x.Person);

               d.Status = p.EnumSelection(x => x.CurrentStatus);
            })
            .Build();

         public PersonVM()
            : base(ClassDescriptor) {
         }

         public Person Person { get; private set; }

         public T InvokeGetValue<T>(IVMPropertyDescriptor<T> property) {
            return GetValue(property);
         }

         public void InitializeFrom(Person source) {
            Person = source;
         }

         Person IHasSourceObject<Person>.Source {
            get { return Person; }
            set { Person = value; }
         }
      }

      private sealed class PersonVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<SingleSelectionVM<PersonStatus>> Status { get; set; }
      }

      private class Person {
         public PersonStatus CurrentStatus { get; set; }
      }
   }

   internal class StatusSelectionItemVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Name { get; set; }

   }
}