namespace Inspiring.Mvvm.ViewModels.MultiSelection {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Fluent;

   public static class MultiSelectionBuilder {
      public static MultiSelectionBuilder<TSourceObject, TItemSource> MultiSelection<TSourceObject, TItemSource>(
         this IVMPropertyFactory<TSourceObject> sourceObjectPropertyFactory,
         Func<TSourceObject, ICollection<TItemSource>> selectedSourceItemsSelector
      ) {
         Contract.Requires<ArgumentNullException>(selectedSourceItemsSelector != null);
         Contract.Requires(sourceObjectPropertyFactory != null);

         var builder = new MultiSelectionBuilder<TSourceObject, TItemSource>(sourceObjectPropertyFactory);

         builder.SelectedSourceItemsPropertyFactory = factory =>
            factory.Property.DelegatesTo(selectedSourceItemsSelector);

         return builder;
      }
   }

   public class MultiSelectionBuilder<TSourceObject, TItemSource> {
      private IVMPropertyFactory<TSourceObject> _sourceObjectPropertyFactory;

      internal MultiSelectionBuilder(IVMPropertyFactory<TSourceObject> sourceObjectPropertyFactory) {
         Contract.Requires(sourceObjectPropertyFactory != null);
         _sourceObjectPropertyFactory = sourceObjectPropertyFactory;
      }

      internal Func<IVMPropertyBuilder<TSourceObject>, VMProperty<ICollection<TItemSource>>> SelectedSourceItemsPropertyFactory {
         get;
         set;
      }

      internal Func<IVMPropertyBuilder<TSourceObject>, VMProperty<IEnumerable<TItemSource>>> AllSourceItemsPropertyFactory {
         get;
         set;
      }

      public void Of<TItemVM>(
         VMDescriptorBase itemDescriptor
      ) where TItemVM : IViewModel, IVMCollectionItem<TItemSource> {
         _sourceObjectPropertyFactory.Local.VM<MultiSelectionVM<TSourceObject, TItemSource, TItemVM>>();
      }
   }
}
