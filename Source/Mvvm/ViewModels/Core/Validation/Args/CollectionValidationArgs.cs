namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   
   public class CollectionValidationArgs<TOwnerVM, TItemVM> :
      ValidationArgs<TOwnerVM>
      where TOwnerVM : IViewModel
      where TItemVM : IViewModel {

      public CollectionValidationArgs(
         IValidator validator,
         TOwnerVM owner,
         IVMCollection<TItemVM> items
       )
         : base(ValidationStep.ViewModel, validator, owner) {

         Check.NotNull(items, nameof(items));
         Items = items;
      }

      public IVMCollection<TItemVM> Items { get; private set; }

      public void AddError(TItemVM item, string message, object details = null) {
         Check.NotNull(item, nameof(item));
         Check.NotNull(message, nameof(message));

         var target = ValidationTarget.ForError(Step, item, Items, null);
         var error = new ValidationError(Validator, target, message, details);
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
         ValidationStep step,
         IValidator validator,
         TOwnerVM owner,
         IVMCollectionBase<TItemVM> items,
         IVMPropertyDescriptor<TValue> property
      )
         : base(step, validator, owner) {

         Check.NotNull(items, nameof(items));
         Check.NotNull(property, nameof(property));

         Items = items;
         TargetProperty = property;
      }

      public IVMCollectionBase<TItemVM> Items { get; private set; }

      public IVMPropertyDescriptor<TValue> TargetProperty { get; private set; }

      public void AddError(TItemVM item, string message, object details = null) {
         Check.NotNull(item, nameof(item));
         Check.NotNull(message, nameof(message));

         var target = ValidationTarget.ForError(Step, item, Items, TargetProperty);
         var error = new ValidationError(Validator, target, message, details);
         AddError(error);
      }

      internal static CollectionValidationArgs<TOwnerVM, TItemVM, TValue> Create(
         IValidator validator,
         ValidationRequest request
      ) {
         var path = request.TargetPath;

         var owner = (TOwnerVM)path[0].ViewModel;
         var collection = (IVMCollectionBase<TItemVM>)path[path.Length - 2].Collection;
         var property = (IVMPropertyDescriptor<TValue>)path[path.Length - 1].Property;

         return new CollectionValidationArgs<TOwnerVM, TItemVM, TValue>(
            request.Step,
            validator,
            owner,
            collection,
            property
         );
      }
   }
}
