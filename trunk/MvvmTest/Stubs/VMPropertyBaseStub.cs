namespace Inspiring.MvvmTest.Stubs {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class VMPropertyBaseStub : VMPropertyBase {
      public VMPropertyBaseStub() {
      }

      public VMPropertyBaseStub(string propertyName, Type propertyType)
         : base(propertyName, propertyType) {
      }

      internal override void ConfigureBehaviors(BehaviorConfiguration configuration) {
         throw new NotImplementedException("TODO");
      }

      internal override void Revalidate(IBehaviorContext context) {
         throw new NotImplementedException("TODO");
      }
   }
}
