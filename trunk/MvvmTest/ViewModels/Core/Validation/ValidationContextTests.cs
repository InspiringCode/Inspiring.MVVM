namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using System;

   [TestClass]
   public class ValidationContextTests {
      private ValidationContext Context { get; set; }

      [TestInitialize]
      public void Setup() {
         ValidationContext.BeginValidation();
         Context = ValidationContext.Current;
      }

      [TestCleanup]
      public void Cleanup() {
         ValidationContext.CompleteValidation(ValidationMode.CommitValidValues);
      }

      [TestMethod]
      public void TryGetValidatorState_ReturnsRegisteredState() {
         Assert.Inconclusive();
      }

      [TestMethod]
      public void TryGetValidatorState_KeyNotRegistered_ReturnsNull() {
         Assert.Inconclusive();
      }

      [TestMethod]
      public void RegisterValidatorState_KeyAlreadyRegistered_ThrowsException() {
         Assert.Inconclusive();
         //string key = "KEY";
         //Context.SetValidationState(key, new TestValidatorState());

         //AssertHelper.Throws<ArgumentException>(() =>
         //   Context.SetValidationState(key, new TestValidatorState())
         //);
      }

      private class TestValidatorState {
      }
   }
}