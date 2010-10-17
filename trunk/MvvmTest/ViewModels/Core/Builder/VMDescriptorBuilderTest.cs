namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class VMDescriptorBuilderTest {
      private PersonVMDescriptor _descriptor;
      private CompanyVMDescriptor _companyDescriptor;

      [TestInitialize]
      public void Setup() {
         _descriptor = PersonVM.Descriptor;
         _companyDescriptor = CompanyVM.Descriptor;
      }

      [TestMethod]
      public void CheckPropertiesNotNull() {
         Assert.IsNotNull(_descriptor.Name);
         Assert.IsNotNull(_descriptor.BirthDate);
         Assert.IsNotNull(_descriptor.Salary);
         Assert.IsNotNull(_descriptor.IsSelected);
         Assert.IsNotNull(_companyDescriptor.Employees);
         Assert.IsNotNull(_companyDescriptor.Customers);
      }

      [TestMethod]
      public void CheckPropertyNames() {
         Assert.AreEqual("Name", _descriptor.Name.PropertyName);
         Assert.AreEqual("BirthDate", _descriptor.BirthDate.PropertyName);
         Assert.AreEqual("Salary", _descriptor.Salary.PropertyName);
         Assert.AreEqual("IsSelected", _descriptor.IsSelected.PropertyName);
         Assert.AreEqual("Employees", _companyDescriptor.Employees.PropertyName);
         Assert.AreEqual("Customers", _companyDescriptor.Customers.PropertyName);
      }

      // [TestMethod] // TODO
      public void CheckAccessorBehaviors() {
         CalculatedPropertyBehavior<Person, string> calculatedStringBehavior;
         MappedPropertyBehavior<PersonVM, DateTime> mappedDateTimeBehavior;
         MappedPropertyBehavior<PersonVM, decimal> mappedDecimalBehavior;
         InstancePropertyBehavior<bool> boolInstanceBehavior;
         CollectionPopulatorBehavior<PersonVM, PersonVM, Person> collectionPopulator;

         Assert.IsTrue(_descriptor.Name.Behaviors.TryGetBehavior(out calculatedStringBehavior));
         Assert.IsTrue(_descriptor.BirthDate.Behaviors.TryGetBehavior(out mappedDateTimeBehavior));
         Assert.IsTrue(_descriptor.Salary.Behaviors.TryGetBehavior(out mappedDecimalBehavior));
         Assert.IsTrue(_descriptor.IsSelected.Behaviors.TryGetBehavior(out boolInstanceBehavior));
         Assert.IsTrue(_companyDescriptor.Employees.Behaviors.TryGetBehavior(out collectionPopulator));
         Assert.IsTrue(_companyDescriptor.Customers.Behaviors.TryGetBehavior(out collectionPopulator));
      }

      [TestMethod]
      public void CheckCollectionProperty() {
         CompanyVM compVM = new CompanyVM(
            new Company {
               Employees = new List<Person> {
                  new Person { FirstName = "First" },
                  new Person { FirstName = "Second" }
               },
               Customers = new List<Person> {
                  new Person { FirstName = "Thrid" }
               }
            }
         );

         var emps = compVM.GetValue(_companyDescriptor.Employees);
         var custs = compVM.GetValue(_companyDescriptor.Customers);

         Assert.IsNotNull(emps);
         Assert.IsNotNull(custs);

         Assert.AreEqual(2, emps.Count);
         Assert.AreEqual(1, custs.Count);

         Assert.AreEqual(compVM.Company.Employees.ElementAt(1), emps[1].Person);
      }

      private class CompanyVM : ViewModel<CompanyVMDescriptor> {
         public static readonly CompanyVMDescriptor Descriptor = VMDescriptorBuilder
            .For<CompanyVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyFactory();
               var com = c.GetPropertyFactory(x => x.Company);

               return new CompanyVMDescriptor {
                  Employees = v.MappedCollection(x => x.Company.Employees).Of<PersonVM>(PersonVM.Descriptor),
                  Customers = com.MappedCollection(x => x.Customers).Of<PersonVM>(PersonVM.Descriptor)
               };
            })
            .Build();

         public CompanyVM(Company company)
            : base(Descriptor) {
            Company = company;
         }

         public Company Company { get; set; }
      }

      private class CompanyVMDescriptor : VMDescriptor {
         public VMCollectionProperty<PersonVM> Employees { get; set; }
         public VMCollectionProperty<PersonVM> Customers { get; set; }
      }

      private class PersonVM : ViewModel<PersonVMDescriptor>, ICanInitializeFrom<Person> {
         public static readonly PersonVMDescriptor Descriptor = VMDescriptorBuilder
            .For<PersonVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyFactory();
               var p = c.GetPropertyFactory(x => x.Person);

               return new PersonVMDescriptor {
                  BirthDate = p.Mapped(x => x.BirthDate),
                  Salary = p.Mapped(x => x.Salary),
                  Name = p.Calculated(x => String.Format("{0} {1}", x.FirstName, x.LastName)),
                  IsSelected = v.Local<bool>()
               };
            })
            .WithValidations((d, c) => {

            })
            .WithDependencies((d, c) => {

            })
            .WithBehaviors((d, c) => {
            })
            .Build();
         public PersonVM()
            : base(Descriptor) {

         }
         public Person Person { get; set; }

         public void InitializeFrom(Person source) {
            Person = source;
         }
      }

      private class PersonVMDescriptor : VMDescriptor {
         public VMProperty<string> Name { get; set; }
         public VMProperty<DateTime> BirthDate { get; set; }
         public VMProperty<decimal> Salary { get; set; }
         public VMProperty<bool> IsSelected { get; set; }
      }
      private class Person {
         public string FirstName { get; set; }
         public string LastName { get; set; }
         public DateTime BirthDate { get; set; }
         public decimal Salary { get; set; }
      }

      private class Company {
         public IEnumerable<Person> Employees { get; set; }
         public IEnumerable<Person> Customers { get; set; }
      }
   }
}
