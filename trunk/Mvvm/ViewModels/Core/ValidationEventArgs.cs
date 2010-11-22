namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   [Obsolete("Replaced by 'ValidationArgs'.")]
   public sealed class ValidationEventArgs : EventArgs {
      private bool _affectsOtherItems = false;

      public ValidationEventArgs(
         VMPropertyBase property,
         object propertyValue,
         IViewModel viewModel
      ) {
         Property = property;
         PropertyValue = propertyValue;
         VM = viewModel;
         Errors = new List<string>();
      }

      public VMPropertyBase Property { get; private set; }

      public object PropertyValue { get; private set; }

      public IViewModel VM { get; private set; }

      public bool AffectsOtherItems {
         get { return _affectsOtherItems; }
         set {
            if (_affectsOtherItems == false) {
               _affectsOtherItems = value;
            }
         }
      }

      internal List<string> Errors { get; private set; }

      internal void ClearAffectsOtherItems() {
         _affectsOtherItems = false;
      }

      public void AddError(string errorMessage) {
         Contract.Requires<ArgumentNullException>(errorMessage != null);
         Errors.Add(errorMessage);
      }
   }
}
