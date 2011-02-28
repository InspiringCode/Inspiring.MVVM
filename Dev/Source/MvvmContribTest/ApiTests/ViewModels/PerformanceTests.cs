namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class PerformanceTests : TestBase {
      private const int EmploymentTypeCount = 1000;
      private const int EmployeeCount = 20;
      private static readonly List<EmploymentType> AllEmploymentTypes = GenerateEmploymentTypes(EmploymentTypeCount);


      [TestMethod]
      public void TestMethod1() {
         var sw = Stopwatch.StartNew();

         for (int i = 0; i < EmployeeCount; i++) {
            var vm = new EmployeeVM("Employee " + i);
            vm.Initialize();
         }

         sw.Stop();

         Console.WriteLine("Time [ms]: " + sw.ElapsedMilliseconds);
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
               var v = c.GetPropertyBuilder();

               d.Name = v.Property.Of<string>();
               d.EmploymentType = v
                  .SingleSelection(x => x.EmploymentTypeSource)
                  .WithItems(x => AllEmploymentTypes)
                  .WithCaption(x => x.Name);
            })
            .Build();

         public EmployeeVM(string name)
            : base(ClassDescriptor) {
            Name = name;
            EmploymentTypeSource = AllEmploymentTypes[AllEmploymentTypes.Count / 2];
         }

         public string Name {
            get { return GetValue(Descriptor.Name); }
            private set { SetValue(Descriptor.Name, value); }
         }

         public SingleSelectionVM<EmploymentType> EmploymentType {
            get { return GetValue(Descriptor.EmploymentType); }
         }

         private EmploymentType EmploymentTypeSource {
            get;
            set;
         }

         public void Initialize() {
            Load(Descriptor.EmploymentType);
            var selectedItem = EmploymentType.SelectedItem;
         }
      }

      public sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<SingleSelectionVM<EmploymentType>> EmploymentType { get; set; }
      }

      public sealed class EmploymentType {
         public EmploymentType(string name) {
            Name = name;
         }

         public string Name { get; private set; }
      }
   }
}