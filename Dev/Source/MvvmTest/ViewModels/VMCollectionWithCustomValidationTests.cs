namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class VMCollectionWithCustomValidationTests : TestBase {

      private PersonListeVM _viewModel;

      [TestInitialize]
      public void Setup() {

         var persons = new Person[2] { 
            new Person() { FirstName = "FirstName1", LastName = "LastName1" },
            new Person() { FirstName = "FirstName2", LastName = "LastName2" }
         };

         _viewModel = new PersonListeVM(persons);
      }

      /// <summary>
      /// This integration test was written to reproduce a StackOverflowException.
      /// </summary>
      [TestMethod]
      public void AccessVMCollection_withViewModelValidationThatAccessesTheVMCollection_doesNotThrowStackOverflowException() {

         _viewModel.PersonListe.Contains(null);
      }

      private class PersonListeVM :
         ViewModel<PersonListeVMDescriptor> {

         public static PersonListeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<PersonListeVMDescriptor>()
            .For<PersonListeVM>()
            .WithProperties((d, b) => {
               var vm = b.GetPropertyBuilder();

               d.PersonListe = vm.Collection
                  .Wraps(x => x.PersonListeObjects)
                  .With<PersonVM>(PersonVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.CheckViewModel((vm, args) => {
                  if (vm.PersonListe.Count == 0) {
                     ;
                  }
               });
            })
            .Build();

         public IVMCollection<PersonVM> PersonListe {
            get { return GetValue(Descriptor.PersonListe); }
         }

         public IEnumerable<Person> PersonListeObjects { get; private set; }

         public PersonListeVM(IEnumerable<Person> persons)
            : base(ClassDescriptor) {
            PersonListeObjects = persons;
         }
      }

      private class PersonListeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<PersonVM>> PersonListe { get; set; }
      }

      private class PersonVM : DefaultViewModelWithSourceBase<PersonVMDescriptor, Person> {

         public static readonly PersonVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<PersonVMDescriptor>()
            .For<PersonVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               var p = b.GetPropertyBuilder(x => x.Source);

               d.FirstName = p.Property.MapsTo(x => x.FirstName);
               d.LastName = p.Property.MapsTo(x => x.LastName);
               d.BirthDate = p.Property.MapsTo(x => x.BirthDate);
               d.Salary = p.Property.MapsTo(x => x.Salary);
               d.Name = p.Property.DelegatesTo(x => String.Format("{0} {1}", x.FirstName, x.LastName),
                                               (x, val) => { });
               d.IsSelected = v.Property.Of<bool>();
            })
            .WithValidators(b => {
               b.EnableParentValidation();
            })
            .Build();

         public PersonVM()
            : base(ClassDescriptor) {

         }

         #region Helper properties for unit tests

         public string Name {
            get { return GetValue(Descriptor.Name); }
            set { SetValue(Descriptor.Name, value); }
         }

         public string FirstName {
            get { return GetValue(Descriptor.FirstName); }
            set { SetValue(Descriptor.FirstName, value); }
         }

         public string LastName {
            get { return GetValue(Descriptor.LastName); }
            set { SetValue(Descriptor.LastName, value); }
         }

         public DateTime BirthDate {
            get { return GetValue(Descriptor.BirthDate); }
            set { SetValue(Descriptor.BirthDate, value); }
         }

         public decimal Salary {
            get { return GetValue(Descriptor.Salary); }
            set { SetValue(Descriptor.Salary, value); }
         }

         public bool IsSelected {
            get { return GetValue(Descriptor.IsSelected); }
            set { SetValue(Descriptor.IsSelected, value); }
         }

         #endregion
      }

      private class PersonVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<string> FirstName { get; set; }
         public IVMPropertyDescriptor<string> LastName { get; set; }
         public IVMPropertyDescriptor<DateTime> BirthDate { get; set; }
         public IVMPropertyDescriptor<decimal> Salary { get; set; }
         public IVMPropertyDescriptor<bool> IsSelected { get; set; }
      }

      private class Person {
         public string FirstName { get; set; }
         public string LastName { get; set; }
         public DateTime BirthDate { get; set; }
         public decimal Salary { get; set; }
      }
   }
}