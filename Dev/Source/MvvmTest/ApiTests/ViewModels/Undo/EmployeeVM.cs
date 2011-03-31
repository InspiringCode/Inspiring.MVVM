namespace Inspiring.MvvmTest.ApiTests.ViewModels.Undo {
   using Inspiring.Mvvm.ViewModels;

   public sealed class EmployeeVM : DefaultViewModelWithSourceBase<EmployeeVMDescriptor, Employee> {
      public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<EmployeeVMDescriptor>()
         .For<EmployeeVM>()
         .WithProperties((d, c) => {
            var b = c.GetPropertyBuilder(x => x.Source);

            d.Name = b.Property.MapsTo(x => x.Name);
            d.Projects = b.Collection
               .Wraps(x => x.Projects)
               .With<ProjectVM>(ProjectVM.ClassDescriptor);
            d.SelectedProject = b.Property.Of<ProjectVM>();
         })
         .WithViewModelBehaviors(b => {
            b.IsUndoRoot();
            b.EnableUndo();
         })
         .Build();

      public EmployeeVM()
         : base(ClassDescriptor) {
      }
   }

   public sealed class EmployeeVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Name { get; set; }
      public IVMPropertyDescriptor<ProjectVM> SelectedProject { get; set; }
      public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
   }

}
