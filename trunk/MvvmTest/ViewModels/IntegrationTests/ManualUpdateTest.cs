using System;
using System.ComponentModel;
using Inspiring.Mvvm.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.ViewModels {
   [TestClass]
   public class ManualUpdateTest {
      private TestVM _vm;
      private TestVMSource _source;

      [TestInitialize]
      public void Setup() {
         _source = new TestVMSource();
         _source.MappedMutableValue = "Test";
         _source.ChildValue = new ChildVMSource { MappedMutableValue = "Childtest" };
         _source.AddChild(new ChildVMSource { MappedMutableValue = "Item 1" });
         _source.SetCalculated(42);

         _vm = new TestVM();
         _vm.Source = _source;
      }

      // [TestMethod] // TODO
      public void CheckPrerequisites() {
         //AssertNoChange(TestVM.Descriptor.MappedMutableProperty, () => _source.MappedMutableValue = "New value");
         //AssertNoChange(TestVM.Descriptor.CalculatedMutableProperty, () => _source.SetCalculated(43));
         //AssertNoChange(TestVM.Descriptor.LocalProperty, () => { });
         AssertNoChange(TestVM.ClassDescriptor.MappedVMProperty, () => _source.ChildValue.MappedMutableValue = "Childtest new");
         AssertNoChange(TestVM.ClassDescriptor.MappedCollectionProperty, () => _source.AddChild(new ChildVMSource { MappedMutableValue = "New value" }));
         Assert.Inconclusive("Disconnected properties are not supported yet.");
      }

      // [TestMethod] // TODO
      public void UpdateFromSourceForMappedMutableProperty() {
         Assert.Inconclusive("Disconnected properties are not supported yet.");

         CheckUpdateFromSource(
            TestVM.ClassDescriptor.MappedMutableProperty,
            () => _source.MappedMutableValue = "New value"
         );
      }

      //[TestMethod] // TODO
      public void UpdateFromSourceForCalculatedMutableProperty() {
         Assert.Inconclusive("Disconnected properties are not supported yet.");

         CheckUpdateFromSource(
            TestVM.ClassDescriptor.CalculatedMutableProperty,
            () => _source.SetCalculated(43)
         );
      }

      //[TestMethod] // TODO
      public void UpdateFromSourceForLocalProperty() {
         Assert.Inconclusive("Disconnected properties are not supported yet.");

         AssertHelper.Throws<ArgumentException>(() =>
            _vm.InvokeUpdateFromSource(TestVM.ClassDescriptor.LocalProperty)
         );
      }

      [TestMethod]
      public void UpdateFromSourceForMappedVM() {
         CheckUpdateFromSource(
            TestVM.ClassDescriptor.MappedVMProperty,
            () => _source.ChildValue.MappedMutableValue = "New value"
         );
      }

      [TestMethod]
      public void UpdateFromSourceForMappedCollection() {
         CheckUpdateFromSource(
            TestVM.ClassDescriptor.MappedVMProperty,
            () => _source.AddChild(new ChildVMSource())
         );
      }

      [TestMethod]
      public void UpdateSourceForMappedMutableProperty() {
         CheckUpdateSource(
            TestVM.ClassDescriptor.MappedMutableProperty,
            "New value",
            () => _source.MappedMutableValue
         );
      }

      [TestMethod]
      public void UpdateSourceForCalculatedMutableProperty() {
         CheckUpdateSource(
            TestVM.ClassDescriptor.CalculatedMutableProperty,
            43,
            () => _source.CalculatedMutableValue
         );
      }

      //[TestMethod] // TODO
      public void UpdateSourceForLocalProperty() {
         AssertHelper.Throws<ArgumentException>(() =>
             _vm.InvokeUpdateSource(TestVM.ClassDescriptor.LocalProperty)
         );
      }

      public void UpdateSourceForMappedVM() {
         AssertHelper.Throws<NotSupportedException>(() =>
            _vm.InvokeUpdateSource(TestVM.ClassDescriptor.MappedVMProperty)
         );
      }

      public void UpdateSourceForMappedCollection() {
         AssertHelper.Throws<NotSupportedException>(() =>
            _vm.InvokeUpdateSource(TestVM.ClassDescriptor.MappedCollectionProperty)
         );
      }

      private void CheckUpdateFromSource<T>(VMPropertyBase<T> property, Action sourceMutator) {
         bool called = false;
         T propertyChangedValue = default(T);
         PropertyChangedEventHandler handler = delegate(object sender, PropertyChangedEventArgs args) {
            Assert.IsFalse(called);
            called = true;
            Assert.AreEqual(property.PropertyName, args.PropertyName);
            propertyChangedValue = _vm.GetValue(property);
         };
         _vm.PropertyChanged += handler;

         var before = _vm.GetValue(property);
         sourceMutator();
         _vm.InvokeUpdateFromSource(property);
         var after = _vm.GetValue(property);
         Assert.AreNotEqual(
            before,
            after,
            "Expected that property {0} has changed.",
            property.PropertyName
         );

         Assert.IsTrue(called);
         Assert.AreEqual(after, propertyChangedValue);

         _vm.PropertyChanged -= handler;
      }

      private void CheckUpdateFromSource<T>(VMCollectionProperty<T> property, Action sourceMutator) where T : IViewModel {
         var before = _vm.GetValue(property);
         sourceMutator();
         _vm.InvokeUpdateFromSource(property);
         var after = _vm.GetValue(property);
         CollectionAssert.AreNotEquivalent(
            before,
            after,
            "Expected that property {0} has changed.",
            property.PropertyName
         );
      }

      private void CheckUpdateSource<T>(
         VMPropertyBase<T> property,
         T newValue,
         Func<T> sourceValueGetter
      ) {
         var before = sourceValueGetter();
         _vm.SetValue(property, newValue);
         var after = sourceValueGetter();
         Assert.AreNotEqual(
            before,
            after,
            "Expected that source of property {0} has changed.",
            property.PropertyName
         );
      }

      private void AssertNoChange<T>(VMPropertyBase<T> property, Action testCode) {
         var before = _vm.GetValue(property);
         testCode();
         var after = _vm.GetValue(property);
         Assert.AreEqual(
            before,
            after,
            "Expected that property {0} does not change.",
            property.PropertyName
         );
      }
   }
}
