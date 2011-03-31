namespace Inspiring.MvvmTest.ApiTests.ViewModels.Undo {
   using Inspiring.Mvvm.ViewModels;

   public sealed class ProjectVM : DefaultViewModelWithSourceBase<ProjectVMDescriptor, Project> {
      public static readonly ProjectVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<ProjectVMDescriptor>()
         .For<ProjectVM>()
         .WithProperties((d, c) => {
            var b = c.GetPropertyBuilder(x => x.Source);

            d.Title = b.Property.MapsTo(x => x.Title);
         })
         .WithViewModelBehaviors(b => {
            b.EnableUndo();
         })
         .Build();

      public ProjectVM()
         : base(ClassDescriptor) {
      }
   }

   public sealed class ProjectVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Title { get; set; }
      //public IVMPropertyDescriptor<CustomerVM> Customer { get; set; }
   }
}
