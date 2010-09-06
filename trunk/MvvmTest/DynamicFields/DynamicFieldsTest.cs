using Inspiring.Mvvm.ViewModels.Behaviors;
namespace Inspiring.MvvmTest.DynamicFields {
   using System;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class DynamicFieldsTest {
      private FieldDefinitionCollection _fieldDefinitions;

      private FieldDefinitionGroup _valueGroup;
      private FieldDefinitionGroup _errorGroup;

      private FieldDefinition<string> _nameField;
      private FieldDefinition<decimal> _salaryField;
      private FieldDefinition<Address> _addressField;
      private FieldDefinition<string> _salaryErrorField;

      [TestInitialize]
      public void Initialize() {
         _fieldDefinitions = new FieldDefinitionCollection();
         _valueGroup = new FieldDefinitionGroup();
         _errorGroup = new FieldDefinitionGroup();

         _nameField = _fieldDefinitions.DefineField<string>(_valueGroup);
         _salaryField = _fieldDefinitions.DefineField<decimal>(_valueGroup);
         _addressField = _fieldDefinitions.DefineField<Address>(_valueGroup);

         _salaryErrorField = _fieldDefinitions.DefineField<string>(_errorGroup);
      }

      [TestMethod]
      public void GetValueBeforeSet() {
         FieldValueHolder holder = _fieldDefinitions.CreateValueHolder();
         string name;
         decimal salary;

         AssertHelper.Throws<InvalidOperationException>(() => name = holder.GetValue(_nameField));
         AssertHelper.Throws<InvalidOperationException>(() => salary = holder.GetValue(_salaryField));

         name = holder.GetValueOrDefault(_nameField);
         salary = holder.GetValueOrDefault(_salaryField);

         Assert.IsFalse(holder.HasValue(_nameField));
         Assert.IsFalse(holder.HasValue(_salaryField));

         Assert.AreEqual(null, name);
         Assert.AreEqual(0m, salary);
      }

      [TestMethod]
      public void GetAndSetValues() {
         FieldValueHolder billHolder = _fieldDefinitions.CreateValueHolder();
         FieldValueHolder johnHolder = _fieldDefinitions.CreateValueHolder();

         Address billAddress = new Address();
         Address johnAddress = new Address();

         billHolder.SetValue(_nameField, "Bill");
         billHolder.SetValue(_salaryField, 20000m);
         billHolder.SetValue(_addressField, billAddress);
         billHolder.SetValue(_salaryErrorField, "Salary too high!");

         johnHolder.SetValue(_nameField, "John");
         johnHolder.SetValue(_addressField, johnAddress);

         Assert.AreEqual("Bill", billHolder.GetValue(_nameField));
         Assert.AreEqual(20000m, billHolder.GetValue(_salaryField));
         Assert.AreEqual(billAddress, billHolder.GetValue(_addressField));
         Assert.AreEqual("Salary too high!", billHolder.GetValue(_salaryErrorField));

         Assert.AreEqual("John", johnHolder.GetValue(_nameField));
         Assert.AreEqual(johnAddress, johnHolder.GetValue(_addressField));
         Assert.AreEqual(0m, johnHolder.GetValueOrDefault(_salaryField));
         Assert.AreEqual(null, johnHolder.GetValueOrDefault(_salaryErrorField));

         Assert.IsTrue(johnHolder.HasValue(_nameField));
         Assert.IsTrue(johnHolder.HasValue(_addressField));
         Assert.IsFalse(johnHolder.HasValue(_salaryField));
         Assert.IsFalse(johnHolder.HasValue(_salaryErrorField));

      }

      private class Address {

      }
   }
}
