namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class DisplayValueTests : ValidationTestBase {
      private const int ValidRatingValue = 3;
      private const int InvalidRatingValue = 6;

      private static readonly object ValidRatingDisplayValue = 4;
      private static readonly object InvalidRatingDisplayValue = 2.5m;

      private static readonly string InvalidDisplayValueValidationError =
         "Rating must be an integer (no decimal places)!";

      private static readonly string InvalidValueValidationError =
         "Rating must be between 1 and 5!";

      private MovieReviewVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new MovieReviewVM();
      }

      //[TestMethod] // TODO
      public void GetDisplayValue_Initially_ReturnsSourceValue() {
         VM.RatingSourceValue = ValidRatingValue;
         Assert.AreEqual(ValidRatingValue, VM.RatingDisplayValue);
      }

      //[TestMethod] // TODO
      public void GetDisplayValue_AfterInvalidValueWasSet_ReturnsInvalidValue() {
         VM.RatingDisplayValue = InvalidRatingDisplayValue;
         Assert.AreEqual(InvalidRatingDisplayValue, VM.RatingDisplayValue);
      }

      //[TestMethod] // TODO
      public void SetDisplayValue_ToValidValue_SetsSourceValue() {
         VM.RatingDisplayValue = InvalidRatingDisplayValue;
         VM.RatingDisplayValue = ValidRatingDisplayValue;

         Assert.AreEqual(ValidRatingDisplayValue, VM.RatingSourceValue);
      }

      //[TestMethod] // TODO
      public void SetDisplayValue_ToNull_SetsNullableSourceValueToNull() {
         VM.RatingSourceValue = ValidRatingValue;
         VM.RatingDisplayValue = null;

         Assert.IsNull(VM.RatingSourceValue);
      }

      //[TestMethod] // TODO
      public void SetDisplayValue_ToInvalidValue_DoesNotSetSourceValue() {
         VM.RatingSourceValue = ValidRatingValue;
         VM.RatingDisplayValue = InvalidRatingDisplayValue;

         Assert.AreEqual(ValidRatingValue, VM.RatingSourceValue);
      }

      //[TestMethod] // TODO
      public void SetDisplayValue_ToInvalidValue_RaisesPropertyChanged() {
         VM.RatingDisplayValue = InvalidRatingDisplayValue;

         Assert.AreEqual(1, VM.PropertyChangedCount, "PropertyChanged should be raised exactly once.");
         Assert.AreEqual("Rating", VM.ChangedPropertyName, "PropertyChanged should be raised for 'Rating'.");
      }

      //[TestMethod] // TODO
      public void SetDisplayValue_ToInvalidValue_AddsValidationError() {
         VM.RatingDisplayValue = InvalidRatingDisplayValue;

         var expected = CreateValidationState(InvalidDisplayValueValidationError);

         Assert.AreEqual(expected, VM.ValidationState);
      }

      //[TestMethod] // TODO
      public void SetDisplayValue_ToValidValue_ClearsValidationError() {
         VM.RatingDisplayValue = InvalidRatingDisplayValue;
         VM.RatingDisplayValue = ValidRatingDisplayValue;

         Assert.IsTrue(VM.ValidationState.IsValid);
      }

      //[TestMethod] // TODO
      public void SetDisplayValue_WithInvalidUnconvertedValue_AddsValidationError() {
         Assert.Inconclusive();
      }

      //[TestMethod] // TODO
      public void GetValidationState_AfterInvalidValueWasSet_ReturnsUnionOfOwnAndNextBehaviorValidationState() {
         VM.RatingDisplayValue = InvalidRatingDisplayValue;
         VM.RatingSourceValue = InvalidRatingValue;

         var expected = CreateValidationState(InvalidDisplayValueValidationError, InvalidValueValidationError);

         Assert.AreEqual(expected, VM.ValidationState);
      }

      //[TestMethod] // TODO
      public void SetSourceValue_AfterInvalidDisplayValueWasSet_ClearsValidationError() {
         VM.RatingDisplayValue = InvalidRatingDisplayValue;
         VM.RatingSourceValue = ValidRatingValue;

         Assert.IsTrue(VM.ValidationState.IsValid);
      }

      //[TestMethod] // TODO
      public void SetSourceValue_AfterInvalidDisplayValueWasSet_ClearsValueCache() {
         VM.RatingDisplayValue = InvalidRatingDisplayValue;
         VM.RatingSourceValue = ValidRatingValue;

         Assert.AreEqual(ValidRatingValue, VM.RatingDisplayValue);
      }

      private class MovieReviewVM : ViewModel<MovieReviewVMDescriptor> {
         public static readonly MovieReviewVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<MovieReviewVMDescriptor>()
            .For<MovieReviewVM>()
            .WithProperties((d, c) => {
               var vm = c.GetPropertyBuilder();

               d.Rating = vm.Property.Of<Nullable<int>>();
            })
            .Build();

         public MovieReviewVM()
            : base(ClassDescriptor) {
         }

         public Nullable<int> RatingSourceValue {
            get { return GetValidatedValue(Descriptor.Rating); }
            set { SetValue(Descriptor.Rating, value); }
         }

         public object RatingDisplayValue {
            get { return GetDisplayValue(Descriptor.Rating); }
            set { SetDisplayValue(Descriptor.Rating, value); }
         }

         public int PropertyChangedCount { get; set; }

         public string ChangedPropertyName { get; set; }

         public ValidationResult ValidationState {
            get { return Kernel.GetValidationState(Descriptor.Rating); }
         }

         protected override void OnPropertyChanged(string propertyName) {
            base.OnPropertyChanged(propertyName);
            PropertyChangedCount++;
            ChangedPropertyName = propertyName;
         }
      }

      public sealed class MovieReviewVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<Nullable<int>> Rating { get; set; }
      }
   }
}