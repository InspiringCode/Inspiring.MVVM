namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class PersonVM : ViewModel<PersonVMDescriptor>, ICanInitializeFrom<Person> {
      public static readonly PersonVMDescriptor Descriptor = VMDescriptorBuilder
            .For<PersonVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyBuilder();
               var p = c.GetPropertyBuilder(x => x.Person);

               return new PersonVMDescriptor {
                  FirstName = p.Property.MapsTo(x => x.FirstName),
                  LastName = p.Property.MapsTo(x => x.LastName),
                  BirthDate = p.Property.MapsTo(x => x.BirthDate),
                  Salary = p.Property.DelegatesTo(x => x.Salary, (x, val) => x.Salary = val),
                  Name = p.Property.DelegatesTo(x => String.Format("{0} {1}", x.FirstName, x.LastName)),
                  IsSelected = v.Property.Of<bool>(),
                  Projects = p.Collection.Wraps(x => x.Projects).With<ProjectVM>(PersonVM.Descriptor),
                  CurrentProject = v.Property.Of<ProjectVM>()
               };
            })
         //.WithValidations((d, c) => {
         //})
         //.WithDependencies((d, c) => {
         //})
         //.WithBehaviors((d, c) => {
         //})
            .Build();

      public PersonVM()
         : base() {
      }

      public PersonVM(Person person)
         : this() {
         Person = person;
      }

      public Person Person { get; set; }

      public IEnumerable<Project> Projects { get; set; }

      public void InitializeFrom(Person source) {
         Person = source;
      }
   }

   public class PersonVMDescriptor : VMDescriptor {
      public VMProperty<string> Name { get; set; }
      public VMProperty<string> FirstName { get; set; }
      public VMProperty<string> LastName { get; set; }
      public VMProperty<DateTime> BirthDate { get; set; }
      public VMProperty<decimal> Salary { get; set; }
      public VMProperty<bool> IsSelected { get; set; }
      public VMProperty<ProjectVM> CurrentProject { get; set; }
      public VMProperty<IVMCollection<ProjectVM>> Projects { get; set; }
   }

   public class ProjectVM : ViewModel<ProjectVMDescriptor>, ICanInitializeFrom<Project> {
      public static readonly ProjectVMDescriptor Descriptor = VMDescriptorBuilder
            .For<ProjectVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyBuilder();

               return new ProjectVMDescriptor {
                  Name = v.Property.MapsTo(x => x.Project.Name)
               };
            })
         //.WithValidations((d, c) => {
         //})
         //.WithDependencies((d, c) => {
         //})
         //.WithBehaviors((d, c) => {
         //})
            .Build();

      public ProjectVM()
         : base() {
      }

      public ProjectVM(Project project)
         : this() {
         Project = project;
      }

      public Project Project { get; set; }

      public void InitializeFrom(Project source) {
         Project = source;
      }
   }

   public class ProjectVMDescriptor : VMDescriptor {
      public VMProperty<string> Name { get; set; }
      public VMProperty<int> MemberCount { get; set; }
   }

   public class Person {
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public DateTime BirthDate { get; set; }
      public decimal Salary { get; set; }
      public ICollection<Project> Projects { get; set; }
      public Project CurrentProject { get; set; }
   }

   public class Project {
      public string Name { get; set; }
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
