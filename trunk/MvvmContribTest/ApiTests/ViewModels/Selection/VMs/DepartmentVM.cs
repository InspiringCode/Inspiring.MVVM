namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;

   internal sealed class DepartmentVM : ViewModel<DepartmentVMDescriptor>, IHasSourceObject<Department> {
      public static readonly DepartmentVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<DepartmentVMDescriptor>()
         .For<DepartmentVM>()
         .WithProperties((d, c) => {
            var vm = c.GetPropertyBuilder();
            var k = c.GetPropertyBuilder(x => x.DepartmentSource);

            d.Name = k.Property.MapsTo(x => x.Name);
         })
         .Build();

      public DepartmentVM()
         : base(ClassDescriptor) {
      }

      public Department DepartmentSource { get; private set; }

      public string Name {
         get { return GetValue(Descriptor.Name); }
      }

      public void InitializeFrom(Department source) {
         DepartmentSource = source;
      }

      Department IHasSourceObject<Department>.Source {
         get { return DepartmentSource; }
      }
   }

   public sealed class DepartmentVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Name { get; set; }
   }
}
