using System;
using Inspiring.Mvvm.Common;
using Inspiring.MvvmTest.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.Common {
   [TestClass]
   public class PropertyPathTest : TestBase {
      [TestMethod]
      public void GetSetValue() {
         Employee person = new Employee {
            FirstName = "Franz",
            LastName = "Huber",
            Address = new Address {
               City = "NY"
            }
         };

         var path1 = PropertyPath.Create<Person, string>(p => p.FirstName);
         Assert.AreEqual("Franz", path1.GetValue(person));
         path1.SetValue(person, "Hans");
         Assert.AreEqual("Hans", person.FirstName);

         var path2 = PropertyPath.Create<Employee, string>(p => p.LastName);
         Assert.AreEqual("Huber", path2.GetValue(person));
         path2.SetValue(person, "Maier");
         Assert.AreEqual("Maier", person.LastName);

         var path3 = PropertyPath.Create<Employee, string>(p => p.Address.City);
         Assert.AreEqual("NY", path3.GetValue(person));
         path3.SetValue(person, "AZ");
         Assert.AreEqual("AZ", person.Address.City);

         person.Address = null;
         AssertHelper.Throws<NullReferenceException>(() =>
            path3.GetValue(person)
         )
         .Containing("'[Employee].Address'")
         .Containing("'[Employee].Address.City'");

         var path4 = PropertyPath.CreateWithDefaultValue<Employee, string>(p => p.Address.City, "-");
         Assert.AreEqual("-", path4.GetValue(person));

         // SetValue should be ignored...
         path4.SetValue(person, "Test");
         Assert.AreEqual(null, person.Address);

         var path5 = PropertyPath.Create<Person, Person>(p => p);
         Assert.AreEqual(person, path5.GetValue(person));
         AssertHelper.Throws<InvalidOperationException>(
            () => path5.SetValue(person, person)
         ).Containing("empty");

         path5 = PropertyPath.Empty<Person>();
         Assert.AreEqual(person, path5.GetValue(person));
         AssertHelper.Throws<InvalidOperationException>(
            () => path5.SetValue(person, person)
         ).Containing("empty");
      }

      [TestMethod]
      public void Concat() {
         var path1 = PropertyPath.Create<Employee, Address>(p => p.Address);
         var path2 = PropertyPath.Create<Address, string>(a => a.City);

         var path3 = PropertyPath.Concat(path1, path2);
         TestAddressPath(path3);

         var emptyEmployeePath = PropertyPath.Empty<Employee>();
         var emptyStringPath = PropertyPath.Create<string, string>(s => s);

         var path4 = PropertyPath.Concat(emptyEmployeePath, path3);
         var path5 = PropertyPath.Concat(path3, emptyStringPath);
         TestAddressPath(path4);
         TestAddressPath(path5);
      }

      private void TestAddressPath(PropertyPath<Employee, string> path) {
         Employee person = new Employee {
            FirstName = "Franz",
            LastName = "Huber",
            Address = new Address {
               City = "NY"
            }
         };

         Assert.AreEqual("NY", path.GetValue(person));

         path.SetValue(person, "LA");
         Assert.AreEqual("LA", person.Address.City);
      }

      private class Person {
         public string FirstName { get; set; }
         public string LastName { get; set; }
         private int Age { get; set; }
      }

      private class Employee : Person {
         public Address Address { get; set; }
      }

      private class Address {
         public string City { get; set; }
      }
   }
}
