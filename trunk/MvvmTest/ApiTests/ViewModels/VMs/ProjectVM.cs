namespace Inspiring.MvvmTest.ApiTests.ViewModels {

   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public sealed class ProjectVM : ViewModel<ProjectVMDescriptor>, IHasSourceObject<Project> {
      public static readonly ProjectVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<ProjectVMDescriptor>()
         .For<ProjectVM>()
         .WithProperties((d, c) => {
            var p = c.GetPropertyBuilder(x => x.ProjectSource);

            d.Title = p.Property.MapsTo(x => x.Title);
            d.Customer = p.VM.Wraps(x => x.Customer).With<CustomerVM>();
         })
         .WithValidators(b => {
            b.EnableParentValidation();
            b.Check(x => x.Title).Custom((vm, val, args) => {
               args.Errors.Add("Error");
               vm.WasValidated = true;
            });
         })
         .Build();

      public ProjectVM()
         : this(ClassDescriptor) {
      }

      public ProjectVM(ProjectVMDescriptor descriptor)
         : base(descriptor) {

      }

      public Project ProjectSource { get; private set; }

      public bool WasValidated { get; set; }

      public CustomerVM Customer {
         get { return GetValue(Descriptor.Customer); }
         set { SetValue(Descriptor.Customer, value); }
      }

      public string Title {
         get { return GetValue(Descriptor.Title); }
         set { SetValue(Descriptor.Title, value); }
      }

      public void InitializeFrom(Project source) {
         ProjectSource = source;
      }

      public void UpdateCustomerFromSource() {
         Kernel.UpdateFromSource(Descriptor.Customer);
      }

      public void Revalidate() {
         Kernel.Revalidate(ValidationScope.SelfOnly, ValidationMode.DiscardInvalidValues);
      }

      Project IHasSourceObject<Project>.Source {
         get { return ProjectSource; }
         set { ProjectSource = value; }
      }
   }

   public sealed class ProjectVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Title { get; set; }
      public IVMPropertyDescriptor<CustomerVM> Customer { get; set; }
   }
}
