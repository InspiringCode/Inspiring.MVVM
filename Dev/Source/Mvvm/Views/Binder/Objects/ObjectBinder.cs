namespace Inspiring.Mvvm.Views {
   using System;
   using System.Collections.Generic;
   using System.Linq.Expressions;

   public static class ObjectBinder {
      public static void BindType<TType>(Action<ObjectBinder<ObjectSource<TType>>> binderAction) {
         throw new NotImplementedException();
      }

      public static BindingTargetBuilder<ObjectSource<TProperty>> Property<TOwner, TProperty>(
         this ObjectBinder<ObjectSource<TOwner>> binder,
         Expression<Func<TOwner, TProperty>> propertySelector
      ) {
         throw new NotImplementedException();
      }

      public static void Object<TOwner, TObject>(
         this ObjectBinder<ObjectSource<TOwner>> binder,
         Expression<Func<TOwner, TObject>> objectSelector,
         Action<ObjectBinder<ObjectSource<TObject>>> binderAction
      ) {
         throw new NotImplementedException();
      }

      public static BindingTargetBuilder<CollectionSource<ObjectSource<TItem>>> Collection<TOwner, TItem>(
         this ObjectBinder<ObjectSource<TOwner>> binder,
         Expression<Func<TOwner, IEnumerable<TItem>>> collectionSelector
      ) {
         throw new NotImplementedException();
      }
   }
}
