namespace Inspiring.Mvvm.ViewModels {

   public static class SelectionExtensions {
      public static ISingleSelectionPropertyBuilder<TParentVM> SingleSelection<TParentVM>(
         this IRootVMPropertyFactory<TParentVM> propertyFactory
      ) where TParentVM : ViewModel {
         return new SingleSelectionPropertyBuilder<TParentVM>(propertyFactory);
      }
   }
}
