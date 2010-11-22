using System;
using System.ComponentModel;
using Inspiring.Mvvm.ViewModels;
using Inspiring.Mvvm.ViewModels.Core;
using Inspiring.MvvmTest.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Inspiring.MvvmTest.ViewModels.Core.TypeDescriptor {
   [TestClass]
   public class VMPropertyDescriptorTests {
      private VMPropertyBase _property;
      private PropertyDescriptor _descriptor;
      private IBehaviorContext _vm;

      [TestInitialize]
      public void Setup() {
         _property = new VMPropertyBaseStub(typeof(object), "Test");

         _descriptor = new VMPropertyDescriptor(_property);
         _vm = new Mock<IBehaviorContext>().Object;
      }

      [TestMethod]
      public void GetValue_CallsGetDisplayValue() {
         // Arrange
         object expectedReturnValue = "Test";

         var mock = new Mock<IDisplayValueAccessorBehavior>(MockBehavior.Strict);
         mock
            .Setup(x => x.GetDisplayValue(It.IsAny<IBehaviorContext>()))
            .Returns(expectedReturnValue);

         _property.Behaviors = new Behavior { Successor = mock.Object };

         // Act
         object result = _descriptor.GetValue(_vm);

         // Assert
         Assert.AreSame(expectedReturnValue, result);
      }

      [TestMethod]
      public void SetValue_CallsSetDisplayValue() {
         // Arrange
         object expectedValue = "Test";

         var mock = new Mock<IDisplayValueAccessorBehavior>(MockBehavior.Strict);
         mock.Setup(x => x.SetDisplayValue(It.IsAny<IBehaviorContext>(), expectedValue));

         _property.Behaviors = new Behavior { Successor = mock.Object };

         // Act
         _descriptor.SetValue(_vm, expectedValue);

         // Assert
         mock.Verify(
            x => x.SetDisplayValue(It.IsAny<IBehaviorContext>(), expectedValue),
            Times.Once()
         );
      }

      [TestMethod]
      public void ComponentType_ReturnsTypeOfIViewModel() {
         Assert.AreEqual(typeof(IViewModel), _descriptor.ComponentType);
      }

      [TestMethod]
      public void PropertyType_ReturnsPropertyTypeOfVMProperty() {
         Assert.AreEqual(_property.PropertyType, _descriptor.PropertyType);
      }

      [TestMethod]
      public void IsReadOnly_ReturnsFalse() {
         Assert.IsFalse(_descriptor.IsReadOnly);
      }

      [TestMethod]
      public void CanResetValue_ReturnsFalse() {
         Assert.IsFalse(_descriptor.CanResetValue(_vm));
      }

      [TestMethod]
      public void ResetValue_ThrowsException() {
         AssertHelper.Throws<NotSupportedException>(
            () => _descriptor.ResetValue(_vm)
         );
      }

      [TestMethod]
      public void ShouldSerializeValue_ReturnsTrue() {
         Assert.IsTrue(_descriptor.ShouldSerializeValue(_vm));
      }
   }
}
