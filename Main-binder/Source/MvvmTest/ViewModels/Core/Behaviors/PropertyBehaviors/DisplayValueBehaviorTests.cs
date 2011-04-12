namespace Inspiring.MvvmTest.ViewModels.Core.Behaviors.PropertyBehaviors {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class DisplayValueBehaviorTests {
      //[TestMethod]
      public void GetValue_Initially_ReturnsValueOfNextBehavior() {

      }

      //[TestMethod]
      public void GetValue_AfterInvalidValueWasSet_ReturnsInvalidValue() {

      }

      //[TestMethod]
      public void SetValue_ToValidValue_CallsNextBehavior() {

      }

      //[TestMethod]
      public void SetValue_SettingNullableTypeToNull_Succeeds() {

      }

      //[TestMethod]
      public void SetValue_ToInvalidValue_DoesNotCallNextBehavior() {

      }

      //[TestMethod]
      public void SetValue_ToValidValue_DoesNotCallNotifyChange() {

      }

      //[TestMethod]
      public void SetValue_ToInvalidValue_DoesCallNotifyChange() {

      }

      //[TestMethod]
      public void SetValue_ToInvalidValue_AddsValidationError() {

      }

      //[TestMethod]
      public void SetValue_ToValidValueAfterInvalid_ClearsValidationError() {

      }

      //[TestMethod]
      public void SetValue_WithInvalidUnconvertedValue_AddsValidationError() {

      }

      //[TestMethod]
      public void GetValidationState_AfterInvalidValueWasSet_ReturnsUnionOfOwnAndNextBehaviorValidationState() {

      }

      //[TestMethod]
      public void HandlePropertyChanged_AfterInvalidValueWasSet_ClearsValidationError() {

      }

      //[TestMethod]
      public void HandlePropertyChanged_AfterInvalidValueWasSet_ClearsValueCache() {

      }

      private ValidationState CreateValidationStateWithError() {
         throw new NotImplementedException();
      }

      private void SetupNextBehaviorsGetValueResult(int result) {
         throw new NotImplementedException();
      }

      private void SetupNextValidationStateProvidersResult(ValidationState result) {
         throw new NotImplementedException();
      }

      private void SetupContextToAddValidationError(ValidationError error) {
         throw new NotImplementedException();
      }

      private void SetupContextToAddNoValidationError() {
         throw new NotImplementedException();
      }

      private void AssertSetValueWasCalledOnNextBehavior(int withValue) {
         throw new NotImplementedException();
      }

      private void AssertSetValueWasNotCalled() {
         throw new NotImplementedException();
      }

      private void AssertNotifyChangeWasCalled() {
         throw new NotImplementedException();
      }

      private void AssertNotifyChangeWasNotCalled() {
         throw new NotImplementedException();
      }
   }
}