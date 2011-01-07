namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class PersonVM : ViewModel<PersonVMDescriptor>, ICanInitializeFrom<Person> {
      public static readonly PersonVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<PersonVMDescriptor>()
            .For<PersonVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               var p = c.GetPropertyBuilder(x => x.Person);

               d.FirstName = p.Property.MapsTo(x => x.FirstName);
               d.LastName = p.Property.MapsTo(x => x.LastName);
               d.BirthDate = p.Property.MapsTo(x => x.BirthDate);
               d.Salary = p.Property.DelegatesTo(x => x.Salary, (x, val) => x.Salary = val);
               d.Name = p.Property.DelegatesTo(x => String.Format("{0} {1}", x.FirstName, x.LastName));
               d.IsSelected = v.Property.Of<bool>();
               d.Projects = p.Collection.Wraps(x => x.Projects).With<ProjectVM>(PersonVM.ClassDescriptor);
               d.CurrentProject = v.Property.Of<ProjectVM>();
            })
         //.WithValidators(c => {
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
      public IVMProperty<string> Name { get; set; }
      public IVMProperty<string> FirstName { get; set; }
      public IVMProperty<string> LastName { get; set; }
      public IVMProperty<DateTime> BirthDate { get; set; }
      public IVMProperty<decimal> Salary { get; set; }
      public IVMProperty<bool> IsSelected { get; set; }
      public IVMProperty<ProjectVM> CurrentProject { get; set; }
      public IVMProperty<IVMCollection<ProjectVM>> Projects { get; set; }
   }

   public class ProjectVM : ViewModel<ProjectVMDescriptor>, IVMCollectionItem<Project> {
      public static readonly ProjectVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ProjectVMDescriptor>()
            .For<ProjectVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();

               d.Name = v.Property.MapsTo(x => x.Project.Name);
            })
         //.WithValidators(c => {
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

      Project IVMCollectionItem<Project>.Source {
         get { return Project; }
      }

      public void InitializeFrom(Project source) {
         Project = source;
      }
   }

   public class ProjectVMDescriptor : VMDescriptor {
      public IVMProperty<string> Name { get; set; }
      public IVMProperty<int> MemberCount { get; set; }
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
