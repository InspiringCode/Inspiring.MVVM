namespace Inspiring.MvvmTest.ViewModels.IntegrationTests {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using System.ComponentModel;
   using Inspiring.Mvvm.ViewModels;

   [TestClass]
   public class ValidationTest {
      private TestVMSource _source;
      private TestVM _vm;

      [TestInitialize]
      public void Setup() {
         _source = new TestVMSource();
         _source.MappedMutableValue = "Test";
         _source.ChildValue = new ChildVMSource { MappedMutableValue = "Childtest" };
         _source.AddChild(new ChildVMSource { MappedMutableValue = "Item 1" });
         _source.AddChild(new ChildVMSource { MappedMutableValue = "Item 2" });
         _source.SetCalculated(42);

         _vm = new TestVM();
         _vm.Source = _source;
      }
      [TestMethod]
      public void TestDefault() {
         Assert.IsTrue(_vm.IsValid(true));
      }

      [TestMethod]
      public void TestParentError() {
         _vm.ViewModelValidationResult = ValidationResult.Failure("Test");
         Assert.IsFalse(_vm.IsValid(true));
      }

      [TestMethod]
      public void TestParentPropertyError() {
         _vm.LocalPropertyValidationResult = ValidationResult.Failure("Test");
         Assert.IsFalse(_vm.IsValid(true));
      }

      [TestMethod]
      public void TestChildError() {
         _vm.MappedCollectionAccessor[0].MappedMutablePropertyValidationResult = ValidationResult.Failure("Test");
         Assert.IsFalse(_vm.IsValid(true));
      }

      [TestMethod]
      public void TestValidateParentOnly() {
         _vm.MappedCollectionAccessor[0].MappedMutablePropertyValidationResult = ValidationResult.Failure("Test");
         Assert.IsTrue(_vm.IsValid(false));
      }
   }
}