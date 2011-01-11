﻿namespace Inspiring.Mvvm.Common {
   using System.Windows.Input;
   using Inspiring.Mvvm.ViewModels;

   public sealed class OkCancelVM : ViewModel<OkCancelVMDescriptor>, IHasSourceObject<IOkCancelHandler> {
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
         : base(ClassDescriptor) {
      }

      private IOkCancelHandler DialogActionHandler { get; set; }

      public void InitializeFrom(IOkCancelHandler handler) {
         DialogActionHandler = handler;
      }

      IOkCancelHandler IHasSourceObject<IOkCancelHandler>.Source {
         get { return DialogActionHandler; }
         set { DialogActionHandler = value; }
      }
   }

   public sealed class OkCancelVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<ICommand> Ok { get; set; }
      public IVMPropertyDescriptor<ICommand> Cancel { get; set; }
   }
}
