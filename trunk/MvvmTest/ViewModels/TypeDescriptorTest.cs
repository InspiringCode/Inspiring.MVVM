using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.ViewModels {
   [TestClass]
   public class TypeDescriptorTest {
      private Person _person;
      private PersonVM _vm;
      private PropertyDescriptorCollection _properties;

      [TestInitialize]
      public void Setup() {
         _person = SampleDataFactory.CreatePerson("John");
         _person.LastName = null;
         _vm = SampleDataFactory.CreatePersonVM(_person);
         _properties = TypeDescriptor.GetProperties(_vm);
      }

      // [TestMethod] // TODO
      public void CheckPropertyCount() {
         Assert.AreEqual(SampleDataFactory.PersonVMPropertyCount, _properties.Count);
      }

      [TestMethod]
      public void GetSetProperties() {
         Assert.AreEqual(_person.FirstName, _properties["FirstName"].GetValue(_vm));
         Assert.AreEqual(_person.LastName, _properties["LastName"].GetValue(_vm));
         Assert.AreEqual(_person.BirthDate, _properties["BirthDate"].GetValue(_vm));
         Assert.AreEqual(_person.Salary, _properties["Salary"].GetValue(_vm));

         _properties["LastName"].SetValue(_vm, "Robbinson");
         _properties["BirthDate"].SetValue(_vm, new DateTime(2010, 1, 1));

         Assert.AreEqual("Robbinson", _person.LastName);
         Assert.AreEqual(new DateTime(2010, 1, 1), _person.BirthDate);
      }
   }
}
