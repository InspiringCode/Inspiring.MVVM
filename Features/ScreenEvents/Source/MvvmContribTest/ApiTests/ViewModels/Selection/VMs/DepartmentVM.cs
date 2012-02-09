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
            d.CachedName = k.Property.MapsTo(x => x.Name);
         })
         .WithBehaviors(b => {
            b.Property(x => x.CachedName).IsCached();
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
      public IVMPropertyDescriptor<string> CachedName { get; set; }
   }
}
