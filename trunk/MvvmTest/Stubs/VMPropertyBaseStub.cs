namespace Inspiring.MvvmTest.Stubs {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class VMPropertyBaseStub : VMPropertyBase {
      //public VMPropertyBaseStub() {
      //}

      public VMPropertyBaseStub(Type propertyType, string propertyName)
         : base(propertyType, propertyName) {
      }

      internal override void Revalidate(IBehaviorContext context) {
         throw new NotImplementedException("TODO");
      }

      protected override object GetValueCore(IBehaviorContext context, ValueStage stage) {
         throw new NotImplementedException();
      }

      protected override void SetValueCore(IBehaviorContext context, object value) {
         throw new NotImplementedException();
      }
   }
}
