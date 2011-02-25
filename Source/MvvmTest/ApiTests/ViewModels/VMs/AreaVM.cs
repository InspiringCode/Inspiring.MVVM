namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public sealed class AreaVM : DefaultViewModelWithSourceBase<AreaVMDescriptor, Area> {
      public static readonly AreaVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<AreaVMDescriptor>()
         .For<AreaVM>()
         .WithProperties((d, c) => {
            var vm = c.GetPropertyBuilder();
            var a = c.GetPropertyBuilder(x => x.Source);

            d.Caption = a.Property.MapsTo(x => x.Caption);
         })
         .Build();

      public AreaVM()
         : base(ClassDescriptor) {
      }
   }

   public sealed class AreaVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Caption { get; set; }
   }
}
