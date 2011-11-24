namespace Inspiring.MvvmContribTest.ApiTests.Bugs {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public sealed class RefreshContainerWhenItemContainsSelectionTests {

      [TestMethod]
      public void RefreshContainer_WhenItemIsAddedToSoureCollection_RevalidatesSelectionOfItemVMWhenLoaded() {
         Employee emp = new Employee();
         EmployeeVM empVM = new EmployeeVM();
         empVM.InitializeFrom(emp);

         empVM.Load(x => x.Projects);

         emp.AddProjekt();

         empVM.RefreshContainer(x => x.Projects);

         ProjectVM projectVM = empVM.GetValue(x => x.Projects).Single();
         projectVM.Load(x => x.Department);

         ValidationAssert.ErrorMessages(projectVM.GetValidationResult(ValidationResultScope.All), ProjectVM.ValidationError);
      }

      private class Employee {

         public Employee() {
            Projects = new List<Project>();
         }

         public string Name { get; set; }
         public Project CurrentProject { get; set; }
         public List<Project> Projects { get; set; }

         public Project AddProjekt() {
            var projekt = new Project();
            Projects.Add(projekt);

            return projekt;
         }

         public void RemoveProjekt(Project projekt) {
            Projects.Remove(projekt);
         }
      }

      private class Project {
         public Department Department { get; set; }
      }

      private class Department { }

      private sealed class EmployeeVM : DefaultViewModelWithSourceBase<EmployeeVMDescriptor, Employee> {
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, c) => {
               var b = c.GetPropertyBuilder(x => x.Source);

               d.Projects = b.Collection
                  .Wraps(x => x.Projects)
                  .With<ProjectVM>(ProjectVM.ClassDescriptor);
            })
            .Build();


         public EmployeeVM()
            : base(ClassDescriptor) {
         }
      }

      private sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
      }

      private sealed class ProjectVM : DefaultViewModelWithSourceBase<ProjectVMDescriptor, Project> {
         public const string ValidationError = "Department is required";

         public static readonly ProjectVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ProjectVMDescriptor>()
            .For<ProjectVM>()
            .WithProperties((d, b) => {
               var vm = b.GetPropertyBuilder();

               d.Department = vm.SingleSelection(x => x.Source.Department)
                  .WithItems(x => new[] { new Department() })
                  .WithCaption(x => "Department");

            })
            .WithValidators(b => {
               b.ValidateDescendant(x => x.Department)
               .Check(x => x.SelectedItem)
               .HasValue(ValidationError);
            })
            .Build();

         public ProjectVM()
            : base(ClassDescriptor) {
         }

      }

      private sealed class ProjectVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<SingleSelectionVM<Department>> Department { get; set; }
      }
   }
}
