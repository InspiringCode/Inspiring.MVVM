namespace Inspiring.Mvvm.Views {
   using System;
   using System.Collections.Generic;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.ViewModels;

   public static class ViewModelBinder {
      public static void BindDescriptor<TDescriptor>(
         Action<ObjectBinder<ViewModelSource<TDescriptor>>> binderAction
      ) where TDescriptor : IVMDescriptor {
         throw new NotImplementedException();
      }

      public static BindingTargetBuilder<ObjectSource<TProperty>> Property<TDescriptor, TProperty>(
         this ObjectBinder<ViewModelSource<TDescriptor>> binder,
         Expression<Func<TDescriptor, IVMPropertyDescriptor<TProperty>>> propertySelector
      ) where TDescriptor : IVMDescriptor {
         throw new NotImplementedException();
      }

      public static void Object<TOwnerDesc, TObjectDesc>(
         this ObjectBinder<ViewModelSource<TOwnerDesc>> binder,
         Expression<Func<TOwnerDesc, IVMPropertyDescriptor<IViewModel<TObjectDesc>>>> objectSelector,
         Action<ObjectBinder<ViewModelSource<TObjectDesc>>> binderAction
      )
         where TOwnerDesc : IVMDescriptor
         where TObjectDesc : IVMDescriptor {

         throw new NotImplementedException();
      }

      public static BindingTargetBuilder<CollectionSource<ViewModelSource<TItemDesc>>> Collection<TOwnerDesc, TItemDesc>(
         this ObjectBinder<ViewModelSource<TOwnerDesc>> binder,
         Expression<Func<TOwnerDesc, IVMPropertyDescriptor<IEnumerable<IViewModel<TItemDesc>>>>> collectionSelector
      )
         where TOwnerDesc : IVMDescriptor
         where TItemDesc : IVMDescriptor {

         throw new NotImplementedException();
      }
   }
}
