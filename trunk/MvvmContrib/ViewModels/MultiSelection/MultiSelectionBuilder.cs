namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Fluent;

   public static class MultiSelectionBuilder {

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
   }

   public class MultiSelectionBuilder<TSourceObject, TItemSource> {
      private IVMPropertyBuilder<TSourceObject> _sourceObjectPropertyBuilder;

      /// <param name="sourceObjectPropertyBuilder">
      ///   The original <see cref="VMPropertyBuilder"/> that was extended by
      ///   with the extension method.
      /// </param>
      internal MultiSelectionBuilder(IVMPropertyBuilder<TSourceObject> sourceObjectPropertyBuilder) {
         Contract.Requires(sourceObjectPropertyBuilder != null);
         _sourceObjectPropertyBuilder = sourceObjectPropertyBuilder;
      }

      /// <summary>
      ///   Caches the property builder.
      /// </summary>
      internal Func<IVMPropertyBuilder<TSourceObject>, VMProperty<ICollection<TItemSource>>> SelectedSourceItemsPropertyFactory {
         get;
         set;
      }

      /// <summary>
      ///   Caches the property builder.
      /// </summary>
      internal Func<IVMPropertyBuilder<TSourceObject>, VMProperty<IEnumerable<TItemSource>>> AllSourceItemsPropertyFactory {
         get;
         set;
      }

      internal Func<TItemSource, bool> Filter {
         get;
         set;
      }

      public MultiSelectionBuilder<TSourceObject, TItemSource> WithFilter(Func<TItemSource, bool> filter) {
         Filter = filter;
         return this;
      }

      public MultiSelectionBuilder<TSourceObject, TItemSource> WithItems(
         Func<TSourceObject, IEnumerable<TItemSource>> allSourceItemsSelector
      ) {
         AllSourceItemsPropertyFactory = delegate(IVMPropertyBuilder<TSourceObject> factory) {
            return factory.Property.DelegatesTo(allSourceItemsSelector);
         };

         return this;
      }

      public VMProperty<MultiSelectionVM<TItemSource, TItemVM>> Of<TItemVM>(
         VMDescriptorBase itemDescriptor
      ) where TItemVM : IViewModel, IVMCollectionItem<TItemSource> {
         Contract.Assert(SelectedSourceItemsPropertyFactory != null);

         var allSourceItemsPropertyFactory =
            AllSourceItemsPropertyFactory ??
            CreateLocatingPropertyFactory();

         // The descriptor is created only once for every owner VM property/descriptor
         // and reused for every VM instance created from the owner VM descriptor.
         var descriptor = MultiSelectionVM<TSourceObject, TItemSource, TItemVM>.CreateDescriptor(
            itemDescriptor,
            SelectedSourceItemsPropertyFactory,
            allSourceItemsPropertyFactory
         );

         return _sourceObjectPropertyBuilder.VM.Custom(
            viewModelFactory: new MultSelectionFactory<TItemVM>(descriptor, Filter)
         );
      }

      /// <summary>
      ///   If no source for 'AllItems' was specified, all items are by default 
      ///   resolved by asking the <see cref="IServiceLocator"/> of the source
      ///   object for an <see cref="IEnumerable{T}"/>.
      /// </summary>
      private Func<IVMPropertyBuilder<TSourceObject>, VMProperty<IEnumerable<TItemSource>>> CreateLocatingPropertyFactory() {
         return delegate(IVMPropertyBuilder<TSourceObject> factory) {
            return factory.Property.Custom(
               sourceValueAccessor: new LocatingItemSourceBehavior()
            );
         };
      }

      private class MultSelectionFactory<TItemVM> :
         Behavior,
         IViewModelFactoryBehavior<MultiSelectionVM<TItemSource, TItemVM>>
         where TItemVM : IViewModel, IVMCollectionItem<TItemSource> {

         private MultiSelectionVMDescriptor<TItemSource, TItemVM> _descriptor;
         private Func<TItemSource, bool> _filter;

         public MultSelectionFactory(MultiSelectionVMDescriptor<TItemSource, TItemVM> descriptor, Func<TItemSource, bool> filter) {
            _descriptor = descriptor;
            _filter = filter;
         }

         public MultiSelectionVM<TItemSource, TItemVM> CreateInstance(IBehaviorContext context) {
            TSourceObject sourceObject = this.GetValueNext<TSourceObject>(context, ValueStage.None);

            var vm = new MultiSelectionVM<TSourceObject, TItemSource, TItemVM>(
               _descriptor,
               context.ServiceLocator
            );

            vm.InitializeFrom(sourceObject);

            vm.ActiveItemFilter = _filter;

            return vm;
         }
      }

      private class LocatingItemSourceBehavior :
         Behavior,
         IValueAccessorBehavior<IEnumerable<TItemSource>> {

         public IEnumerable<TItemSource> GetValue(IBehaviorContext context, ValueStage stage) {
            // TODO: Better error message
            return context.ServiceLocator.GetInstance<IEnumerable<TItemSource>>();
         }

         public void SetValue(IBehaviorContext context, IEnumerable<TItemSource> value) {
            throw new NotSupportedException();
         }
      }

   }
}
