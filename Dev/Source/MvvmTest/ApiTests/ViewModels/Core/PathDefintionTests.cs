namespace Inspiring.MvvmTest.ApiTests.ViewModels.Core {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   // TODO: Write these tests...

   [TestClass]
   public class PathDefintionTests : TestBase {
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


      private class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Name = v.Property.Of<string>();
               d.SelectedProject = v.VM.Of<ProjectVM>();
               d.Projects = v.Collection.Of<ProjectVM>(ProjectVM.ClassDescriptor);
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
      }

      private class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<ProjectVM> SelectedProject { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
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
