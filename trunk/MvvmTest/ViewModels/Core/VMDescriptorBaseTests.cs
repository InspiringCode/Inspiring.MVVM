using System;
using Inspiring.Mvvm.ViewModels;
using Inspiring.Mvvm.ViewModels.Future;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.ViewModels.Core {
   [TestClass]
   public class VMDescriptorBaseTests {
      [TestMethod]
      public void Modify_CallTwoTimes_Succeeds() {
         TestVMDescriptor descriptor = new TestVMDescriptor();
         descriptor.Modify();
         descriptor.Modify();
      }

      [TestMethod]
      public void Modify_CallOnSealedInstance_ThrowsException() {
         TestVMDescriptor descriptor = new TestVMDescriptor();
         descriptor.Seal();

         AssertHelper.Throws<InvalidOperationException>(
            () => descriptor.Modify()
         );
      }

      [TestMethod]
      public void Properties_Get_ReturnsDiscoveredProperties() {
         var propertiesToReturn = new VMPropertyCollection(new VMPropertyBase[0]);
         var descriptor = new TestVMDescriptor(propertiesToReturn);

         var returnedProperties = descriptor.Properties;

         Assert.AreSame(propertiesToReturn, returnedProperties);
      }


      [TestMethod]
      public void Properties_Get_SealsDescriptor() {
         TestVMDescriptor descriptor = new TestVMDescriptor();
         var properties = descriptor.Properties;

         Assert.IsTrue(descriptor.IsSealed);
      }

      private class TestVMDescriptor : VMDescriptorBase {
         public TestVMDescriptor() {
            PropertiesToReturn = new VMPropertyCollection(new VMPropertyBase[0]);
         }

         public TestVMDescriptor(VMPropertyCollection propertiesToReturn) {
            PropertiesToReturn = propertiesToReturn;
         }

         public VMPropertyCollection PropertiesToReturn { get; private set; }

         protected override VMPropertyCollection DiscoverProperties() {
            return PropertiesToReturn;
         }
      }
   }
}
