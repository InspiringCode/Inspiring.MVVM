namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class VMDescriptorTest {
      //[TestMethod]
      //public void TestInitialization() {
      //   PersonVMDescriptor d = new PersonVMDescriptor();

      //   Assert.IsNotNull(d.Name);
      //   Assert.IsNotNull(d.BirthDate);
      //   Assert.IsNotNull(d.Salary);

      //   Assert.AreEqual("Name", d.Name.PropertyName);
      //   Assert.AreEqual("BirthDate", d.BirthDate.PropertyName);
      //   Assert.AreEqual("Salary", d.Salary.PropertyName);
      //}

      private class PersonVMDescriptor : VMDescriptor {
         public VMProperty<string> Name { get; private set; }
         public VMProperty<DateTime> BirthDate { get; private set; }
         public VMProperty<decimal> Salary { get; private set; }
      }
   }
}
