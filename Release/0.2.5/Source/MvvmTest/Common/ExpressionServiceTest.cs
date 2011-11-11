﻿namespace Inspiring.MvvmTest.Common {
   using System;
   using System.Reflection;
   using Inspiring.Mvvm.Common;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ExpressionServiceTest {
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

      public DateTime BirthDate { get; set; }

      private class Person {
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