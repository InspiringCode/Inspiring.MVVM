using System;
namespace Inspiring.Mvvm.ViewModels {

   public static class SelectionExtensions {
      public static ISingleSelectionPropertyBuilder<TVM> SingleSelection<TVM>(
         this IRootVMPropertyFactory<TVM> propertyFactory
      ) where TVM : ViewModel {
         throw new NotImplementedException();
      }
   }
}
