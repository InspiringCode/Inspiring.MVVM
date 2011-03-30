namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public abstract class HierarchyValidationFixture {
      public ValidatorInvocationLog Log { get; set; }

      [TestInitialize]
      public void Setup() {
         Log = new ValidatorInvocationLog();
      }

      protected EmployeeVM CreateEmployeeVM() {
         return new EmployeeVM { Log = Log };
      }

      //protected EmployeeVM CreateEmployeeVMWithProjects() {
      //   var vm = CreateEmployeeVM();
      //   vm.Projects.Add(CreateProjectVM());
      //   vm.Projects.Add(CreateProjectVM());
      //   return vm;
      //}

      protected ProjectVM CreateProjectVM() {
         return new ProjectVM { Log = Log };
      }

      protected CustomerVM CreateCustomerVM() {
         return new CustomerVM { Log = Log };
      }

      public enum Validator {
         EmployeeName,
         ProjectTitle,
         ProjectDescription,
         CustomerName,
         CustomerAddress,
         CustomerPostalCode
      }

      public sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();

               d.Name = v.Property.Of<string>();
               d.Projects = v.Collection.Of<ProjectVM>(ProjectVM.ClassDescriptor);
            })
            .WithValidators(c => {
               c.Check(x => x.Name).Custom((vm, val, args) => {
                  vm.Log.AddCall(Validator.EmployeeName, args);
               });
               c
                  .ValidateDescendant(x => x.Projects)
                  .Check(x => x.Title)
                  .Custom((vm, val, args) => {
                     ((ProjectVM)vm).Log.AddCall(Validator.ProjectTitle, args);
                  });
               c
                  .ValidateDescendant(x => x.Projects)
                  .ValidateDescendant(x => x.Customer)
                  .Check(x => x.Name)
                  .Custom((vm, val, args) => {
                     ((CustomerVM)vm).Log.AddCall(Validator.CustomerName, args);
                  });
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
         }

         public ValidatorInvocationLog Log { get; set; }

         public IVMCollection<ProjectVM> Projects {
            get { return GetValue(Descriptor.Projects); }
         }

         public void Revalidate(ValidationScope scope = ValidationScope.SelfOnly) {
            Revalidate(scope, ValidationMode.DiscardInvalidValues);
         }
      }

      public sealed class ProjectVM : ViewModel<ProjectVMDescriptor> {
         public static readonly ProjectVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ProjectVMDescriptor>()
            .For<ProjectVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();

               d.Title = v.Property.Of<string>();
               d.Description = v.Property.Of<string>();
               d.Customer = v.VM.Of<CustomerVM>();
            })
            .WithValidators(c => {
               c.EnableParentValidation(x => x.Title);

               c.Check(x => x.Description).Custom((vm, val, args) => {
                  vm.Log.AddCall(Validator.ProjectDescription, args);
               });
               c.ValidateDescendant(x => x.Customer).Check(x => x.Address).Custom((vm, val, args) => {
                  ((CustomerVM)vm).Log.AddCall(Validator.CustomerAddress, args);
               });
            })
            .Build();

         public ProjectVM()
            : base(ClassDescriptor) {
         }

         public ValidatorInvocationLog Log { get; set; }

         public CustomerVM Customer {
            get { return GetValue(Descriptor.Customer); }
            set { SetValue(Descriptor.Customer, value); }
         }

         public void Revalidate(ValidationScope scope = ValidationScope.SelfOnly) {
            Revalidate(scope, ValidationMode.DiscardInvalidValues);
         }
      }

      public sealed class CustomerVM : ViewModel<CustomerVMDescriptor> {
         public static readonly CustomerVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<CustomerVMDescriptor>()
            .For<CustomerVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();

               d.Name = v.Property.Of<string>();
               d.Address = v.Property.Of<string>();
               d.PostalCode = v.Property.Of<int>();
            })
            .WithValidators(c => {
               c.EnableParentValidation(x => x.Name);
               c.EnableParentValidation(x => x.Address);
               c.Check(x => x.PostalCode).Custom((vm, val, args) => {
                  vm.Log.AddCall(Validator.CustomerPostalCode, args);
               });
            })
            .Build();

         public CustomerVM()
            : base(ClassDescriptor) {
         }

         public ValidatorInvocationLog Log { get; set; }

         public void Revalidate(ValidationScope scope = ValidationScope.SelfOnly) {
            Revalidate(scope, ValidationMode.DiscardInvalidValues);
         }
      }

      public sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
      }

      public sealed class ProjectVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Title { get; set; }
         public IVMPropertyDescriptor<string> Description { get; set; }
         public IVMPropertyDescriptor<CustomerVM> Customer { get; set; }
      }

      public sealed class CustomerVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<string> Address { get; set; }
         public IVMPropertyDescriptor<int> PostalCode { get; set; }
      }
   }
}