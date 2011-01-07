namespace Inspiring.MvvmTest.Stubs {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;

   internal sealed class VMDescriptorStub : VMDescriptorBase {
      List<IVMProperty> _properties;

      public VMDescriptorStub() {
         _properties = new List<IVMProperty>();
      }

      public VMDescriptorStub(params IVMProperty[] properties) {
         _properties = new List<IVMProperty>(properties);
      }

      public void AddProperty(string name, Type type) {
         _properties.Add(new VMPropertyBaseStub(type, name));
      }

      protected override VMPropertyCollection DiscoverProperties() {
         return new VMPropertyCollection(_properties.ToArray());
      }
   }
}
