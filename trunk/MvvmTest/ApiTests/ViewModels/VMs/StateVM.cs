namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;

   public sealed class StateVM : ViewModel<StateVMDescriptor> {
      public static readonly StateVMDescriptor Descriptor = VMDescriptorBuilder
         .OfType<StateVMDescriptor>()
         .For<StateVM>()
         .WithProperties((d, c) => {
            var v = c.GetPropertyBuilder();
            d.Caption = v.Property.Of<string>();
         })
         .Build();

      public StateVM(string caption)
         : base(Descriptor) {
         Caption = caption;
      }

      public string Caption {
         get { return GetValue(Descriptor.Caption); }
         set { SetValue(Descriptor.Caption, value); }
      }
   }

   public sealed class StateVMDescriptor : VMDescriptor {
      public VMProperty<string> Caption { get; set; }
   }
}
