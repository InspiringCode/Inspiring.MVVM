using Inspiring.Mvvm.ViewModels;
namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ViewModelsTest {
      private Person _person;
      private PersonVM _vm;

      [TestInitialize]
      public void Setup() {
         _person = new Person();
         _vm = new PersonVM(_person);
      }

      [TestMethod]
      public void CheckDefaultValues() {
         Assert.IsNull(_vm.FirstName);
         Assert.IsNull(_vm.LastName);
         Assert.AreEqual(" ", _vm.Name);
         Assert.AreEqual(default(DateTime), _vm.BirthDate);
         Assert.AreEqual(default(decimal), _vm.Salary);
         Assert.AreEqual(default(bool), _vm.IsSelected);
      }

      private class PersonVM : ViewModel<PersonVMDescriptor> {
         public static readonly PersonVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<PersonVMDescriptor>()
            .For<PersonVM>()
            .WithProperties((d, pro) => {
               var v = pro.GetPropertyBuilder();
               var p = pro.GetPropertyBuilder(x => x.Person);

               d.FirstName = p.Property.MapsTo(x => x.FirstName);
               d.LastName = p.Property.MapsTo(x => x.LastName);
               d.BirthDate = p.Property.MapsTo(x => x.BirthDate);
               d.Salary = p.Property.MapsTo(x => x.Salary);
               d.Name = p.Property.DelegatesTo(x => String.Format("{0} {1}", x.FirstName, x.LastName));
               d.IsSelected = v.Property.Of<bool>();
            })
            .Build();

         public PersonVM(Person person)
            : base(ClassDescriptor) {
            Person = person;
         }

         public Person Person { get; private set; }

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
