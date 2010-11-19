﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   internal sealed class CollectionValidationBehavior<TItemVM> : Behavior where TItemVM : IViewModel {
      private List<CollectionValidator<TItemVM>> _validators
         = new List<CollectionValidator<TItemVM>>();

      public void Add(CollectionValidator<TItemVM> validator) {
         _validators.Add(validator);
      }

      public void Validate(IEnumerable<TItemVM> allItems, ValidationEventArgs args) {
         _validators.ForEach(x => x((TItemVM)(object)args.VM, allItems, args));
      }
   }
}
