﻿namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Text.RegularExpressions;
   using Inspiring.Mvvm.ViewModels.Core;

   public static class StandardValidations {
      public static void HasValue<TVM, TValue>(
         this PropertyValidatorBuilder<TVM, TValue> builder,
         string errorMessage
      ) where TVM : IViewModel {
         builder.Custom((TVM vm, TValue value, ValidationArgs args) => {
            bool empty =
               (value is string && String.IsNullOrWhiteSpace(value as string)) ||
               Object.Equals(value, null);

            if (empty) {
               args.Errors.Add(new ValidationError(errorMessage));
            }
         });
      }

      public static void Length<TVM>(
         this PropertyValidatorBuilder<TVM, string> builder,
         int maximumLength,
         string errorMessage
      ) where TVM : IViewModel {
         builder.Custom((TVM vm, string value, ValidationArgs args) => {
            if (value != null && value.Length > maximumLength) {
               args.Errors.Add(new ValidationError(errorMessage.FormatWith(maximumLength)));
            }
         });
      }

      //public static void IsUnique<TParentVM, TVM, TValue>(
      //   this IValidationBuilder<TParentVM, TVM, TValue> builder,
      //   Func<TParentVM, IEnumerable<TVM>> allItemSelector,
      //   Func<TVM, TValue> valueSelector,
      //   string errorMessage
      //)
      //   where TParentVM : IViewModel
      //   where TVM : IViewModel {

      //   builder.Custom((TParentVM parent, TVM vm, TValue value) => {
      //      IEnumerable<TVM> allItems = allItemSelector(parent);

      //      return allItems.Any(i => !Object.ReferenceEquals(i, vm) && Object.Equals(valueSelector(i), value)) ?
      //         ValidationResult.Failure(errorMessage) :
      //         ValidationResult.Success();
      //   });
      //}

      //public static void IsUnique<TItemVM, TItemValue>(
      //   this ICollectionValidationBuilder<TItemVM, TItemValue> builder,
      //   string errorMessage
      //) where TItemVM : IViewModel {
      //   builder.Custom(args => {
      //      if (args.AllItems.Any(i => !Object.Equals(i.VM, args.Item.VM) && Object.Equals(i.Value, args.Item.Value))) {
      //         args.AddError(errorMessage);
      //      }

      //      args.AffectsOtherItems = true;
      //   });
      //}

      //public static void IsUnique<TItemVM>(
      //   this ICollectionValidationBuilder<TItemVM, string> builder,
      //   StringComparison comparisonType,
      //   string errorMessage
      //) where TItemVM : IViewModel {
      //   builder.Custom(args => {
      //      if (args.AllItems.Any(i => !Object.Equals(i.VM, args.Item.VM) && String.Equals(i.Value, args.Item.Value, comparisonType))) {
      //         args.AddError(errorMessage);
      //      }

      //      args.AffectsOtherItems = true;
      //   });
      //}

      public static void IsUnique<TItemDescriptor>(
         this CollectionPropertyValidatorBuilder<TItemDescriptor, string> builder,
         StringComparison comparisonType,
         string errorMessage
      ) where TItemDescriptor : VMDescriptorBase {
         builder.Custom<ViewModel<TItemDescriptor>>((item, items, property, args) => {
            bool isUnique = true;
            string itemPropertyValue = item.GetValue(property);

            foreach (ViewModel<TItemDescriptor> i in items) {
               if (!Object.ReferenceEquals(i, item)) {
                  if (String.Equals(i.GetValue(property), itemPropertyValue, comparisonType)) {
                     isUnique = false;
                     args.RevalidationQueue.Add(i);
                  }

                  if (!((IViewModel)i).Kernel.GetValidationState(ValidationStateScope.Self).IsValid) {
                     args.RevalidationQueue.Add(i);
                  }
               }
            }

            if (!isUnique) {
               args.Errors.Add(new ValidationError(errorMessage));
            }
         });

         //builder.Custom((value, values, args) => {
         //   if (values.Count(val => String.Equals(val, value, comparisonType)) > 1) {
         //      args.Errors.Add(new ValidationError(errorMessage));
         //   }

         //   // TODO: Affects other items!!!
         //});
      }

      public static void PropagateChildErrors<TVM, TDescriptor>(
         this ValidatorBuilderBase<TVM, TDescriptor> builder,
         string errorMessage
      )
         where TVM : IViewModel
         where TDescriptor : VMDescriptorBase {
         builder.CheckViewModel((vm, args) => {
            if (!vm.Kernel.GetValidationState(ValidationStateScope.Descendants).IsValid) {
               args.Errors.Add(new ValidationError(errorMessage));
            }
         });
      }

      // TODO: Test me.
      public static void ValidateProperties<TVM, TDescriptor>(
         this ValidatorBuilderBase<TVM, TDescriptor> builder,
         string errorMessage
      )
         where TVM : IViewModel
         where TDescriptor : VMDescriptorBase {

         builder.CheckViewModel((vm, args) => {
            if (!vm.Kernel.GetValidationState(ValidationStateScope.PropertiesOnly).IsValid) {
               args.Errors.Add(new ValidationError(errorMessage));
            }
         });
      }

      // TODO: Test me.
      public static void RegexValidation<TVM>(
         this PropertyValidatorBuilder<TVM, string> builder,
         string regexPattern,
         string errorMessage
      ) where TVM : IViewModel {
         Regex regex = new Regex(regexPattern);
         builder.Custom((vm, value, args) => {
            if (!(String.IsNullOrEmpty(value) || regex.IsMatch(value))) {
               args.Errors.Add(new ValidationError(errorMessage));
            }
         });
      }
   }
}
