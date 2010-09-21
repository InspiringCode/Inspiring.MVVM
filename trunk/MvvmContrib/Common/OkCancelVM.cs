namespace Inspiring.Mvvm.Common {
   using System.Windows.Input;
   using Inspiring.Mvvm.ViewModels;

   public sealed class OkCancelVM : ViewModel<OkCancelVMDescriptor>, ICanInitializeFrom<IOkCancelHandler> {
      public static readonly OkCancelVMDescriptor Descriptor = VMDescriptorBuilder
         .For<OkCancelVM>()
         .CreateDescriptor(c => {
            var h = c.GetPropertyFactory(x => x.DialogActionHandler);

            return new OkCancelVMDescriptor {
               Ok = h.Command(x => x.Ok(), x => x.CanOk()),
               Cancel = h.Command(x => x.Cancel(), x => x.CanCancel())
            };
         })
         .Build();

      public OkCancelVM()
         : base(Descriptor) {
      }

      private IOkCancelHandler DialogActionHandler { get; set; }

      public void InitializeFrom(IOkCancelHandler handler) {
         DialogActionHandler = handler;
      }
   }

   public sealed class OkCancelVMDescriptor : VMDescriptor {
      public VMProperty<ICommand> Ok { get; set; }
      public VMProperty<ICommand> Cancel { get; set; }
   }
}
