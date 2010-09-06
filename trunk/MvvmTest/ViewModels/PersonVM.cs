namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels;

   internal class PersonVM : ViewModel<PersonVMDescriptor> {
      public static readonly PersonVMDescriptor Descriptor = VMDescriptorBuilder
            .For<PersonVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyFactory();
               var p = c.GetPropertyFactory(x => x.Person);

               return new PersonVMDescriptor {
                  FirstName = p.Mapped(x => x.FirstName),
                  LastName = p.Mapped(x => x.LastName),
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
            })
            .Build();

      public PersonVM(Person person)
         : base(Descriptor) {
         Person = person;
      }

      public Person Person { get; set; }
   }

   internal class PersonVMDescriptor : VMDescriptor {
      public VMProperty<string> Name { get; set; }
      public VMProperty<string> FirstName { get; set; }
      public VMProperty<string> LastName { get; set; }
      public VMProperty<DateTime> BirthDate { get; set; }
      public VMProperty<decimal> Salary { get; set; }
      public VMProperty<bool> IsSelected { get; set; }
   }

   internal class Person {
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public DateTime BirthDate { get; set; }
      public decimal Salary { get; set; }
   }

   internal class SampleDataFactory {
      public const int PersonVMPropertyCount = 6;

      public static Person CreatePerson(string firstName = null, string lastName = null) {
         return new Person {
            FirstName = firstName ?? "John",
            LastName = lastName ?? "Smith",
            BirthDate = new DateTime(1980, 1, 20),
            Salary = 2000
         };
      }

      public static PersonVM CreatePersonVM(Person sourceObject = null) {
         sourceObject = sourceObject ?? CreatePerson();
         return new PersonVM(sourceObject);
      }
   }
}
