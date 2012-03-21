namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidatorConditionTests {
      [TestMethod]
      public void PropertyValidation_WhenConditionIsFalse_IsNotExecuted() {
         bool condition = true;

         EmployeeVM vm = CreateVM(b => b
            .When(args => condition)
            .Check(x => x.Name)
            .Custom(args => args.AddError("Test error"))
         );

         vm.Revalidate(ValidationScope.SelfAndAllDescendants);

         ValidationAssert.IsInvalid(vm);

         condition = false;
         vm.Revalidate(ValidationScope.SelfAndAllDescendants);

         ValidationAssert.IsValid(vm);
      }

      [TestMethod]
      public void ViewModelCollectionValidation_WhenItemConditionIsFalse_IsNotExecuted() {
         ProjectVM project1 = new ProjectVM();
         ProjectVM project2 = new ProjectVM();

         EmployeeVM vm = CreateVM(b => b
            .ValidateDescendant(x => x.Projects)
            .When(args => args.Target == project2)
            .CheckViewModel(args => args.AddError("Test error"))
         );
         
         vm.Projects.Add(project1);
         vm.Projects.Add(project2);

         vm.Revalidate(ValidationScope.SelfAndAllDescendants);

         ValidationAssert.IsValid(project1);
         ValidationAssert.IsInvalid(project2);
      }

      private EmployeeVM CreateVM(
         Action<RootValidatorBuilder<EmployeeVM, EmployeeVM, EmployeeVMDescriptor>> configurationAction
      ) {
         return new EmployeeVM(BuildDescriptor(configurationAction));
      }

      // TODO: Code duplication with ValidatorBuilderTests
      private EmployeeVMDescriptor BuildDescriptor(
         Action<RootValidatorBuilder<EmployeeVM, EmployeeVM, EmployeeVMDescriptor>> configurationAction
      ) {
         return VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Name = v.Property.Of<string>();
               d.SelectedProject = v.VM.Of<ProjectVM>();
               d.Projects = v.Collection.Of<ProjectVM>(ProjectVM.ClassDescriptor);
            })
            .WithValidators(configurationAction)
            .Build();
      }


      private sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public EmployeeVM(EmployeeVMDescriptor descriptor)
            : base(descriptor) {
         }

         public IVMCollection<ProjectVM> Projects {
            get { return GetValue(Descriptor.Projects); }
         }
      }

      private sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<ProjectVM> SelectedProject { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
      }

      private sealed class ProjectVM : ViewModel<ProjectVMDescriptor> {
         public static readonly ProjectVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ProjectVMDescriptor>()
            .For<ProjectVM>()
            .WithProperties((d, b) => {

            })
            .WithValidators(b => {
               b.EnableParentViewModelValidation();
            })
            .Build();

         public ProjectVM()
            : base(ClassDescriptor) {
         }
      }

      private sealed class ProjectVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<DateTime> EndDate { get; set; }
      }
   }
}