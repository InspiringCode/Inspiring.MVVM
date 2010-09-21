namespace Inspiring.Mvvm.Common {
   using System.Windows.Input;
   using Inspiring.Mvvm.ViewModels;

   public sealed class SaveDiscardVM : ViewModel<SaveDiscardVMDescriptor>, ICanInitializeFrom<ISaveDiscardHandler> {
      public static readonly SaveDiscardVMDescriptor Descriptor = VMDescriptorBuilder
         .For<SaveDiscardVM>()
         .CreateDescriptor(c => {
            var h = c.GetPropertyFactory(x => x.DialogActionHandler);

            return new SaveDiscardVMDescriptor {
               Save = h.Command(x => x.Save(), x => x.CanSave()),
               Discard = h.Command(x => x.Discard(), x => x.CanDiscard())
            };
         })
         .Build();

      public SaveDiscardVM()
         : base(Descriptor) {
      }

      private ISaveDiscardHandler DialogActionHandler { get; set; }

      public void InitializeFrom(ISaveDiscardHandler handler) {
         DialogActionHandler = handler;
      }
   }

   public sealed class SaveDiscardVMDescriptor : VMDescriptor {
      public VMProperty<ICommand> Save { get; set; }
      public VMProperty<ICommand> Discard { get; set; }
   }
}
