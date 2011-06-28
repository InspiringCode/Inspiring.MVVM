namespace Inspiring.Mvvm.Views.Binder.Xpf {
   using System;
   using System.Windows;

   public static class XpfBinder {
      public static BindingPropertiesBuilder<TSource> To<TSource>(
         this BindingTargetBuilder<TSource> binder,
         DependencyObject target
      ) {
         throw new NotImplementedException();
      }

      public static void BindItem<TItemSource>(
         this BindingPropertiesBuilder<CollectionSource<TItemSource>> binder,
         Action<ObjectBinder<TItemSource>> itemBinderAction
      ) {
         throw new NotImplementedException();
      }
   }
}
