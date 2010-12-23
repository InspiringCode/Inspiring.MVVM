﻿namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;

   internal sealed class DepartmentVM : ViewModel<DepartmentVMDescriptor>, IVMCollectionItem<Department> {
      public static readonly DepartmentVMDescriptor Descriptor = VMDescriptorBuilder
         .OfType<DepartmentVMDescriptor>()
         .For<DepartmentVM>()
         .WithProperties((d, c) => {
            var vm = c.GetPropertyBuilder();
            var k = c.GetPropertyBuilder(x => x.DepartmentSource);

            d.Name = k.Property.MapsTo(x => x.Name);
         })
         .Build();

      public DepartmentVM()
         : base() {
      }

      public Department DepartmentSource { get; private set; }

      public string Name {
         get { return GetValue(DescriptorBase.Name); }
      }

      public void InitializeFrom(Department source) {
         DepartmentSource = source;
      }

      Department IVMCollectionItem<Department>.Source {
         get { return DepartmentSource; }
      }
   }

   public sealed class DepartmentVMDescriptor : VMDescriptor {
      public VMProperty<string> Name { get; set; }
   }
}
