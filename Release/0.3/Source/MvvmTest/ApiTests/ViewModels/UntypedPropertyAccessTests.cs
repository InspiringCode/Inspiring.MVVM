namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class UntypedPropertyAccessTests {
      private EmployeeVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new EmployeeVM();
         VM.SetValue(x => x.Name, "Initial name");
         VM.SetValue(x => x.SelectedProject, new ProjectVM());
      }

      [TestMethod]
      public void GetValue_OfSimpleProperty_ReturnsValue() {
         object untypedValue = GetUntypedValue(EmployeeVM.ClassDescriptor.Name);
         Assert.AreEqual(VM.GetValue(x => x.Name), untypedValue);
      }

      [TestMethod]
      public void GetValue_OfViewModelProperty_ReturnsViewModel() {
         object untypedValue = GetUntypedValue(EmployeeVM.ClassDescriptor.SelectedProject);
         Assert.AreEqual(VM.GetValue(x => x.SelectedProject), untypedValue);
      }

      [TestMethod]
      public void GetValue_OfCollectionProperty_ReturnsCollection() {
         object untypedValue = GetUntypedValue(EmployeeVM.ClassDescriptor.Projects);
         Assert.AreEqual(VM.GetValue(x => x.Projects), untypedValue);
      }

      [TestMethod]
      public void SetValue_OfSimpleProperty_SetsValue() {
         object untypedValue = "New name";
         SetUntypedValue(EmployeeVM.ClassDescriptor.Name, untypedValue);
         Assert.AreEqual(VM.GetValue(x => x.Name), untypedValue);
      }

      [TestMethod]
      public void SetValue_OfViewModelProperty_SetsValue() {
         object untypedValue = new ProjectVM();
         SetUntypedValue(EmployeeVM.ClassDescriptor.SelectedProject, untypedValue);
         Assert.AreEqual(VM.GetValue(x => x.SelectedProject), untypedValue);
      }

      [TestMethod]
      public void SetValue_WithInvalidDataType_ThrowsException() {
         AssertHelper.Throws<InvalidCastException>(() =>
            SetUntypedValue(EmployeeVM.ClassDescriptor.Name, 4711)
         );
      }

      private object GetUntypedValue(IVMPropertyDescriptor property) {
         IViewModel vm = VM;
         return vm.Kernel.GetValue(property);
      }

      private void SetUntypedValue(IVMPropertyDescriptor property, object value) {
         IViewModel vm = VM;
         vm.Kernel.SetValue(property, value);
      }

      public sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Name = v.Property.Of<string>();
               d.SelectedProject = v.VM.Of<ProjectVM>();
               d.Projects = v.Collection.Of<ProjectVM>(ProjectVM.ClassDescriptor);
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
         }
      }

      public sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<ProjectVM> SelectedProject { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
      }

      public sealed class ProjectVM : ViewModel<ProjectVMDescriptor> {
         public static readonly ProjectVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ProjectVMDescriptor>()
            .For<ProjectVM>()
            .WithProperties((d, b) => {
            })
            .Build();

         public ProjectVM()
            : base(ClassDescriptor) {
         }
      }

      public sealed class ProjectVMDescriptor : VMDescriptor {
      }
   }
}