﻿namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public sealed class AreaVM : ViewModel<AreaVMDescriptor>, ICanInitializeFrom<Area> {
      public static readonly AreaVMDescriptor Descriptor = VMDescriptorBuilder
         .For<AreaVM>()
         .CreateDescriptor(c => {
            var vm = c.GetPropertyFactory();
            var a = c.GetPropertyFactory(x => x.Area);

            return new AreaVMDescriptor {
               Caption = a.Mapped(x => x.Caption).Property()
            };
         })
         .Build();

      public AreaVM(IServiceLocator serviceLocator)
         : base(Descriptor) {
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