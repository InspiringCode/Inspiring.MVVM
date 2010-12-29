namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public sealed class AreaVM : ViewModel<AreaVMDescriptor>, ICanInitializeFrom<Area> {
      public static readonly AreaVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<AreaVMDescriptor>()
         .For<AreaVM>()
         .WithProperties((d, c) => {
            var vm = c.GetPropertyBuilder();
            var a = c.GetPropertyBuilder(x => x.Area);

            d.Caption = a.Property.MapsTo(x => x.Caption);
         })
         .Build();

      public AreaVM()
         : base(ClassDescriptor) {
      }

      public Area Area { get; private set; }

      public void InitializeFrom(Area source) {
         Area = source;
      }
   }

   public sealed class AreaVMDescriptor : VMDescriptor {
      public VMProperty<string> Caption { get; set; }
   }
}
