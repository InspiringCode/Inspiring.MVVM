namespace Inspiring.MvvmTest.TestUtils {
   using System;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class GenericComparisonObjectTests {
      private const string FirstEmployeeName = "John";
      private const string SecondEmployeeName = "Mike";
      private static readonly DateTime FirstEmployeeBirthDate = new DateTime(1985, 12, 27, 23, 59, 59);
      private static readonly DateTime SecondEmployeeBirthDate = new DateTime(1976, 1, 12, 11, 30, 0);

      [TestMethod]
      public void ToString_PrintsPropertyValues() {
         var co = CreateFirstEmployeeCO();

         var expected = "{ Name: " + FirstEmployeeName + ", BirthDate: " + FirstEmployeeBirthDate + " }";
         var actual = co.ToString();

         Assert.AreEqual(expected, actual);
      }

      [TestMethod]
      public void ToString_WithNullValue_PrintsPropertyValues() {
         var co = new EmployeeCO(new Employee());

         var expected = "{ Name: <NULL>, BirthDate: " + default(DateTime) + " }";
         var actual = co.ToString();

         Assert.AreEqual(expected, actual);
      }

      [TestMethod]
      public void Equals_WithEqualPropertyValues_ReturnsTrue() {
         var x = CreateFirstEmployeeCO();
         var y = CreateFirstEmployeeCO();

         Assert.AreEqual(x, y);
      }

      [TestMethod]
      public void Equals_WithOneDifferentPropertyValue_ReturnsFalse() {
         var x = CreateFirstEmployeeCO();
         var y = CreateSecondEmployeeCO();

         Assert.AreNotEqual(x, y);
      }

      [TestMethod]
      public void GetHashCode_OfTwoEqualObjects_ReturnsTheSameHashCode() {
         var x = CreateFirstEmployeeCO();
         var y = CreateFirstEmployeeCO();

         Assert.AreEqual(x.GetHashCode(), y.GetHashCode());
      }

      [TestMethod]
      public void GetHashCode_OfTwoUnequalObjects_ReturnsDiferrentHashCodes() {
         var x = CreateFirstEmployeeCO();
         var y = CreateSecondEmployeeCO();

         Assert.AreNotEqual(x.GetHashCode(), y.GetHashCode());
      }

      private static EmployeeCO CreateFirstEmployeeCO() {
         return new EmployeeCO(new Employee { 
            Name = FirstEmployeeName, 
            BirthDate = FirstEmployeeBirthDate 
         });
      }

      private static EmployeeCO CreateSecondEmployeeCO() {
         return new EmployeeCO(new Employee {
            Name = SecondEmployeeName,
            BirthDate = SecondEmployeeBirthDate
         });
      }

      private class EmployeeCO : ComparisonObject<Employee> {
         public EmployeeCO(Employee employee)
            : base(employee, x => x.Name, x => x.BirthDate) {
         }
      }

      private class Employee {
         public string Name { get; set; }
         public DateTime BirthDate { get; set; }
      }
   }
}
