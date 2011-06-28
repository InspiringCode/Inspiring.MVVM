namespace Inspiring.MvvmTest.ApiTests.ViewModels.Core {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class PathDefinitionTests : TestBase {
      private Path PropertyPath { get; set; }
      private Path ViewModelDescendantPropertPath { get; set; }
      private Path CollectionDescendantPropertyPath { get; set; }

      private Path ViewModelPath { get; set; }
      private Path ViewModelDescendantViewModelPath { get; set; }
      private Path CollectionDescendantViewModelPath { get; set; }

      private Path CollectionPropertyPath { get; set; }
      private Path CollectionViewModelPath { get; set; }

      private PathDefinition PropertyPathDefinition { get; set; }
      private PathDefinition ViewModelDescendantPropertPathDefinition { get; set; }
      private PathDefinition CollectionDescendantPropertyPathDefinition { get; set; }

      private PathDefinition ViewModelPathDefinition { get; set; }
      private PathDefinition ViewModelDescendantViewModelPathDefinition { get; set; }
      private PathDefinition CollectionDescendantViewModelPathDefinition { get; set; }

      private PathDefinition CollectionPropertyPathDefinition { get; set; }
      private PathDefinition CollectionViewModelPathDefinition { get; set; }

      private EmployeeVM Employee { get; set; }
      private ProjectVM Project { get; set; }
      private CustomerVM SelectedCustomer { get; set; }
      private CustomerVM AnotherCustomer { get; set; }

      private EmployeeVMDescriptor EmployeeDescriptor { get; set; }
      private ProjectVMDescriptor ProjectDescriptor { get; set; }
      private CustomerVMDescriptor CustomerDescriptor { get; set; }

      [TestInitialize]
      public void Setup() {
         SelectedCustomer = new CustomerVM();

         Project = new ProjectVM();
         Project.SelectedCustomer = SelectedCustomer;
         Project.Customers.Add(SelectedCustomer);

         Employee = new EmployeeVM();
         Employee.SelectedProject = Project;
         Employee.Projects.Add(Project);

         EmployeeDescriptor = EmployeeVM.ClassDescriptor;
         ProjectDescriptor = ProjectVM.ClassDescriptor;
         CustomerDescriptor = CustomerVM.ClassDescriptor;

         PropertyPath = Path.Empty
            .Append(SelectedCustomer)
            .Append(CustomerDescriptor.Name);

         ViewModelDescendantPropertPath = PropertyPath.Prepend(Project);
      }

      [TestMethod]
      public void Matches_OptionalStepWithAnyPropertyStep_MatchesProperty() {
         var definition = PathDefinition
            .Empty
            .Append(new OptionalStep(new AnyPropertyStep<EmployeeVMDescriptor>()));

         var path = Path
            .Empty
            .Append(new EmployeeVM())
            .Append(EmployeeVM.ClassDescriptor.Name);

         var match = definition.Matches(path);

         Assert.IsTrue(match.Success);
      }

      [TestMethod]
      public void Matches_OptionalStepWithAnyPropertyStep_MatchesRootViewModel() {
         var definition = PathDefinition
            .Empty
            .Append(new OptionalStep(new AnyPropertyStep<EmployeeVMDescriptor>()));

         var path = Path
            .Empty
            .Append(new EmployeeVM());

         var match = definition.Matches(path);

         Assert.IsTrue(match.Success);
      }

      [TestMethod]
      public void Matches_OptionalStepWithAnyPropertyStep_DoesNotMatchPropertyOfChildViewModel() {
         var definition = PathDefinition
            .Empty
            .Append(new OptionalStep(new AnyPropertyStep<EmployeeVMDescriptor>()));

         var rootVM = new EmployeeVM();

         var path = Path
            .Empty
            .Append(rootVM)
            .Append(EmployeeVM.ClassDescriptor.SelectedProject)
            .Append(ProjectVM.ClassDescriptor.Name);

         var match = definition.Matches(path);

         Assert.IsFalse(match.Success);
      }

      [TestMethod]
      public void Matches_ChildViewModelPlusOptionalStepWithAnyPropertyStep_MacthesPropertyOfChildViewModel() {
         var definition = PathDefinition
            .Empty
            .Append((EmployeeVMDescriptor x) => x.SelectedProject)
            .Append(new OptionalStep(new AnyPropertyStep<ProjectVMDescriptor>()));

         var projectVM = new ProjectVM();
         var rootVM = new EmployeeVM();
         rootVM.SetValue(x => x.SelectedProject, projectVM);

         var path = Path
            .Empty
            .Append(rootVM)
            .Append(rootVM.GetValue(x => x.SelectedProject))
            .Append(ProjectVM.ClassDescriptor.Name);

         var match = definition.Matches(path);

         Assert.IsTrue(match.Success);
      }

      [TestMethod]
      public void Matches_OptionalStepWithAnyStepsStep_MatchesRootViewMode() {
         var definition = PathDefinition
            .Empty
            .Append(new OptionalStep(new AnyStepsStep<EmployeeVMDescriptor>()));

         var path = Path
            .Empty
            .Append(new EmployeeVM())
            .Append(EmployeeVM.ClassDescriptor.Name);

         var match = definition.Matches(path);

         Assert.IsTrue(match.Success);
      }

      [TestMethod]
      public void Matches_OptionalStepWithAnyStepsStep_MatchesPropertyOfChildViewModel() {
         var definition = PathDefinition
            .Empty
            .Append(new OptionalStep(new AnyStepsStep<EmployeeVMDescriptor>()));

         var rootVM = new EmployeeVM();

         var pathLength = 3;
         var path = Path
            .Empty
            .Append(rootVM)
            .Append(EmployeeVM.ClassDescriptor.SelectedProject)
            .Append(ProjectVM.ClassDescriptor.Name);

         var match = definition.Matches(path);

         Assert.IsTrue(match.Success);
         Assert.AreEqual(pathLength, match.Length);
      }

      [TestMethod]
      public void Matches_ChildViewModelPlusOptionalStepWithAnyStepsStep_MacthesPropertyOfChildChildViewModel() {
         var definition = PathDefinition
            .Empty
            .Append((EmployeeVMDescriptor x) => x.SelectedProject)
            .Append(new OptionalStep(new AnyStepsStep<ProjectVMDescriptor>()));

         var customerVM = new CustomerVM();
         var projectVM = new ProjectVM();
         projectVM.SetValue(x => x.SelectedCustomer, customerVM);
         var rootVM = new EmployeeVM();
         rootVM.SetValue(x => x.SelectedProject, projectVM);

         var path = Path
            .Empty
            .Append(rootVM)
            .Append(rootVM.GetValue(x => x.SelectedProject))
            .Append(projectVM.GetValue(x => x.SelectedCustomer))
            .Append(CustomerVM.ClassDescriptor.Name);

         var match = definition.Matches(path);

         Assert.IsTrue(match.Success);
      }

      [TestMethod]
      public void Matches_OrStepWithTwoPropertySteps_MatchesIfOneOfThePropertiesMatches() {
         var selectedProjectStep = CreatePropertyStep((EmployeeVMDescriptor x) => x.SelectedProject);
         var nameStep = CreatePropertyStep((EmployeeVMDescriptor x) => x.Name);

         var orStep = new OrStep(selectedProjectStep, nameStep);

         var definition = PathDefinition
            .Empty
            .Append(orStep);

         var projectVM = new ProjectVM();
         var rootVM = new EmployeeVM();
         rootVM.SetValue(x => x.SelectedProject, projectVM);

         var namePath = Path
            .Empty
            .Append(rootVM)
            .Append(EmployeeVM.ClassDescriptor.Name);

         var match = definition.Matches(namePath);

         Assert.IsTrue(match.Success);

         var selectedProjectPath = Path
            .Empty
            .Append(rootVM)
            .Append(EmployeeVM.ClassDescriptor.SelectedProject);

         match = definition.Matches(namePath);

         Assert.IsTrue(match.Success);
      }

      [TestMethod]
      public void Matches_OrStepWithTwoPropertySteps_MatchesIfNoThePropertiesMatches() {
         var selectedProjectStep = CreatePropertyStep((EmployeeVMDescriptor x) => x.SelectedProject);
         var nameStep = CreatePropertyStep((EmployeeVMDescriptor x) => x.Name);

         var orStep = new OrStep(selectedProjectStep, nameStep);

         var definition = PathDefinition
            .Empty
            .Append(orStep);

         var projectVM = new ProjectVM();
         var rootVM = new EmployeeVM();

         var projectPath = Path
            .Empty
            .Append(rootVM)
            .Append(EmployeeVM.ClassDescriptor.Projects);

         var match = definition.Matches(projectPath);

         Assert.IsFalse(match.Success);
      }

      [TestMethod]
      public void Matches_ComplexOrStep() {
         var projectsStep = CreatePropertyStep((EmployeeVMDescriptor x) => x.Projects);
         var anotherProjects = CreatePropertyStep((EmployeeVMDescriptor x) => x.AnotherProjects);

         var orStep = new OrStep(projectsStep, anotherProjects);

         var definition = PathDefinition
            .Empty
            .Append(orStep)
            .Append((ProjectVMDescriptor x) => x.Name);

         var projectVM1 = new ProjectVM();
         var projectVM2 = new ProjectVM();
         var rootVM = new EmployeeVM();
         rootVM.GetValue(x => x.Projects).Add(projectVM1);
         rootVM.GetValue(x => x.AnotherProjects).Add(projectVM2);

         var projectPath = Path
            .Empty
            .Append(rootVM)
            .Append(rootVM.GetValue(x => x.Projects).First())
            .Append(ProjectVM.ClassDescriptor.Name);

         var match = definition.Matches(projectPath);

         Assert.IsTrue(match.Success);

         var anotherProjectPath = Path
            .Empty
            .Append(rootVM)
            .Append(rootVM.GetValue(x => x.AnotherProjects).First())
            .Append(ProjectVM.ClassDescriptor.Name);

         match = definition.Matches(anotherProjectPath);

         Assert.IsTrue(match.Success);
      }

      private PropertyStep<TDescriptor> CreatePropertyStep<TDescriptor, TValue>(
         Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector
      ) where TDescriptor : IVMDescriptor {
         return new PropertyStep<TDescriptor>(propertySelector, typeof(TValue));
      }

      private class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Name = v.Property.Of<string>();
               d.SelectedProject = v.VM.Of<ProjectVM>();
               d.Projects = v.Collection.Of<ProjectVM>(ProjectVM.ClassDescriptor);
               d.AnotherProjects = v.Collection.Of<ProjectVM>(ProjectVM.ClassDescriptor);
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
         }

         public ProjectVM SelectedProject {
            get { return GetValue(Descriptor.SelectedProject); }
            set { SetValue(Descriptor.SelectedProject, value); }
         }

         public IVMCollection<ProjectVM> Projects {
            get { return GetValue(Descriptor.Projects); }
         }

         public IVMCollection<ProjectVM> AnotherProjects {
            get { return GetValue(Descriptor.AnotherProjects); }
         }
      }

      private class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<ProjectVM> SelectedProject { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> AnotherProjects { get; set; }
      }

      private class ProjectVM : ViewModel<ProjectVMDescriptor> {
         public static readonly ProjectVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ProjectVMDescriptor>()
            .For<ProjectVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Name = v.Property.Of<string>();
               d.SelectedCustomer = v.VM.Of<CustomerVM>();
               d.Customers = v.Collection.Of<CustomerVM>(CustomerVM.ClassDescriptor);
            })
            .Build();

         public ProjectVM()
            : base(ClassDescriptor) {
         }

         public CustomerVM SelectedCustomer {
            get { return GetValue(Descriptor.SelectedCustomer); }
            set { SetValue(Descriptor.SelectedCustomer, value); }
         }

         public IVMCollection<CustomerVM> Customers {
            get { return GetValue(Descriptor.Customers); }
         }
      }

      private class ProjectVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<CustomerVM> SelectedCustomer { get; set; }
         public IVMPropertyDescriptor<IVMCollection<CustomerVM>> Customers { get; set; }
      }

      private class CustomerVM : ViewModel<CustomerVMDescriptor> {
         public static readonly CustomerVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<CustomerVMDescriptor>()
            .For<CustomerVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Name = v.Property.Of<string>();
            })
            .Build();

         public CustomerVM()
            : base(ClassDescriptor) {
         }
      }

      private class CustomerVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
      }
   }
}
