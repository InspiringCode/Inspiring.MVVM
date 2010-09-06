using System;
using Inspiring.Mvvm.ViewModels;
using Inspiring.Mvvm.ViewModels.Behaviors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.ViewModels {
   [TestClass]
   public class VMDescriptorBuilderTest {
      private PersonVMDescriptor _descriptor;

      [TestInitialize]
      public void Setup() {
         _descriptor = VMDescriptorBuilder
            .For<PersonVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyFactory();
               var p = c.GetPropertyFactory(x => x.Person);

               return new PersonVMDescriptor {
                  BirthDate = p.Mapped(x => x.BirthDate),
                  Salary = p.Mapped(x => x.Salary),
                  Name = p.Calculated(x => String.Format("{0} {1}", x.FirstName, x.LastName)),
                  IsSelected = v.Simple<bool>()
               };
            })
            .WithValidations((d, c) => {

            })
            .WithDependencies((d, c) => {

            })
            .WithBehaviors((d, c) => {
               c.EnableBehavior(VMBehaviors.DisconnectedViewModel);
            })
            .Build();
      }

      [TestMethod]
      public void CheckPropertiesNotNull() {
         Assert.IsNotNull(_descriptor.Name);
         Assert.IsNotNull(_descriptor.BirthDate);
         Assert.IsNotNull(_descriptor.Salary);
         Assert.IsNotNull(_descriptor.IsSelected);
      }

      [TestMethod]
      public void CheckPropertyNames() {
         Assert.AreEqual("Name", _descriptor.Name.PropertyName);
         Assert.AreEqual("BirthDate", _descriptor.BirthDate.PropertyName);
         Assert.AreEqual("Salary", _descriptor.Salary.PropertyName);
         Assert.AreEqual("IsSelected", _descriptor.IsSelected.PropertyName);
      }

      [TestMethod]
      public void CheckAccessorBehaviors() {
         CalculatedPropertyBehavior<Person, string> calculatedStringBehavior;
         MappedPropertyBehavior<PersonVM, DateTime> mappedDateTimeBehavior;
         MappedPropertyBehavior<PersonVM, decimal> mappedDecimalBehavior;
         InstancePropertyBehavior<bool> boolInstanceBehavior;

         Assert.IsTrue(_descriptor.Name.TryGetBehavior(out calculatedStringBehavior));
         Assert.IsTrue(_descriptor.BirthDate.TryGetBehavior(out mappedDateTimeBehavior));
         Assert.IsTrue(_descriptor.Salary.TryGetBehavior(out mappedDecimalBehavior));
         Assert.IsTrue(_descriptor.IsSelected.TryGetBehavior(out boolInstanceBehavior));
      }

      private class PersonVM : ViewModel<PersonVMDescriptor> {
         public PersonVM()
            : base(null) {

         }
         public Person Person { get; set; }
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
   }
}
