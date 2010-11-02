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

      internal override void ConfigureBehaviors(BehaviorConfiguration configuration, FieldDefinitionCollection fieldDefinitions) {
         throw new NotImplementedException("TODO");
      }

      internal override void Revalidate(IBehaviorContext context) {
         throw new NotImplementedException("TODO");
      }
   }
}
