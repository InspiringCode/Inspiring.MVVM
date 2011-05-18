namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Text.RegularExpressions;
   using Inspiring.Mvvm.ViewModels.Core;

   public static class StandardValidations {
      public static void HasValue<TOwnerVM, TTargetVM, TValue>(
         this PropertyValidatorBuilder<TOwnerVM, TTargetVM, TValue> builder,
         string errorMessage
      )
         where TOwnerVM : IViewModel
         where TTargetVM : IViewModel {

         builder.Custom((args) => {
            bool empty =
               (args.Value is string && String.IsNullOrWhiteSpace(args.Value as string)) ||
               Object.Equals(args.Value, null);

            if (empty) {
               args.AddError(errorMessage);
            }
         });
      }

      public static void Length<TOwnerVM, TTargetVM>(
         this PropertyValidatorBuilder<TOwnerVM, TTargetVM, string> builder,
         int maximumLength,
         string errorMessage
      )
         where TOwnerVM : IViewModel
         where TTargetVM : IViewModel {

         builder.Custom((args) => {
            if (args.Value != null && args.Value.Length > maximumLength) {
               args.AddError(errorMessage.FormatWith(maximumLength));
            }
         });
      }

      public static void IsUnique<TOwnerVM, TItemDescriptor, T>(
         this CollectionValidatorBuilder<TOwnerVM, TItemDescriptor, T> builder, // TODO
         string errorMessage
      )
         where TOwnerVM : IViewModel
         where TItemDescriptor : IVMDescriptor {

         builder.Custom<IViewModel<TItemDescriptor>>((args) => {
            args
               .Items
               .GroupBy(x => x.Kernel.GetValue(args.TargetProperty))
               .Where(g => g.Count() > 1)
               .SelectMany(g => g)
               .ForEach(item => {
                  args.AddError(item, errorMessage);
               });
         });
      }

      public static void IsUnique<TOwnerVM, TItemDescriptor>(
         this CollectionValidatorBuilder<TOwnerVM, TItemDescriptor, string> builder,
         StringComparison comparisonType,
         string errorMessage
      )
         where TOwnerVM : IViewModel
         where TItemDescriptor : IVMDescriptor {

         builder.Custom<IViewModel<TItemDescriptor>>((args) => {
            args
               .Items
               .GroupBy(x => x.Kernel.GetValue(args.TargetProperty), GetStringComparer(comparisonType))
               .Where(g => g.Count() > 1)
               .SelectMany(g => g)
               .ForEach(item => {
                  args.AddError(item, errorMessage);
               });
         });
      }

      public static void IsUnique<TOwnerVM, TItemVM, TKey>(
         this CollectionValidatorBuilder<TOwnerVM, TItemVM> builder,
         Func<TItemVM, TKey> keySelector,
         string errorMessage
      )
         where TOwnerVM : IViewModel
         where TItemVM : IViewModel {

         builder.Custom(args => {
            args
               .Items
               .GroupBy(keySelector)
               .Where(g => g.Count() > 1)
               .SelectMany(g => g)
               .ForEach(item => {
                  args.AddError(item, errorMessage);
               });
         });
      }

      public static void IsUnique<TOwnerVM, TItemVM>(
         this CollectionValidatorBuilder<TOwnerVM, TItemVM> builder,
         IEqualityComparer<TItemVM> comparer,
         string errorMessage
      )
         where TOwnerVM : IViewModel
         where TItemVM : IViewModel {

         builder.Custom(args => {
            args
               .Items
               .GroupBy(x => x, comparer)
               .Where(g => g.Count() > 1)
               .SelectMany(g => g)
               .ForEach(item => {
                  args.AddError(item, errorMessage);
               });
         });
      }

      public static void IsUnique<TOwnerVM, TItemVM>(
         this CollectionValidatorBuilder<TOwnerVM, TItemVM> builder,
         string errorMessage
      )
         where TOwnerVM : IViewModel
         where TItemVM : IViewModel {

         IsUnique(
            builder,
            x => x,
            errorMessage
         );
      }

      private static IEqualityComparer<string> GetStringComparer(StringComparison comparisonType) {
         switch (comparisonType) {
            case StringComparison.CurrentCulture:
               return StringComparer.CurrentCulture;
            case StringComparison.CurrentCultureIgnoreCase:
               return StringComparer.CurrentCultureIgnoreCase;
            case StringComparison.InvariantCulture:
               return StringComparer.InvariantCulture;
            case StringComparison.InvariantCultureIgnoreCase:
               return StringComparer.InvariantCultureIgnoreCase;
            case StringComparison.Ordinal:
               return StringComparer.Ordinal;
            case StringComparison.OrdinalIgnoreCase:
               return StringComparer.OrdinalIgnoreCase;
            default:
               throw new NotSupportedException();
         }
      }

      /// <summary>
      ///   Adds an expli1cit view model-level validation error (with the given 
      ///   <paramref name="errorMessage"/>) to the current VM if any of its 
      ///   descendants are invalid (see remarks).
      /// </summary>
      /// <remarks>
      ///   <para>Beware that the validation state is always propagated to to the
      ///      parent (meaning that <see cref="ViewModel.IsValid"/> returns false
      ///      if any descendant is invalid) but <see cref="ViewModel.GetValidationState"/>
      ///      with <see cref="ValidationResultScope.Self"/> does not return an error
      ///      by default unless you add this validation rule.</para>
      ///   <para>This validation rule is most useful in hierarchical data structures
      ///      (e.g. an employee has projects which have tasks which have records) to
      ///      guide the user to the (probably not visible) record that is actually
      ///      invalid.</para>
      /// </remarks>
      public static void PropagateChildErrors<TOwnerVM, TTargetVM, TDescriptor>(
         this ValidatorBuilder<TOwnerVM, TTargetVM, TDescriptor> builder,
         string errorMessage
      )
         where TOwnerVM : IViewModel
         where TTargetVM : IViewModel
         where TDescriptor : class, IVMDescriptor {

         builder.CheckViewModel((args) => {
            if (!args.Owner.Kernel.GetValidationResult(ValidationResultScope.Descendants).IsValid) {
               args.AddError(errorMessage);
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
      public static void ValidateProperties<TOwnerVM, TTargetVM, TDescriptor>(
         this ValidatorBuilder<TOwnerVM, TTargetVM, TDescriptor> builder,
         string errorMessage
      )
         where TOwnerVM : IViewModel
         where TTargetVM : IViewModel
         where TDescriptor : class, IVMDescriptor {

         builder.CheckViewModel((args) => {
            if (!args.Owner.Kernel.GetValidationResult(ValidationResultScope.PropertiesOnly).IsValid) {
               args.AddError(errorMessage);
            }
         });
      }

      public static void RegexValidation<TOwnerVM, TTargetVM>(
         this PropertyValidatorBuilder<TOwnerVM, TTargetVM, string> builder,
         string regexPattern,
         string errorMessage
      )
         where TOwnerVM : IViewModel
         where TTargetVM : IViewModel {

         Regex regex = new Regex(regexPattern);
         builder.Custom((args) => {
            if (!(String.IsNullOrEmpty(args.Value) || regex.IsMatch(args.Value))) {
               args.AddError(errorMessage);
            }
         });
      }
   }
}