using Inspiring.MvvmTest.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.DynamicFields {
   [TestClass]
   public class FieldValueHolderTest : TestBase {
      [TestMethod]
      public void TestMethod1() {
         //FieldDefinitionCollection fieldDefinitions = new FieldDefinitionCollection();
         //FieldDefinitionGroup valueGroup = new FieldDefinitionGroup();
         //FieldDefinitionGroup errorGroup = new FieldDefinitionGroup();

         //FieldDefinition<string> nameField = fieldDefinitions.DefineField<string>(valueGroup);
         //FieldDefinition<decimal> salaryField = fieldDefinitions.DefineField<decimal>(valueGroup);
         //FieldDefinition<DateTime> birthDateField = fieldDefinitions.DefineField<DateTime>(valueGroup);
         //FieldDefinition<string> salaryErrorField = fieldDefinitions.DefineField<string>(errorGroup);

         //FieldValueHolder holder = fieldDefinitions.CreateValueHolder();

         //string propertyName;
         //decimal salary;
         //DateTime birthDate;
         //string salaryError;

         //Assert.IsFalse(holder.TryGetValue(nameField, out propertyName));
         //Assert.IsFalse(holder.TryGetValue(birthDateField, out birthDate));
         //Assert.IsFalse(holder.TryGetValue(salaryErrorField, out salaryError));

         //holder.SetValue(nameField, "Bill");
         //holder.SetValue(salaryField, 20000);
         //holder.SetValue(salaryErrorField, "Salary too high!");

         //Assert.IsTrue(holder.TryGetValue(nameField, out propertyName));
         //Assert.IsTrue(holder.TryGetValue(salaryField, out salary));
         //Assert.IsFalse(holder.TryGetValue(birthDateField, out birthDate));
         //Assert.IsTrue(holder.TryGetValue(salaryErrorField, out salaryError));

         //Assert.AreEqual("Bill", propertyName);
         //Assert.AreEqual(20000m, salary);
         //Assert.AreEqual(default(DateTime), birthDate);
         //Assert.AreEqual("Salary too high!", salaryError);
      }
   }
}
