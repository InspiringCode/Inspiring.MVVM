namespace Inspiring.MvvmTest.ApiTests.ViewModels {

   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public sealed class ProjectVM : ViewModel<ProjectVMDescriptor>, ICanInitializeFrom<Project> {
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
         })
         .Build();

      public ProjectVM()
         : this(ClassDescriptor) {
      }

      public ProjectVM(ProjectVMDescriptor descriptor)
         : base(descriptor) {

      }

      public Project ProjectSource { get; private set; }

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
   }

   public sealed class ProjectVMDescriptor : VMDescriptor {
      public VMProperty<string> Title { get; set; }
      public VMProperty<CustomerVM> Customer { get; set; }
   }
}
