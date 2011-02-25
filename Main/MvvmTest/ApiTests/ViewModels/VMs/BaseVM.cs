namespace Inspiring.MvvmTest.ApiTests.ViewModels.VMs {
   using Inspiring.Mvvm.ViewModels;

   public abstract class BaseVM : ViewModel<BaseVMDescriptor> {

      public static readonly BaseVMDescriptor ClassDescriptor = VMDescriptorBuilder
        .OfType<BaseVMDescriptor>()
        .For<BaseVM>()
        .WithProperties((d, b) => {
           var a = b.GetPropertyBuilder();
           d.IsSelected = a.Property.Of<bool>();
           d.IsExpanded = a.Property.Of<bool>();
        })
     .Build();

      public BaseVM(BaseVMDescriptor descriptor)
         : base(descriptor) {
      }

      public bool IsSelected {
         get { return GetValue(Descriptor.IsSelected); }
         set { SetValue(Descriptor.IsSelected, value); }
      }

      public bool IsExpanded {
         get { return GetValue(Descriptor.IsExpanded); }
         set { SetValue(Descriptor.IsExpanded, value); }
      }
   }

   public class BaseVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<bool> IsSelected { get; set; }
      public IVMPropertyDescriptor<bool> IsExpanded { get; set; }
   }
}
