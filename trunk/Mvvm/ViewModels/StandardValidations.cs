namespace Inspiring.Mvvm.ViewModels {
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

      // TODO: Test me.
      public static void IsUnique<TItemDescriptor, T>(
         this CollectionPropertyValidatorBuilder<TItemDescriptor, T> builder,
         string errorMessage
      ) where TItemDescriptor : VMDescriptorBase {
         // We only require 'IViewModel' because 'IViewModel<TItemDescriptor>' does not
         // succeed in all inheritance cases.
         // TODO: Is there a better solution?
         builder.Custom<IViewModel>((item, items, property, args) => {
            bool isUnique = true;
            T itemPropertyValue = item.Kernel.GetValue(property);

            foreach (IViewModel i in items) {
               if (!Object.ReferenceEquals(i, item)) {
                  if (Object.Equals(i.Kernel.GetValue(property), itemPropertyValue)) {
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

      public static void IsUnique<TItemDescriptor>(
         this CollectionPropertyValidatorBuilder<TItemDescriptor, string> builder,
         StringComparison comparisonType,
         string errorMessage
      ) where TItemDescriptor : VMDescriptorBase {
         builder.Custom<IViewModel<TItemDescriptor>>((item, items, property, args) => {
            bool isUnique = true;
            string itemPropertyValue = item.Kernel.GetValue(property);

            foreach (IViewModel<TItemDescriptor> i in items) {
               if (!Object.ReferenceEquals(i, item)) {
                  if (String.Equals(i.Kernel.GetValue(property), itemPropertyValue, comparisonType)) {
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

      /// <summary>
      ///   Adds an explicit view model-level validation error (with the given 
      ///   <paramref name="errorMessage"/>) to the current VM if any of its 
      ///   descendants are invalid (see remarks).
      /// </summary>
      /// <remarks>
      ///   <para>Beware that the validation state is always propagated to to the
      ///      parent (meaning that <see cref="ViewModel.IsValid"/> returns false
      ///      if any descendant is invalid) but <see cref="ViewModel.GetValidationState"/>
      ///      with <see cref="ValidationStateScope.Self"/> does not return an error
      ///      by default unless you add this validation rule.</para>
      ///   <para>This validation rule is most useful in hierarchical data structures
      ///      (e.g. an employee has projects which have tasks which have records) to
      ///      guide the user to the (probably not visible) record that is actually
      ///      invalid.</para>
      /// </remarks>
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

      /// <summary>
      ///   Adds an explicit view model-level validation error (with the given 
      ///   <paramref name="errorMessage"/> to the current VM if any of its 
      ///   properties are invalid (see remarks).
      /// </summary>
      /// <remarks>
      ///   This rule is useful if a VM is displayed in a grid where not all
      ///   properties are visible in the grid. If any of the properites of a
      ///   VM (visible or not) become invalid, a view model-level (row level)
      ///   validation error is added to the VM (which is for example visualized
      ///   by a red cross to the left of the grid row). This indicates to the
      ///   user that something currently not visible may be invalid and that he
      ///   or she should open the details of the current record to correct the
      ///   validation error.
      /// </remarks>
      public static void ValidateProperties<TVM, TDescriptor>(
         this ValidatorBuilderBase<TVM, TDescriptor> builder,
         string errorMessage
      )
         where TVM : IViewModel
         where TDescriptor : VMDescriptorBase {

         // TODO: Test me.

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
