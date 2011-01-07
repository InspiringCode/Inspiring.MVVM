namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;

   public sealed class StateVM : ViewModel<StateVMDescriptor> {
      public static readonly StateVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<StateVMDescriptor>()
         .For<StateVM>()
         .WithProperties((d, c) => {
            var v = c.GetPropertyBuilder();
            d.Caption = v.Property.Of<string>();
         })
         .Build();

      public StateVM(string caption)
         : base(ClassDescriptor) {
         Caption = caption;
      }

      public string Caption {
         get { return GetValue(Descriptor.Caption); }
         set { SetValue(Descriptor.Caption, value); }
      }
   }

   public sealed class StateVMDescriptor : VMDescriptor {
      public IVMProperty<string> Caption { get; set; }
   }
}
