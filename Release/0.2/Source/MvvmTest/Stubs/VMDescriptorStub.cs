namespace Inspiring.MvvmTest.Stubs {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;

   internal sealed class VMDescriptorStub : VMDescriptorBase {
      List<IVMPropertyDescriptor> _properties;

      public VMDescriptorStub() {
         _properties = new List<IVMPropertyDescriptor>();
      }

      public VMDescriptorStub(params IVMPropertyDescriptor[] properties) {
         _properties = new List<IVMPropertyDescriptor>(properties);
      }

      public void AddProperty(string name, Type type) {
         _properties.Add(new VMPropertyBaseStub(type, name));
      }

      protected override VMPropertyCollection DiscoverProperties() {
         return new VMPropertyCollection(_properties.ToArray());
      }
   }
}
