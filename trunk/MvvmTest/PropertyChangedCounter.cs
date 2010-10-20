namespace Inspiring.MvvmTest {
   using System;
   using System.ComponentModel;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   internal sealed class PropertyChangedCounter {
      private int _assertedRaises = 0;

      public PropertyChangedCounter(INotifyPropertyChanged observableObject) {
         observableObject.PropertyChanged += (sender, e) => {
            Count++;
         };
      }
      public PropertyChangedCounter(
         INotifyPropertyChanged observableObject,
         string propertyName
      ) {
         observableObject.PropertyChanged += (sender, e) => {
            if (e.PropertyName == propertyName) {
               Count++;
            }
         };
      }

      public int Count { get; private set; }

      public void AssertOneRaise(string message = null) {
         _assertedRaises++;
         Assert.AreEqual(
            _assertedRaises,
            Count,
            "{0}The number of actual raisings of PropertyChanged ({1}) is not equal to " +
            "the expected number of raisings ({2}).",
            message != null ? " " + message : String.Empty,
            Count,
            _assertedRaises
         );
      }

      public void AssertNoRaise(string message = null) {
         Assert.AreEqual(
            _assertedRaises,
            Count,
            "{0}Did not expect a raising of PropertyChanged.",
            message != null ? " " + message : String.Empty
         );
      }
   }
}
