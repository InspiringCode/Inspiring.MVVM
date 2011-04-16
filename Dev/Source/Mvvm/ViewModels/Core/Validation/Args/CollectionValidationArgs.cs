namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;

   public class CollectionValidationArgs<TOwnerVM, TItemVM> :
      ValidationArgs<TOwnerVM>
      where TOwnerVM : IViewModel
      where TItemVM : IViewModel {

      public CollectionValidationArgs(
         IValidator validator,
         TOwnerVM owner,
         IVMCollection<TItemVM> items
       )
         : base(validator, owner) {

         Contract.Requires(items != null);
         Items = items;
      }

      public IVMCollection<TItemVM> Items { get; private set; }

      public void AddError(TItemVM item, string message, object details = null) {
         Contract.Requires<ArgumentNullException>(item != null);
         Contract.Requires<ArgumentNullException>(message != null);

         var error = new ValidationError(Validator, item, message, details);
         AddError(error);
      }

      internal static CollectionValidationArgs<TOwnerVM, TItemVM> Create(
         IValidator validator,
         ValidationRequest request
      ) {
         var path = request.TargetPath;

         var owner = (TOwnerVM)path[0].ViewModel;
         var collection = (IVMCollection<TItemVM>)path[path.Length - 1].Collection;

         return new CollectionValidationArgs<TOwnerVM, TItemVM>(
            validator,
            owner,
            collection
         );
      }
   }

   public class CollectionValidationArgs<TOwnerVM, TItemVM, TValue> :
      ValidationArgs<TOwnerVM>
      where TOwnerVM : IViewModel
      where TItemVM : IViewModel {

      public CollectionValidationArgs(
         IValidator validator,
         TOwnerVM owner,
         IVMCollection<TItemVM> items,
         IVMPropertyDescriptor<TValue> property
      )
         : base(validator, owner) {

         Contract.Requires(items != null);
         Contract.Requires(property != null);

         Items = items;
         TargetProperty = property;
      }

      public IVMCollection<TItemVM> Items { get; private set; }

      public IVMPropertyDescriptor<TValue> TargetProperty { get; private set; }

      public void AddError(TItemVM item, string message, object details = null) {
         Contract.Requires<ArgumentNullException>(item != null);
         Contract.Requires<ArgumentNullException>(message != null);

         var error = new ValidationError(Validator, item, TargetProperty, message, details);
         AddError(error);
      }

      internal static CollectionValidationArgs<TOwnerVM, TItemVM, TValue> Create(
         IValidator validator,
         ValidationRequest request
      ) {
         var path = request.TargetPath;

         var owner = (TOwnerVM)path[0].ViewModel;
         var collection = (IVMCollection<TItemVM>)path[path.Length - 2].Collection;
         var property = (IVMPropertyDescriptor<TValue>)path[path.Length - 1].Property;

         return new CollectionValidationArgs<TOwnerVM, TItemVM, TValue>(
            validator,
            owner,
            collection,
            property
         );
      }
   }
}
