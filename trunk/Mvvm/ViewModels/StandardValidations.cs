﻿namespace Inspiring.Mvvm.ViewModels {
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

      public static void IsUnique<TItemVM, TItemValue>(
         this ICollectionValidationBuilder<TItemVM, TItemValue> builder,
         string errorMessage
      ) where TItemVM : ViewModel {
         builder.Custom(args => {
            if (args.AllItems.Any(i => i.VM != args.Item.VM && Object.Equals(i.Value, args.Item.Value))) {
               args.AddError(errorMessage);
            }

            args.AffectsOtherItems = true;
         });
      }

      public static void PropagateChildErrors<TItemVM>(
         this ICollectionValidationBuilder<TItemVM> builder,
         string errorMessage
      ) where TItemVM : ViewModel {
         builder.Custom((item, items, args) => {
            // HACK: Count > 0 is a hack, when and with what values should this validation be run?
            if (items.Any(x => !x.IsValid(true)) || args.Errors.Count > 0) {
               args.AddError(errorMessage);
            }
         });
      }
   }
}
