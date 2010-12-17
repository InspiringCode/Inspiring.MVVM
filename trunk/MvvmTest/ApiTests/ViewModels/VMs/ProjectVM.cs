namespace Inspiring.MvvmTest.ApiTests.ViewModels {

   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public sealed class ProjectVM : ViewModel<ProjectVMDescriptor>, ICanInitializeFrom<Project> {
      public static readonly ProjectVMDescriptor Descriptor = VMDescriptorBuilder
         .For<ProjectVM>()
         .CreateDescriptor(c => {
            var p = c.GetPropertyBuilder(x => x.ProjectSource);

            return new ProjectVMDescriptor {
               Title = p.Mapped(x => x.Title).Property(),
               Customer = p.Mapped(x => x.Customer).VM<CustomerVM>()
            };
         })
         .Build();

      public ProjectVM()
         : this(Descriptor) {
      }

      public ProjectVM(ProjectVMDescriptor descriptor)
         : base(descriptor) {

      }

      public Project ProjectSource { get; private set; }

      public CustomerVM Customer {
         get { return GetValue(DescriptorBase.Customer); }
         set { SetValue(DescriptorBase.Customer, value); }
      }

      public void InitializeFrom(Project source) {
         ProjectSource = source;
      }

      public void UpdateCustomerFromSource() {
         Kernel.UpdateFromSource(DescriptorBase.Customer);
      }
   }

   public sealed class ProjectVMDescriptor : VMDescriptor {
      public VMProperty<string> Title { get; set; }
      public VMProperty<CustomerVM> Customer { get; set; }
   }
}
