using Inspiring.Mvvm.ViewModels.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.DynamicFields {
   [TestClass]
   public class FieldDefinitionCollectionTest {
      [TestMethod]
      public void DefineFields() {
         FieldDefinitionCollection fieldDefinitions = new FieldDefinitionCollection();
         FieldDefinitionGroup valueGroup = new FieldDefinitionGroup();
         FieldDefinitionGroup errorGroup = new FieldDefinitionGroup();

         FieldDefinition<string> nameField = fieldDefinitions.DefineField<string>(valueGroup);
         FieldDefinition<decimal> salaryField = fieldDefinitions.DefineField<decimal>(valueGroup);
         FieldDefinition<string> salaryErrorField = fieldDefinitions.DefineField<string>(errorGroup);

         Assert.AreEqual(0, nameField.GroupIndex);
         Assert.AreEqual(0, salaryField.GroupIndex);
         Assert.AreEqual(1, salaryErrorField.GroupIndex);

         Assert.AreEqual(0, nameField.FieldIndex);
         Assert.AreEqual(1, salaryField.FieldIndex);
         Assert.AreEqual(0, salaryErrorField.FieldIndex);
      }
   }
}
