namespace Inspiring.MvvmTest.Stubs {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class VMPropertyBaseStub : VMPropertyDescriptor {
      //public VMPropertyBaseStub() {
      //}

      public VMPropertyBaseStub(Type propertyType, string propertyName)
         : base(propertyType, propertyName) {
      }

      protected override object GetValueCore(IBehaviorContext context) {
         throw new NotImplementedException();
      }

      protected override void SetValueCore(IBehaviorContext context, object value) {
         throw new NotImplementedException();
      }
   }
}
