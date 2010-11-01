namespace Inspiring.MvvmTest.Stubs {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Future;

   internal sealed class VMDescriptorStub : VMDescriptorBase {
      List<VMPropertyBase> _properties;

      public VMDescriptorStub() {
         _properties = new List<VMPropertyBase>();
      }

      public VMDescriptorStub(params VMPropertyBase[] properties) {
         _properties = new List<VMPropertyBase>(properties);
      }

      public void AddProperty(string name, Type type) {
         _properties.Add(new VMPropertyBaseStub(name, type));
      }

      protected override VMPropertyCollection DiscoverProperties() {
         return new VMPropertyCollection(_properties.ToArray());
      }
   }
}
