namespace Inspiring.Mvvm.Common {
   using System;
   using System.Windows.Input;
   using Inspiring.Mvvm.ViewModels;

   public enum DataState {
      Unchanged,
      Invalid,
      Changed
   }

   public sealed class SaveDiscardVM : ViewModel<SaveDiscardVMDescriptor>, ICanInitializeFrom<ISaveDiscardHandler> {
      public static readonly SaveDiscardVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<SaveDiscardVMDescriptor>()
         .For<SaveDiscardVM>()
         .WithProperties((d, c) => {
            var h = c.GetPropertyBuilder(x => x.DialogActionHandler);

               d.Save = h.Command(x => x.Save(), x => x.CanSave());
               d.Discard = h.Command(x => x.Discard(), x => x.CanDiscard());
               d.State = h.Property.DelegatesTo(x => {
                  if (!x.IsValid) {
                     return DataState.Invalid;
                  } else {
                     return x.HasChanges ? DataState.Changed : DataState.Unchanged;
                  }
               });
         })
         .Build();

      private EventHandler _requerySuggestedHandler;

      public SaveDiscardVM()
         : base() {
      }

      public DataState State {
         get { return GetValue(Descriptor.State); }
      }

      private ISaveDiscardHandler DialogActionHandler { get; set; }

      public void InitializeFrom(ISaveDiscardHandler handler) {
         DialogActionHandler = handler;

         // RequerySuggested is a weak event. We have to hold a strong 
         // reference to the handler to avoid garbage collection!
         _requerySuggestedHandler = new EventHandler((sender, e) => {
            throw new NotImplementedException();
            //OnPropertyChanged(Descriptor.State);
         });

         CommandManager.RequerySuggested += _requerySuggestedHandler;
      }
   }

   public sealed class SaveDiscardVMDescriptor : VMDescriptor {
      public VMProperty<ICommand> Save { get; set; }
      public VMProperty<ICommand> Discard { get; set; }
      public VMProperty<DataState> State { get; set; }
   }
}
