namespace Inspiring.Mvvm.ViewModels.SingleSelection {
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
      internal Func<IVMPropertyBuilder<TSourceObject>, IVMProperty<TItemSource>> SelectedSourceItemPropertyFactory {
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

      public IVMProperty<SingleSelectionVM<TItemSource, TItemVM>> Of<TItemVM>(
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

         var property = _sourceObjectPropertyBuilder.VM.Custom(
            viewModelAccessor: new SingleSelectionFactory<TItemVM>(descriptor, Filter)
         );

         _sourceObjectPropertyBuilder
            .Configuration
            .PropertyConfigurations[property]
            .Enable(
               BehaviorKeys.ManualUpdateBehavior,
               new ManualUpdateSelectionPropertyBehavior<SingleSelectionVM<TItemSource, TItemVM>, TSourceObject>()
            );

         return property;
      }

      public IVMProperty<SingleSelectionVM<TItemSource>> WithCaption(
         Func<TItemSource, string> captionGetter
      ) {
         Contract.Requires<ArgumentNullException>(captionGetter != null);
         Contract.Assert(SelectedSourceItemPropertyFactory != null);

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
         var descriptor = SingleSelectionWithSourceVM<TSourceObject, TItemSource>.CreateDescriptor(
            itemDescriptor,
            SelectedSourceItemPropertyFactory,
            allSourceItemsPropertyFactory
         );

         var property = _sourceObjectPropertyBuilder.VM.Custom(
            viewModelAccessor: new SingleSelectionFactory(descriptor, Filter)
         );

         _sourceObjectPropertyBuilder
            .Configuration
            .PropertyConfigurations[property]
            .Enable(
               BehaviorKeys.ManualUpdateBehavior,
               new ManualUpdateSelectionPropertyBehavior<SingleSelectionVM<TItemSource, SelectionItemVM<TItemSource>>, TSourceObject>()
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

      private class SingleSelectionFactory<TItemVM> :
         Behavior,
         IValueAccessorBehavior<SingleSelectionVM<TItemSource, TItemVM>>
         where TItemVM : IViewModel, IVMCollectionItem<TItemSource> {

         private SingleSelectionVMDescriptor<TItemSource, TItemVM> _descriptor;
         private Func<TItemSource, bool> _filter;

         public SingleSelectionFactory(SingleSelectionVMDescriptor<TItemSource, TItemVM> descriptor, Func<TItemSource, bool> filter) {
            _descriptor = descriptor;
            _filter = filter;
         }

         public SingleSelectionVM<TItemSource, TItemVM> GetValue(IBehaviorContext context) {
            TSourceObject sourceObject = this.GetValueNext<TSourceObject>(context);

            var vm = new SingleSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>(
               _descriptor,
               context.ServiceLocator
            );

            vm.InitializeFrom(sourceObject);
            vm.ActiveItemFilter = _filter;

            return vm;
         }

         public void SetValue(IBehaviorContext context, SingleSelectionVM<TItemSource, TItemVM> value) {
            throw new NotSupportedException();
         }
      }

      private class SingleSelectionFactory :
         Behavior,
         IValueAccessorBehavior<SingleSelectionVM<TItemSource>> {

         private SingleSelectionVMDescriptor<TItemSource> _descriptor;
         private Func<TItemSource, bool> _filter;

         public SingleSelectionFactory(SingleSelectionVMDescriptor<TItemSource> descriptor, Func<TItemSource, bool> filter) {
            _descriptor = descriptor;
            _filter = filter;
         }

         public SingleSelectionVM<TItemSource> GetValue(IBehaviorContext context) {
            TSourceObject sourceObject = this.GetValueNext<TSourceObject>(context);

            var vm = new SingleSelectionWithSourceVM<TSourceObject, TItemSource>(
               _descriptor,
               context.ServiceLocator
            );

            vm.InitializeFrom(sourceObject);
            vm.ActiveItemFilter = _filter;

            return vm;
         }

         public void SetValue(IBehaviorContext context, SingleSelectionVM<TItemSource> value) {
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
