namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public sealed class ValidationEventArgs : EventArgs {
      private bool _affectsOtherItems = false;

      public ValidationEventArgs(
         VMProperty property,
         object propertyValue,
         ViewModel viewModel
      ) {
         Property = property;
         PropertyValue = propertyValue;
         VM = viewModel;
         Errors = new List<string>();
      }

      public VMProperty Property { get; private set; }

      public object PropertyValue { get; private set; }

      public ViewModel VM { get; private set; }

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
