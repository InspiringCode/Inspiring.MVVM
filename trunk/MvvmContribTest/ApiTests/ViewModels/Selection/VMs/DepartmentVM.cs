namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;

   internal sealed class DepartmentVM : DefaultViewModelWithSourceBase<DepartmentVMDescriptor, Department> {
      public static readonly DepartmentVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<DepartmentVMDescriptor>()
         .For<DepartmentVM>()
         .WithProperties((d, c) => {
            var vm = c.GetPropertyBuilder();
            var k = c.GetPropertyBuilder(x => x.Source);

            d.Name = k.Property.MapsTo(x => x.Name);
         })
         .Build();

      public DepartmentVM()
         : base(ClassDescriptor) {
      }

      public string Name {
         get { return GetValue(Descriptor.Name); }
      }
   }

   public sealed class DepartmentVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Name { get; set; }
   }
}
