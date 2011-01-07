namespace Inspiring.Mvvm.Common {
   using System.Windows.Input;
   using Inspiring.Mvvm.ViewModels;

   public sealed class OkCancelVM : ViewModel<OkCancelVMDescriptor>, ICanInitializeFrom<IOkCancelHandler> {
      public static readonly OkCancelVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<OkCancelVMDescriptor>()
         .For<OkCancelVM>()
         .WithProperties((d, c) => {
            var h = c.GetPropertyBuilder(x => x.DialogActionHandler);

            d.Ok = h.Command(x => x.Ok(), x => x.CanOk());
            d.Cancel = h.Command(x => x.Cancel(), x => x.CanCancel());
         })
         .Build();

      public OkCancelVM()
         : base() {
      }

      private IOkCancelHandler DialogActionHandler { get; set; }

      public void InitializeFrom(IOkCancelHandler handler) {
         DialogActionHandler = handler;
      }
   }

   public sealed class OkCancelVMDescriptor : VMDescriptor {
      public IVMProperty<ICommand> Ok { get; set; }
      public IVMProperty<ICommand> Cancel { get; set; }
   }
}
