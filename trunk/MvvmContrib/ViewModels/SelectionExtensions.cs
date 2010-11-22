﻿namespace Inspiring.Mvvm.ViewModels {

   public static class SelectionExtensions {
      public static ISingleSelectionPropertyBuilder<TParentVM> SingleSelection<TParentVM>(
         this IRootVMPropertyFactory<TParentVM> propertyFactory
      ) where TParentVM : IViewModel {
         return new SingleSelectionPropertyBuilder<TParentVM>(propertyFactory);
      }

      public static IEnumSelectionPropertyBuilder<TParentVM> EnumSelection<TParentVM>(
         this IRootVMPropertyFactory<TParentVM> propertyFactory
      ) where TParentVM : IViewModel {
         return new EnumSelectionPropertyBuilder<TParentVM>(propertyFactory);
      }
   }
}
