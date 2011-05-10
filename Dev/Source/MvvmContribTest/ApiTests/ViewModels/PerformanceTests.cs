namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class PerformanceTests : TestBase {
      private const int EmploymentTypeCount = 20;
      private const int EmployeeCount = 100;
      private static readonly List<EmploymentType> AllEmploymentTypes = GenerateEmploymentTypes(EmploymentTypeCount);
      private static readonly Random Random = new Random();


      [TestMethod]
      public void TestMethod1() {
         IEnumerable<Employee> source = GenerateEmployees();

         var list = new EmployeeListVM();

         var sw = Stopwatch.StartNew();

         list = new EmployeeListVM();
         list.Source = source;
         list.Revalidate(ValidationScope.SelfAndLoadedDescendants);

         foreach (EmployeeVM item in list.GetValue(x => x.Employees)) {
            var selection = item.GetValue(x => x.EmploymentType);
            object simulatedBindingAccess = item.GetValue(x => x.Name);
            simulatedBindingAccess = selection.GetValue(x => x.AllItems);
            simulatedBindingAccess = selection.GetValue(x => x.SelectedItem);
         }

         //sw.Stop();
         //Console.WriteLine("Time [ms]: " + sw.ElapsedMilliseconds);
         //sw = Stopwatch.StartNew();
         //list.Revalidate(ValidationScope.SelfAndAllDescendants);

         sw.Stop();

         Console.WriteLine("Time [ms]: " + sw.ElapsedMilliseconds);
      }

      [TestMethod]
      public void TestPerformanceOfGetValidationResult() {
         IEnumerable<Employee> source = GenerateEmployees();
         var list = new EmployeeListVM();
         
         list = new EmployeeListVM();
         list.Source = source;
         //list.Revalidate(ValidationScope.SelfAndAllDescendants);

         foreach (EmployeeVM item in list.GetValue(x => x.Employees)) {
            var selection = item.GetValue(x => x.EmploymentType);
            object simulatedBindingAccess = item.GetValue(x => x.Name);
            simulatedBindingAccess = selection.GetValue(x => x.AllItems);
            simulatedBindingAccess = selection.GetValue(x => x.SelectedItem);
         }

         //var sw = Stopwatch.StartNew();
         //var result = list.GetValidationResult(ValidationResultScope.All);
         //sw.Stop();

         //Console.WriteLine("Time [ms]: " + sw.ElapsedMilliseconds);
      }

      private static IEnumerable<Employee> GenerateEmployees() {
         return Enumerable
            .Range(1, EmployeeCount)
            .Select(i => new Employee {
               Name = "Employee " + i,
               Type = GetRandomEmployeeType()
            })
            .ToArray();
      }

      private static EmploymentType GetRandomEmployeeType() {
         int i = Random.Next(0, AllEmploymentTypes.Count);
         return AllEmploymentTypes[i];
      }

      private static List<EmploymentType> GenerateEmploymentTypes(int EmploymentTypeCount) {
         List<EmploymentType> list = new List<EmploymentType>();

         for (int i = 0; i < EmploymentTypeCount; i++) {
            list.Add(new EmploymentType { Name = "Employment type " + i });
         }

         return list;
      }


      private class EmployeeListVM : ViewModel<EmployeeListVMDescriptor> {
         public static readonly EmployeeListVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeListVMDescriptor>()
            .For<EmployeeListVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Employees = v
                  .Collection
                  .Wraps(x => x.Source)
                  .With<EmployeeVM>(EmployeeVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.CheckCollection(x => x.Employees, x => x.Name).IsUnique("Duplicate item");
            })
            .Build();

         public EmployeeListVM()
            : base(ClassDescriptor) {
         }

         public IEnumerable<Employee> Source { get; set; }
      }

      private class EmployeeListVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<EmployeeVM>> Employees { get; set; }
      }

      private class EmployeeVM : DefaultViewModelWithSourceBase<EmployeeVMDescriptor, Employee> {
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, c) => {
               var e = c.GetPropertyBuilder(x => x.Source);

               d.Name = e.Property.MapsTo(x => x.Name);
               d.EmploymentType = e
                  .SingleSelection(x => x.Type)
                  .WithItems(x => AllEmploymentTypes)
                  .WithCaption(x => x.Name);
            })
            .WithValidators(b => {
               b.EnableParentValidation(x => x.Name);
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
         }

         public SingleSelectionVM<EmploymentType> EmploymentType {
            get { return GetValue(Descriptor.EmploymentType); }
         }
      }

      private class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<SingleSelectionVM<EmploymentType>> EmploymentType { get; set; }
      }

      private class Employee {
         public string Name { get; set; }
         public EmploymentType Type { get; set; }
      }

      private class EmploymentType {
         public string Name { get; set; }
      }
   }
}