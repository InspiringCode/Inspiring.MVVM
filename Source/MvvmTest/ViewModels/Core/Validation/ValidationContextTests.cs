//namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
//   using Inspiring.Mvvm.ViewModels;
//   using Inspiring.Mvvm.ViewModels.Core;
//   using Microsoft.VisualStudio.TestTools.UnitTesting;
//   using System;

//   [TestClass]
//   public class ValidationContextTests {
//      private ValidationContext Context { get; set; }

//      [TestInitialize]
//      public void Setup() {
//         ValidationContext.BeginValidation();
//         Context = ValidationContext.Current;
//      }

//      [TestCleanup]
//      public void Cleanup() {
//         ValidationContext.CompleteValidation(ValidationMode.CommitValidValues);
//      }

//      [TestMethod]
//      public void TryGetValidatorState_ReturnsRegisteredState() {
//         var key = "TEST";
//         var expectedState = new TestValidatorState();
//         Context.SetValidatorState(key, expectedState);

//         var actual = Context.TryGetValidatorState<TestValidatorState>(key);
//         Assert.AreSame(expectedState, actual);
//      }

//      [TestMethod]
//      public void TryGetValidatorState_KeyNotRegistered_ReturnsNull() {
//         var state = Context.TryGetValidatorState<TestValidatorState>(key: "TEST");
//         Assert.IsNull(state);
//      }

//      [TestMethod]
//      public void TryGetValidatorState_WithWrongTypeArgument_ThrowsInvalidCastOperation() {
//         var key = "TEST";
//         Context.SetValidatorState(key, new TestValidatorState());

//         AssertHelper.Throws<InvalidCastException>(() =>
//            Context.TryGetValidatorState<AnotherValidatorState>(key)
//         );
//      }

//      [TestMethod]
//      public void RegisterValidatorState_KeyAlreadyRegistered_ThrowsException() {
//         var key = "TEST";
//         Context.SetValidatorState(key, new TestValidatorState());

//         AssertHelper.Throws<ArgumentException>(() =>
//            Context.SetValidatorState(key, new TestValidatorState())
//         );
//      }

//      private class TestValidatorState {
//      }

//      private class AnotherValidatorState {
//      }
//   }
//}