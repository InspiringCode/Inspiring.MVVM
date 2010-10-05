namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Core;

   public static class StandardValidations {
      public static void HasValue<TVM, TValue>(
         this IValidationBuilder<TVM, TValue> builder,
         string errorMessage
      ) where TVM : ViewModel {
         builder.Custom((TVM vm, TValue value) => {
            bool empty =
               (value is string && String.IsNullOrWhiteSpace(value as string)) ||
               Object.Equals(value, null);

            return empty ?
               ValidationResult.Failure(errorMessage) :
               ValidationResult.Success();
         });
      }

      public static void Length<TVM>(
         this IValidationBuilder<TVM, string> builder,
         int maximumLength,
         string errorMessage
      ) where TVM : ViewModel {
         builder.Custom((TVM vm, string value) => {
            return value != null && value.Length > maximumLength ?
               ValidationResult.Failure(errorMessage.FormatWith(maximumLength)) :
               ValidationResult.Success();
         });
      }

      public static void IsUnique<TParentVM, TVM, TValue>(
         this IValidationBuilder<TParentVM, TVM, TValue> builder,
         Func<TParentVM, IEnumerable<TVM>> allItemSelector,
         Func<TVM, TValue> valueSelector,
         string errorMessage
      )
         where TParentVM : ViewModel
         where TVM : ViewModel {

         builder.Custom((TParentVM parent, TVM vm, TValue value) => {
            IEnumerable<TVM> allItems = allItemSelector(parent);

            return allItems.Any(i => !Object.ReferenceEquals(i, vm) && Object.Equals(valueSelector(i), value)) ?
               ValidationResult.Failure(errorMessage) :
               ValidationResult.Success();
         });
      }
   }
}
