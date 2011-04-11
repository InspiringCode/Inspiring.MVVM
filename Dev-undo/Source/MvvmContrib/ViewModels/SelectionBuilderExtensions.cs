namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using System.Linq.Expressions;
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
