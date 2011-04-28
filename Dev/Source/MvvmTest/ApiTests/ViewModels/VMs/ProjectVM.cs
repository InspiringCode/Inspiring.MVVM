namespace Inspiring.MvvmTest.ApiTests.ViewModels {

   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public sealed class ProjectVM : DefaultViewModelWithSourceBase<ProjectVMDescriptor, Project> {
      public static readonly ProjectVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<ProjectVMDescriptor>()
         .For<ProjectVM>()
         .WithProperties((d, c) => {
            var p = c.GetPropertyBuilder(x => x.Source);

            d.Title = p.Property.MapsTo(x => x.Title);
            d.Customer = p.VM.Wraps(x => x.Customer).With<CustomerVM>();
         })
         .WithValidators(b => {
            b.EnableParentValidation(x => x.Title);
            b.EnableParentValidation(x => x.Customer);

            b.Check(x => x.Title).Custom(args => {
               args.AddError("Error");
               args.Owner.WasValidated = true;
            });
         })
         .Build();

      public ProjectVM()
         : this(ClassDescriptor) {
      }

      public ProjectVM(ProjectVMDescriptor descriptor)
         : base(descriptor) {

      }

      public bool WasValidated { get; set; }

      public CustomerVM Customer {
         get { return GetValue(Descriptor.Customer); }
         set { SetValue(Descriptor.Customer, value); }
      }

      public string Title {
         get { return GetValue(Descriptor.Title); }
         set { SetValue(Descriptor.Title, value); }
      }

      public void UpdateCustomerFromSource() {
         Kernel.UpdateFromSource(Descriptor.Customer);
      }

      public void Revalidate() {
         Kernel.Revalidate(ValidationScope.Self, ValidationMode.DiscardInvalidValues);
      }
   }

   public sealed class ProjectVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Title { get; set; }
      public IVMPropertyDescriptor<CustomerVM> Customer { get; set; }
   }
}
