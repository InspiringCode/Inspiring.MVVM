namespace Inspiring.Mvvm.Common {
   using System;
   using System.ComponentModel;
   using System.Linq.Expressions;

   public abstract class NotifyObject : INotifyPropertyChanged {
      public event PropertyChangedEventHandler PropertyChanged;

      protected void OnPropertyChanged<T>(Expression<Func<T>> propertySelector) {
         string propertyName = ExpressionService.GetPropertyName(propertySelector);
         OnPropertyChanged(propertyName);
      }

      protected void OnPropertyChanged(string propertyName) {
         PropertyChangedEventHandler handler = PropertyChanged;
         if (handler != null) {
            handler(this, new PropertyChangedEventArgs(propertyName));
         }
      }
   }
}
