//using System;
//using System.ComponentModel;
//using Inspiring.Mvvm.ViewModels;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace Inspiring.MvvmTest.ViewModels.IntegrationTests : TestBase {
//   [TestClass]
//   public class ChangeNotificationTest : TestBase {
//      private TestVM _vm;
//      private TestVMSource _source;

//      [TestInitialize]
//      public void Setup() {
//         _source = new TestVMSource();
//         _source.MappedMutableValue = "Test";
//         _source.ChildValue = new ChildVMSource { MappedMutableValue = "Childtest" };
//         _source.AddChild(new ChildVMSource { MappedMutableValue = "Item 1" });
//         _source.SetCalculated(42);

//         _vm = new TestVM();
//         _vm.Source = _source;
//      }


//      [TestMethod]
//      public void SetMappedPropertyToDifferentValue() {
//         AssertPropertyChanged(TestVM.Descriptor.MappedMutableProperty, "New value", true);
//         AssertValueChangedForTypeDescriptor(TestVM.Descriptor.MappedMutableProperty, "Other value", true);
//      }

//      [TestMethod]
//      public void SetCalculatedPropertyToDifferentValue() {
//         AssertPropertyChanged(TestVM.Descriptor.CalculatedMutableProperty, 43, true);
//         AssertValueChangedForTypeDescriptor(TestVM.Descriptor.CalculatedMutableProperty, 44, true);
//      }

//      [TestMethod]
//      public void SetLocalPropertyToDifferentValue() {
//         // Property is not initialized (implicitly 0.0)
//         AssertPropertyChanged(TestVM.Descriptor.LocalProperty, 42, true);
//         AssertPropertyChanged(TestVM.Descriptor.LocalProperty, 43, true);

//      }

//      [TestMethod]
//      public void SetLocalPropertyToDifferentDisplayValue() {
//         // Property is not initialized (implicitly 0.0)
//         AssertValueChangedForTypeDescriptor(TestVM.Descriptor.LocalProperty, 42, true);
//         AssertValueChangedForTypeDescriptor(TestVM.Descriptor.LocalProperty, 43, true);

//      }

//      [TestMethod]
//      public void SetMappedPropertyToSameValue() {
//         AssertPropertyChanged(TestVM.Descriptor.MappedMutableProperty, "Test", false);
//         AssertValueChangedForTypeDescriptor(TestVM.Descriptor.MappedMutableProperty, "Test", false);
//      }

//      [TestMethod]
//      public void SetCalculatedPropertyToSameValue() {
//         AssertPropertyChanged(TestVM.Descriptor.CalculatedMutableProperty, 42, false);
//         AssertValueChangedForTypeDescriptor(TestVM.Descriptor.CalculatedMutableProperty, 42, false);
//      }

//      [TestMethod]
//      public void SetLocalPropertyToSameValue() {
//         // Property is not initialized (implicitly 0.0)
//         AssertPropertyChanged(TestVM.Descriptor.LocalProperty, default(decimal), false);
//      }

//      [TestMethod]
//      public void SetLocalPropertyToSameDisplayValue() {
//         // Property is not initialized (implicitly 0.0)
//         AssertValueChangedForTypeDescriptor(TestVM.Descriptor.LocalProperty, default(decimal), false);
//      }

//      private void AssertPropertyChanged<T>(VMPropertyBase<T> property, T newValue, bool assertEvent) {
//         bool called = false;
//         PropertyChangedEventHandler handler = delegate(object sender, PropertyChangedEventArgs args) {
//            Assert.IsFalse(called);
//            called = true;
//            Assert.AreEqual(property.PropertyName, args.PropertyName);
//            Assert.AreEqual(newValue, _vm.GetValue(property));
//         };

//         _vm.PropertyChanged += handler;
//         var before = _vm.GetValue(property);
//         _vm.SetValue(property, newValue);
//         Assert.AreEqual(assertEvent, called);

//         _vm.PropertyChanged -= handler;
//      }

//      private void AssertValueChangedForTypeDescriptor<T>(VMPropertyBase<T> property, T newValue, bool assertEvent) {
//         bool called = false;
//         EventHandler handler = delegate(object sender, EventArgs args) {
//            Assert.IsFalse(called);
//            called = true;
//            Assert.AreEqual(newValue, property.GetDisplayValue(_vm));
//         };

//         TypeDescriptor.GetProperties(_vm)[property.PropertyName].AddValueChanged(_vm, handler);

//         var before = property.GetDisplayValue(_vm);
//         property.SetDisplayValue(_vm, newValue);
//         Assert.AreEqual(assertEvent, called);

//         TypeDescriptor.GetProperties(_vm)[property.PropertyName].RemoveValueChanged(_vm, handler);
//      }


//   }
//}
