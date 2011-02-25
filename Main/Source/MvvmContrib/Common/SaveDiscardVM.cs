namespace Inspiring.Mvvm.Common {
   using System;
   using System.Windows.Input;
   using Inspiring.Mvvm.ViewModels;

   public enum DataState {
      Unchanged,
      Invalid,
      Changed
   }

   public sealed class SaveDiscardVM : DefaultViewModelWithSourceBase<SaveDiscardVMDescriptor, ISaveDiscardHandler> {
      public static readonly SaveDiscardVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<SaveDiscardVMDescriptor>()
         .For<SaveDiscardVM>()
         .WithProperties((d, c) => {
            var h = c.GetPropertyBuilder(x => x.Source);

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
         : base(ClassDescriptor) {
      }

      public DataState State {
         get { return GetValue(Descriptor.State); }
      }

      /// <inheritdoc />
      public override void InitializeFrom(ISaveDiscardHandler source) {
         base.InitializeFrom(source);

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
      public IVMPropertyDescriptor<ICommand> Save { get; set; }
      public IVMPropertyDescriptor<ICommand> Discard { get; set; }
      public IVMPropertyDescriptor<DataState> State { get; set; }
   }
}
