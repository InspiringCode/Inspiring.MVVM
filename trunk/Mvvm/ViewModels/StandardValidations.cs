namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Text.RegularExpressions;
   using Inspiring.Mvvm.ViewModels.Core;

   public static class StandardValidations {
      public static void HasValue<TVM, TValue>(
         this IValidationBuilder<TVM, TValue> builder,
         string errorMessage
      ) where TVM : IViewModel {
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
      ) where TVM : IViewModel {
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
         where TParenTVM : IViewModel
         where TVM : IViewModel {

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
      ) where TItemVM : IViewModel {
         builder.Custom(args => {
            if (args.AllItems.Any(i => i.VM != args.Item.VM && Object.Equals(i.Value, args.Item.Value))) {
               args.AddError(errorMessage);
            }

            args.AffectsOtherItems = true;
         });
      }

      public static void IsUnique<TItemVM>(
         this ICollectionValidationBuilder<TItemVM, string> builder,
         StringComparison comparisonType,
         string errorMessage
      ) where TItemVM : IViewModel {
         builder.Custom(args => {
            if (args.AllItems.Any(i => i.VM != args.Item.VM && String.Equals(i.Value, args.Item.Value, comparisonType))) {
               args.AddError(errorMessage);
            }

            args.AffectsOtherItems = true;
         });
      }

      public static void PropagateChildErrors<TVM>(
         this IValidationBuilder<TVM> builder,
         string errorMessage
      ) where TVM : IViewModel {
         builder.ViewModelValidator((vm, args) => {
            if (!vm.AreChildrenValid(validateGrandchildren: false)) {
               args.AddError(errorMessage);
            }
         });
      }

      // TODO: Test me.
      public static void ValidateProperties<TVM>(
         this IValidationBuilder<TVM> builder,
         string errorMessage
      ) where TVM : IViewModel {
         builder.ViewModelValidator((vm, args) => {
            if (!vm.ArePropertiesValid(validateChildren: false)) {
               args.AddError(errorMessage);
            }
         });
      }

      // TODO: Test me.
      public static void RegexValidation<TVM>(
         this IValidationBuilder<TVM, string> builder,
         string regexPattern,
         string errorMessage
      ) where TVM : IViewModel {
         Regex regex = new Regex(regexPattern);
         builder.Custom((vm, value) => {
            return String.IsNullOrEmpty(value) || regex.IsMatch(value) ?
               ValidationResult.Success() :
               ValidationResult.Failure(errorMessage);
         });
      }
   }
}
