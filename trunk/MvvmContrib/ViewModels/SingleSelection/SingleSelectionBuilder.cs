﻿namespace Inspiring.Mvvm.ViewModels.SingleSelection {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Fluent;

   public class SingleSelectionBuilder<TSourceObject, TItemSource> {
      private IVMPropertyBuilder<TSourceObject> _sourceObjectPropertyBuilder;

      /// <param name="sourceObjectPropertyBuilder">
      ///   The original <see cref="VMPropertyBuilder"/> that was extended by
      ///   with the extension method.
      /// </param>
      internal SingleSelectionBuilder(IVMPropertyBuilder<TSourceObject> sourceObjectPropertyBuilder) {
         Contract.Requires(sourceObjectPropertyBuilder != null);
         _sourceObjectPropertyBuilder = sourceObjectPropertyBuilder;
      }

      /// <summary>
      ///   Caches the property builder.
      /// </summary>
      internal Func<IVMPropertyBuilder<TSourceObject>, VMProperty<TItemSource>> SelectedSourceItemPropertyFactory {
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

      public SingleSelectionBuilder<TSourceObject, TItemSource> WithFilter(Func<TItemSource, bool> filter) {
         Filter = filter;
         return this;
      }

      public SingleSelectionBuilder<TSourceObject, TItemSource> WithItems(
         Func<TSourceObject, IEnumerable<TItemSource>> allSourceItemsSelector
      ) {
         AllSourceItemsPropertyFactory = delegate(IVMPropertyBuilder<TSourceObject> factory) {
            return factory.Property.DelegatesTo(allSourceItemsSelector);
         };

         return this;
      }

      public VMProperty<SingleSelectionVM<TItemSource, TItemVM>> Of<TItemVM>(
         VMDescriptorBase itemDescriptor
      ) where TItemVM : IViewModel, IVMCollectionItem<TItemSource> {
         Contract.Assert(SelectedSourceItemPropertyFactory != null);

         var allSourceItemsPropertyFactory =
            AllSourceItemsPropertyFactory ??
            CreateLocatingPropertyFactory();

         // The descriptor is created only once for every owner VM property/descriptor
         // and reused for every VM instance created from the owner VM descriptor.
         var descriptor = SingleSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>.CreateDescriptor(
            itemDescriptor,
            SelectedSourceItemPropertyFactory,
            allSourceItemsPropertyFactory
         );

         return _sourceObjectPropertyBuilder.VM.Custom(
            viewModelFactory: new SingleSelectionFactory<TItemVM>(descriptor, Filter)
         );
      }

      public VMProperty<SingleSelectionVM<TItemSource>> WithCaption(
         Func<TItemSource, string> captionGetter
      )  {
         Contract.Requires<ArgumentNullException>(captionGetter != null);
         Contract.Assert(SelectedSourceItemPropertyFactory != null);

         SelectionItemVMDescriptor itemDescriptor = VMDescriptorBuilder
            .For<SelectionItemVM<TItemSource>>()
            .CreateDescriptor(c => {
               var s = c.GetPropertyBuilder(x => x.Source);

               return new SelectionItemVMDescriptor {
                  Caption = s.Property.DelegatesTo(captionGetter)
               };
            })
            .Build();


         var allSourceItemsPropertyFactory =
            AllSourceItemsPropertyFactory ??
            CreateLocatingPropertyFactory();

         // The descriptor is created only once for every owner VM property/descriptor
         // and reused for every VM instance created from the owner VM descriptor.
         var descriptor = SingleSelectionWithSourceVM<TSourceObject, TItemSource>.CreateDescriptor(
            itemDescriptor,
            SelectedSourceItemPropertyFactory,
            allSourceItemsPropertyFactory
         );

         return _sourceObjectPropertyBuilder.VM.Custom(
            viewModelFactory: new SingleSelectionFactory(descriptor, Filter)
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

      private class SingleSelectionFactory<TItemVM> :
         Behavior,
         IViewModelFactoryBehavior<SingleSelectionVM<TItemSource, TItemVM>>
         where TItemVM : IViewModel, IVMCollectionItem<TItemSource> {

         private SingleSelectionVMDescriptor<TItemSource, TItemVM> _descriptor;
         private Func<TItemSource, bool> _filter;

         public SingleSelectionFactory(SingleSelectionVMDescriptor<TItemSource, TItemVM> descriptor, Func<TItemSource, bool> filter) {
            _descriptor = descriptor;
            _filter = filter;
         }

         public SingleSelectionVM<TItemSource, TItemVM> CreateInstance(IBehaviorContext context) {
            TSourceObject sourceObject = this.GetValueNext<TSourceObject>(context, ValueStage.None);

            var vm = new SingleSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>(
               _descriptor,
               context.ServiceLocator
            );

            vm.InitializeFrom(sourceObject);

            vm.ActiveItemFilter = _filter;

            return vm;
         }
      }

      private class SingleSelectionFactory :
         Behavior,
         IViewModelFactoryBehavior<SingleSelectionVM<TItemSource>> {

         private SingleSelectionVMDescriptor<TItemSource> _descriptor;
         private Func<TItemSource, bool> _filter;

         public SingleSelectionFactory(SingleSelectionVMDescriptor<TItemSource> descriptor, Func<TItemSource, bool> filter) {
            _descriptor = descriptor;
            _filter = filter;
         }

         public SingleSelectionVM<TItemSource> CreateInstance(IBehaviorContext context) {
            TSourceObject sourceObject = this.GetValueNext<TSourceObject>(context, ValueStage.None);

            var vm = new SingleSelectionWithSourceVM<TSourceObject, TItemSource>(
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
