namespace Inspiring.MvvmTest.ApiTests.ViewModels.Undo {
   using Inspiring.Mvvm.ViewModels;

   public sealed class ProjectVM : DefaultViewModelWithSourceBase<ProjectVMDescriptor, Project> {
      public static readonly ProjectVMDescriptor ClassDescriptorWithoutUndoRoot = VMDescriptorBuilder
         .OfType<ProjectVMDescriptor>()
         .For<ProjectVM>()
         .WithProperties((d, c) => {
            var b = c.GetPropertyBuilder(x => x.Source);

            d.Title = b.Property.MapsTo(x => x.Title);
            d.Customer = b.VM.Wraps(x => x.Customer).With<CustomerVM>();
         })
         .WithValidators(b => {
            //b.EnableParentValidation(x => x.Title);
         })
         .WithViewModelBehaviors(b => {
            b.EnableUndo();
         })
         .Build();

      public static readonly ProjectVMDescriptor ClassDescriptorWithUndoRoot = VMDescriptorBuilder
         .Inherits(ClassDescriptorWithoutUndoRoot)
         .OfType<ProjectVMDescriptor>()
         .For<ProjectVM>()
         .WithProperties((d, c) => { })
         .WithViewModelBehaviors(b => {
            b.IsUndoRoot();
            b.EnableUndo();
         })
         .Build();

      public ProjectVM()
         : base(ClassDescriptorWithoutUndoRoot) {
      }

      internal string Title {
         get { return GetValue(Descriptor.Title); }
         set { SetValue(Descriptor.Title, value); }
      }

      internal CustomerVM Customer {
         get { return GetValue(Descriptor.Customer); }
         set { SetValue(Descriptor.Customer, value); }
      }
   }

   public sealed class ProjectVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Title { get; set; }
      public IVMPropertyDescriptor<CustomerVM> Customer { get; set; }
   }
}
