namespace Inspiring.Mvvm.Common {
   using System.Windows.Input;
   using Inspiring.Mvvm.ViewModels;

   public sealed class OkCancelVM : DefaultViewModelWithSourceBase<OkCancelVMDescriptor, IOkCancelHandler> {
      public static readonly OkCancelVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<OkCancelVMDescriptor>()
         .For<OkCancelVM>()
         .WithProperties((d, c) => {
            var h = c.GetPropertyBuilder(x => x.Source);

            d.Ok = h.Command(x => x.Ok(), x => x.CanOk());
            d.Cancel = h.Command(x => x.Cancel(), x => x.CanCancel());
         })
         .Build();

      public OkCancelVM()
         : base(ClassDescriptor) {
      }
   }

   public sealed class OkCancelVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<ICommand> Ok { get; set; }
      public IVMPropertyDescriptor<ICommand> Cancel { get; set; }
   }
}
