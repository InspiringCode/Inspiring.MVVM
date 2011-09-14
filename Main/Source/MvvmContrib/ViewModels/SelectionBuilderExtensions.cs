namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Resources;
   using Inspiring.Mvvm.ViewModels.Core;

   public static class SelectionBuilderExtensions {
      public static MultiSelectionBuilder<TSourceObject, TItemSource> MultiSelection<TSourceObject, TItemSource>(
         this IVMPropertyBuilder<TSourceObject> sourceObjectPropertyFactory,
         Func<TSourceObject, ICollection<TItemSource>> selectedSourceItemsSelector
      ) {
         Contract.Requires<ArgumentNullException>(selectedSourceItemsSelector != null);
         Contract.Requires(sourceObjectPropertyFactory != null);

         var builder = new MultiSelectionBuilder<TSourceObject, TItemSource>(sourceObjectPropertyFactory);

         // Store the property factory for later creation of the MultiSelectionVM.
         builder.SelectedSourceItemsPropertyFactory = factory =>
            factory.Property.DelegatesTo(selectedSourceItemsSelector);

         return builder;
      }

      public static SingleSelectionBuilder<TSourceObject, TItemSource> SingleSelection<TSourceObject, TItemSource>(
         this IVMPropertyBuilder<TSourceObject> sourceObjectPropertyFactory,
         Expression<Func<TSourceObject, TItemSource>> selectedSourceItemSelector
      ) {
         Contract.Requires<ArgumentNullException>(selectedSourceItemSelector != null);
         Contract.Requires(sourceObjectPropertyFactory != null);

         var builder = new SingleSelectionBuilder<TSourceObject, TItemSource>(sourceObjectPropertyFactory);

         // Store the property factory for later creation of the MultiSelectionVM.
         builder.SelectedSourceItemPropertyFactory = factory =>
            factory.Property.MapsTo(selectedSourceItemSelector);

         return builder;
      }

      public static IVMPropertyDescriptor<SingleSelectionVM<TEnum>> EnumSelection<TSourceObject, TEnum>(
         this IVMPropertyBuilder<TSourceObject> sourceObjectPropertyFactory,
         Expression<Func<TSourceObject, TEnum>> selectedSourceItemSelector
      ) {
         Contract.Requires<ArgumentNullException>(selectedSourceItemSelector != null);
         Contract.Requires(sourceObjectPropertyFactory != null);

         return sourceObjectPropertyFactory
            .SingleSelection(selectedSourceItemSelector)
            .WithItems(x => GetEnumValues<TEnum>())
            .WithCaption(x => EnumLocalizer.GetCaption(x));
      }

      public static IVMPropertyDescriptor<SingleSelectionVM<TEnum>> UndoableEnumSelection<TSourceObject, TEnum>(
         this IVMPropertyBuilder<TSourceObject> sourceObjectPropertyFactory,
         Expression<Func<TSourceObject, TEnum>> selectedSourceItemSelector
      ) {
         Contract.Requires<ArgumentNullException>(selectedSourceItemSelector != null);
         Contract.Requires(sourceObjectPropertyFactory != null);

         return sourceObjectPropertyFactory
            .SingleSelection(selectedSourceItemSelector)
            .EnableUndo()
            .WithItems(x => GetEnumValues<TEnum>())
            .WithCaption(x => EnumLocalizer.GetCaption(x));
      }

      public static void OnlyExistingItemsAreSelected<TOwner, TTarget, TDescriptor, TSource, TVM>(
         this ValidatorBuilder<
            TOwner,
            TTarget,
            TDescriptor
         > builder,
         Func<TDescriptor, IVMPropertyDescriptor<IViewModel<SingleSelectionVMDescriptor<TSource, TVM>>>> selectionPropertySelector,
         string errorMessage = null,
         object details = null
      )
         where TTarget : IViewModel
         where TDescriptor : class, IVMDescriptor
         where TOwner : IViewModel
         where TVM : IViewModel, IHasSourceObject<TSource> {

         errorMessage = errorMessage ?? Localized.SelectedItemsNotInSourceItems;

         builder
            .ValidateDescendant(selectionPropertySelector)
            .OnlyExistingItemsAreSelected(errorMessage, details);
      }

      public static void OnlyExistingItemsAreSelected<TOwner, TTarget, TDescriptor, TSource, TVM>(
         this ValidatorBuilder<
            TOwner,
            TTarget,
            TDescriptor
         > builder,
         Func<TDescriptor, IVMPropertyDescriptor<IViewModel<MultiSelectionVMDescriptor<TSource, TVM>>>> selectionPropertySelector,
         string errorMessage = null,
         object details = null
      )
         where TTarget : IViewModel
         where TDescriptor : class, IVMDescriptor
         where TOwner : IViewModel
         where TVM : IViewModel, IHasSourceObject<TSource> {

         errorMessage = errorMessage ?? Localized.SelectedItemsNotInSourceItems;

         builder
            .ValidateDescendant(selectionPropertySelector)
            .OnlyExistingItemsAreSelected(errorMessage, details);
      }

      public static void OnlyExistingItemsAreSelected<TOwner, TTarget, TSource, TVM>(
         this ValidatorBuilder<
            TOwner,
            TTarget,
            MultiSelectionVMDescriptor<TSource, TVM>
         > builder,
         string errorMessage = null,
         object details = null
      )
         where TTarget : IViewModel<MultiSelectionVMDescriptor<TSource, TVM>>
         where TOwner : IViewModel
         where TVM : IViewModel, IHasSourceObject<TSource> {

         errorMessage = errorMessage ?? Localized.SelectedItemsNotInSourceItems;

         builder.CheckCollection(x => x.SelectedItems).Custom(args => {
            var selectionVM = (IHasSourceItems<TSource>)args.Items.OwnerVM;
            SourceItemCollections<TSource> source = selectionVM.SourceItems;

            //ISet<TSource> nonExisting = source.SelectedItemsNotContainedInSource;

            foreach (TVM selectedItem in args.Items) {
               if (!source.IsItemContainedInSource(selectedItem.Source)) {
                  args.AddError(selectedItem, errorMessage, details);
               }
            }
         });
      }

      public static void OnlyExistingItemsAreSelected<TOwner, TTarget, TSource, TVM>(
         this ValidatorBuilder<
            TOwner,
            TTarget,
            SingleSelectionVMDescriptor<TSource, TVM>
         > builder,
         string errorMessage = null,
         object details = null
      )
         where TTarget : IViewModel<SingleSelectionVMDescriptor<TSource, TVM>>
         where TOwner : IViewModel
         where TVM : IViewModel, IHasSourceObject<TSource> {

         errorMessage = errorMessage ?? Localized.SelectedItemsNotInSourceItems;

         builder.Check(x => x.SelectedItem).Custom(args => {
            var selectionVM = (IHasSourceItems<TSource>)args.Target;
            SourceItemCollections<TSource> source = selectionVM.SourceItems;

            //ISet<TSource> nonExisting = source.SelectedItemsNotContainedInSource;

            if (args.Value != null) {
               if (!source.IsItemContainedInSource(args.Value.Source)) {
                  args.AddError(errorMessage, details);
               }
            }
         });
      }

      private static TEnum[] GetEnumValues<TEnum>() {
         Type enumType;

         if (IsNullableType(typeof(TEnum))) {
            enumType = Nullable.GetUnderlyingType(typeof(TEnum));
         } else {
            enumType = typeof(TEnum);
         }
         return Enum.GetValues(enumType).Cast<TEnum>().ToArray();
      }

      private static bool IsNullableType(Type t) {
         return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>) ? true : false;
      }
   }
}
