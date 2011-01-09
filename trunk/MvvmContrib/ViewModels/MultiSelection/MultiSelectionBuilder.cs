namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Core;

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
      internal Func<IVMPropertyBuilder<TSourceObject>, IVMProperty<ICollection<TItemSource>>> SelectedSourceItemsPropertyFactory {
         get;
         set;
      }

      /// <summary>
      ///   Caches the property builder.
      /// </summary>
      internal Func<IVMPropertyBuilder<TSourceObject>, IVMProperty<IEnumerable<TItemSource>>> AllSourceItemsPropertyFactory {
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

      public IVMProperty<MultiSelectionVM<TItemSource, TItemVM>> Of<TItemVM>(
         VMDescriptorBase itemDescriptor
      ) where TItemVM : IViewModel, IVMCollectionItem<TItemSource> {
         Contract.Assert(SelectedSourceItemsPropertyFactory != null);

         var allSourceItemsPropertyFactory =
            AllSourceItemsPropertyFactory ??
            CreateLocatingPropertyFactory();

         // The descriptor is created only once for every owner VM property/descriptor
         // and reused for every VM instance created from the owner VM descriptor.
         var descriptor = MultiSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>.CreateDescriptor(
            itemDescriptor,
            SelectedSourceItemsPropertyFactory,
            allSourceItemsPropertyFactory
         );

         var property = _sourceObjectPropertyBuilder.VM.Custom(
            viewModelAccessor: new MultSelectionAccessor<TItemVM>(descriptor, Filter)
         );

         _sourceObjectPropertyBuilder
            .Configuration
            .PropertyConfigurations[property]
            .Enable(
               BehaviorKeys.ManualUpdateBehavior,
               new ManualUpdateSelectionPropertyBehavior<MultiSelectionVM<TItemSource, TItemVM>, TSourceObject>()
            );

         return property;
      }

      public IVMProperty<MultiSelectionVM<TItemSource>> WithCaption(
         Func<TItemSource, string> captionGetter
      ) {
         Contract.Requires<ArgumentNullException>(captionGetter != null);
         Contract.Assert(SelectedSourceItemsPropertyFactory != null);

         SelectionItemVMDescriptor itemDescriptor = VMDescriptorBuilder
            .OfType<SelectionItemVMDescriptor>()
            .For<SelectionItemVM<TItemSource>>()
            .WithProperties((d, c) => {
               var s = c.GetPropertyBuilder(x => x.Source);

               d.Caption = s.Property.DelegatesTo(captionGetter);
            })
            .Build();


         var allSourceItemsPropertyFactory =
            AllSourceItemsPropertyFactory ??
            CreateLocatingPropertyFactory();

         // The descriptor is created only once for every owner VM property/descriptor
         // and reused for every VM instance created from the owner VM descriptor.
         var descriptor = MultiSelectionWithSourceVM<TSourceObject, TItemSource>.CreateDescriptor(
            itemDescriptor,
            SelectedSourceItemsPropertyFactory,
            allSourceItemsPropertyFactory
         );

         var property = _sourceObjectPropertyBuilder.VM.Custom(
            viewModelAccessor: new MultSelectionAccessor(descriptor, Filter)
         );

         _sourceObjectPropertyBuilder
            .Configuration
            .PropertyConfigurations[property]
            .Enable(
               BehaviorKeys.ManualUpdateBehavior,
               new ManualUpdateSelectionPropertyBehavior<MultiSelectionVM<TItemSource, SelectionItemVM<TItemSource>>, TSourceObject>()
            );

         return property;
      }


      /// <summary>
      ///   If no source for 'AllItems' was specified, all items are by default 
      ///   resolved by asking the <see cref="IServiceLocator"/> of the source
      ///   object for an <see cref="IEnumerable{T}"/>.
      /// </summary>
      private Func<IVMPropertyBuilder<TSourceObject>, IVMProperty<IEnumerable<TItemSource>>> CreateLocatingPropertyFactory() {
         return delegate(IVMPropertyBuilder<TSourceObject> factory) {
            return factory.Property.Custom(
               sourceValueAccessor: new LocatingItemSourceBehavior()
            );
         };
      }

      private class MultSelectionAccessor<TItemVM> :
         Behavior,
         IValueAccessorBehavior<MultiSelectionVM<TItemSource, TItemVM>>
         where TItemVM : IViewModel, IVMCollectionItem<TItemSource> {

         private MultiSelectionVMDescriptor<TItemSource, TItemVM> _descriptor;
         private Func<TItemSource, bool> _filter;

         public MultSelectionAccessor(MultiSelectionVMDescriptor<TItemSource, TItemVM> descriptor, Func<TItemSource, bool> filter) {
            _descriptor = descriptor;
            _filter = filter;
         }

         public MultiSelectionVM<TItemSource, TItemVM> GetValue(IBehaviorContext context) {
            TSourceObject sourceObject = this.GetValueNext<TSourceObject>(context);

            var vm = new MultiSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>(
               _descriptor,
               context.ServiceLocator
            );

            vm.InitializeFrom(sourceObject);
            vm.ActiveItemFilter = _filter;

            return vm;
         }

         public void SetValue(IBehaviorContext context, MultiSelectionVM<TItemSource, TItemVM> value) {
            throw new NotSupportedException();
         }
      }

      private class MultSelectionAccessor :
         Behavior,
         IValueAccessorBehavior<MultiSelectionVM<TItemSource>> {

         private MultiSelectionVMDescriptor<TItemSource> _descriptor;
         private Func<TItemSource, bool> _filter;

         public MultSelectionAccessor(MultiSelectionVMDescriptor<TItemSource> descriptor, Func<TItemSource, bool> filter) {
            _descriptor = descriptor;
            _filter = filter;
         }

         public MultiSelectionVM<TItemSource> GetValue(IBehaviorContext context) {
            TSourceObject sourceObject = this.GetValueNext<TSourceObject>(context);

            var vm = new MultiSelectionWithSourceVM<TSourceObject, TItemSource>(
               _descriptor,
               context.ServiceLocator
            );

            vm.InitializeFrom(sourceObject);

            vm.ActiveItemFilter = _filter;

            return vm;
         }

         public void SetValue(IBehaviorContext context, MultiSelectionVM<TItemSource> value) {
            throw new NotSupportedException();
         }
      }

      private class LocatingItemSourceBehavior :
         Behavior,
         IValueAccessorBehavior<IEnumerable<TItemSource>> {

         public IEnumerable<TItemSource> GetValue(IBehaviorContext context) {
            // TODO: Better error message
            return context.ServiceLocator.GetInstance<IEnumerable<TItemSource>>();
         }

         public void SetValue(IBehaviorContext context, IEnumerable<TItemSource> value) {
            throw new NotSupportedException();
         }
      }

   }
}
