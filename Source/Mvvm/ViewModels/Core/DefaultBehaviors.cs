namespace Inspiring.Mvvm.ViewModels.Core {

   public static class DefaultBehaviors {
      public static IValueAccessorBehavior<IVMCollection<TItemVM>> WrapperCollectionValueAccessor<TItemSource, TItemVM>()
         where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

         return new WrapperCollectionAccessorBehavior<TItemVM, TItemSource>(true);
      }
   }
}
