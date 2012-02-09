namespace Inspiring.MvvmTest.Common {
   using System;
   using System.Linq;
   using System.Reflection;
   using Inspiring.Mvvm.Common;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using System.Linq.Expressions;

   [TestClass]
   public class ExpressionServiceTest : TestBase {
      [TestMethod]
      public void GetProperties() {
         PropertyInfo[] path = ExpressionService.GetProperties<Person, DateTime>(p => p.BirthDate);
         Assert.AreEqual(1, path.Length);
         Assert.AreEqual("BirthDate", path[0].Name);

         path = ExpressionService.GetProperties<Person, string>(p => p.Department.Name);
         Assert.AreEqual(2, path.Length);
         Assert.AreEqual("Department", path[0].Name);
         Assert.AreEqual("Name", path[1].Name);

         path = ExpressionService.GetProperties<Person, Person>(p => p);
         Assert.AreEqual(0, path.Length);

         AssertHelper.Throws<ArgumentException>(() => {
            ExpressionService.GetProperties<Person, DateTime>(p => p.GetBirthDate().Date);
         });
      }

      [TestMethod]
      public void GetPropertyName() {
         string name = ExpressionService.GetPropertyName<Person, DateTime>(p => p.BirthDate);
         Assert.AreEqual("BirthDate", name);

         name = ExpressionService.GetPropertyName<Person, Department>(p => p.Department);
         Assert.AreEqual("Department", name);

         AssertHelper.Throws<ArgumentException>(() =>
            ExpressionService.GetPropertyName<Person, string>(p => p.Department.Name)
         ).Containing("more");

         AssertHelper.Throws<ArgumentException>(() =>
            ExpressionService.GetPropertyName<Person, Person>(p => p)
         ).Containing("single");
      }

      [TestMethod]
      public void GetPropertyNameParameterless() {
         string name = ExpressionService.GetPropertyName(() => BirthDate);
         Assert.AreEqual("BirthDate", name);
      }

      [TestMethod]
      public void GetPropertyNameParameterless_StaticFieldInCallingClass_Succeeds() {
         var name = ExpressionService.GetPropertyName(() => StaticField);
         Assert.AreEqual("StaticField", name);
      }

      [TestMethod]
      public void GetPropertyNameParameterless_StaticFieldInExternalClass_Succeeds() {
         var name = ExpressionService.GetPropertyName(() => Person.DefaultDepartment);
         Assert.AreEqual("DefaultDepartment", name);
      }


      [TestMethod]
      public void GetPropertyName_WorksWithPropertiesWithDerivedTypes() {
         Expression<Func<Person, object>> exp = x => x.BirthDate;

         var path = ExpressionService.GetProperties(exp);
         Assert.AreEqual("BirthDate", ToString(path));

         exp = x => x.Department.Name;

         path = ExpressionService.GetProperties(exp);
         Assert.AreEqual("Department.Name", ToString(path));
      }

      private static string ToString(PropertyInfo[] infos) {
         return String.Join(".", infos.Select(x => x.Name));
      }
      
      private static readonly object StaticField = new Object();

      public DateTime BirthDate { get; set; }

      private class Person {
         public static readonly Department DefaultDepartment = null;

         public DateTime BirthDate { get; set; }
         public Department Department { get; set; }
         public DateTime GetBirthDate() {
            return BirthDate;
         }
      }

      private class Department {
         public string Name { get; set; }
      }
   }
}
