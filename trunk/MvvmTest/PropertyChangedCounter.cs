namespace Inspiring.MvvmTest {
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

      public void AssertOneRaise() {
         _assertedRaises++;
         Assert.AreEqual(
            _assertedRaises,
            Count,
            "The number of actual raisings of PropertyChanged ({0}) is not equal to " +
            "the expected number of raisings ({1}).",
            Count,
            _assertedRaises
         );
      }

      public void AssertNoRaise() {
         Assert.AreEqual(_assertedRaises, Count, "Did not expect a raising of PropertyChanged.");
      }
   }
}
