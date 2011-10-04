namespace Inspiring.MvvmTest.Common {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Collections.Generic;
   using Inspiring.Mvvm.Common;

   [TestClass]
   public class ILParserTests {
      [TestMethod]
      public void GetAccessedProperties_IfOnlyPropertyIsAccessed_ReturnsPropertyInfo() {
         string[] props = GetAccessedProperties(new Func<TestClass, string>(x => x.TestProperty));
         Assert.AreEqual("TestProperty", props.SingleOrDefault());
      }

      [TestMethod]
      public void GetAccessedProperties_IfPropertyIsAccessedInSwitchStatement_ReturnsPropertyInfo() {
         int number = 0;

         Action action = () => {
            switch (number) {
               case 0:
                  Console.WriteLine("0");
                  break;
               case 1:
                  var obj = new TestClass();
                  obj.TestProperty = "Test";
                  break;
               default:
                  break;
            }
         };

         string[] props = GetAccessedProperties(action);
         Assert.AreEqual("TestProperty", props.SingleOrDefault());
      }

      private string[] GetAccessedProperties(Delegate del) {
         var parser = new ILParser(del.Method);
         
         IEnumerable<PropertyInfo> infos = parser.GetAccessedProperties();
         return infos
            .Select(x => x.Name)
            .ToArray();
      }

      private class TestClass {
         public string TestProperty { get; set; }

         public int TestMethod() {
            return 42;
         }
      }
   }
}