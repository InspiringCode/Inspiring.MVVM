namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.ViewModels;
using System.Collections.Generic;

   [TestClass]
   public class PerformanceTests {
      private const int EmploymentTypeCount = 1000;
      private static readonly List<EmploymentType> EmploymentTypes = GenerateEmploymentTypes(EmploymentTypeCount);

      [TestMethod]
      public void TestMethod1() {

      }
      
      private static List<EmploymentType> GenerateEmploymentTypes(int EmploymentTypeCount) {
         List<EmploymentType> list = new List<EmploymentType>();

         for (int i = 0; i < EmploymentTypeCount; i++) {
            list.Add(new EmploymentType("Employment type " + i));
         }

         return list;
      }

      public sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, c) => {
               var vm = c.GetPropertyBuilder();
               d.Name = vm.Property.Of<string>();
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
         }
      }

      public sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
      }

      public sealed class EmploymentType {
         public EmploymentType(string name) {
            Name = name;
         }

         public string Name { get; private set; }
      }
   }
}