namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class PersonVM : ViewModel<PersonVMDescriptor>, ICanInitializeFrom<Person> {
      public static readonly PersonVMDescriptor Descriptor = VMDescriptorBuilder
            .For<PersonVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyFactory();
               var p = c.GetPropertyFactory(x => x.Person);

               return new PersonVMDescriptor {
                  FirstName = p.Mapped(x => x.FirstName).Property(),
                  LastName = p.Mapped(x => x.LastName).Property(),
                  BirthDate = p.Mapped(x => x.BirthDate).Property(),
                  Salary = p.Calculated(x => x.Salary, (x, val) => x.Salary = val).Property(),
                  Name = p.Calculated(x => String.Format("{0} {1}", x.FirstName, x.LastName)).Property(),
                  IsSelected = v.Local.Property<bool>(),
                  Projects = p.Collection().Wraps(x => x.Projects).Of<ProjectVM>(PersonVM.Descriptor),
                  CurrentProject = v.Local.Property<ProjectVM>()
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
               var v = c.GetPropertyFactory();

               return new ProjectVMDescriptor {
                  Name = v.Mapped(x => x.Project.Name).Property()
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
