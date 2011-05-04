﻿namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core;

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
      internal Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<TItemSource>> SelectedSourceItemPropertyFactory {
         get;
         set;
      }

      /// <summary>
      ///   Caches the property builder.
      /// </summary>
      internal Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<IEnumerable<TItemSource>>> AllSourceItemsPropertyFactory {
         get;
         set;
      }

      internal Func<TItemSource, bool> Filter {
         get;
         set;
      }

      internal bool ValidationIsEnabled {
         get;
         private set;
      }

      public SingleSelectionBuilder<TSourceObject, TItemSource> EnableValidations() {
         ValidationIsEnabled = true;
         return this;
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

      public IVMPropertyDescriptor<SingleSelectionVM<TItemSource, TItemVM>> Of<TItemVM>(
         IVMDescriptor itemDescriptor
      ) where TItemVM : IViewModel, IHasSourceObject<TItemSource> {
         Contract.Assert(SelectedSourceItemPropertyFactory != null);

         var allSourceItemsPropertyFactory =
            AllSourceItemsPropertyFactory ??
            CreateLocatingPropertyFactory();

         // The descriptor is created only once for every owner VM property/descriptor
         // and reused for every VM instance created from the owner VM descriptor.
         var descriptor = SingleSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>.CreateDescriptor(
            itemDescriptor,
            SelectedSourceItemPropertyFactory,
            allSourceItemsPropertyFactory,
            ValidationIsEnabled
         );

         var property = _sourceObjectPropertyBuilder.Custom.ViewModelProperty(
            valueAccessor: new SingleSelectionFactory<TItemVM>(descriptor, Filter),
            sourceAccessor: _sourceObjectPropertyBuilder.Custom.CreateSourceObjectAccessor()
         );

         //_sourceObjectPropertyBuilder
         //   .Configuration
         //   .PropertyConfigurations[property]
         //   .Enable(
         //      PropertyBehaviorKeys.ManualUpdateBehavior,
         //      new ManualUpdateSelectionPropertyBehavior<SingleSelectionVM<TItemSource, TItemVM>, TSourceObject>()
         //   );

         return property;
      }

      public IVMPropertyDescriptor<SingleSelectionVM<TItemSource>> WithCaption(
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
            allSourceItemsPropertyFactory,
            ValidationIsEnabled
         );

         var property = _sourceObjectPropertyBuilder.Custom.ViewModelProperty(
            valueAccessor: new SingleSelectionFactory(descriptor, Filter),
            sourceAccessor: _sourceObjectPropertyBuilder.Custom.CreateSourceObjectAccessor()
         );


         //_sourceObjectPropertyBuilder
         //   .Configuration
         //   .PropertyConfigurations[property]
         //   .Enable(
         //      PropertyBehaviorKeys.ManualUpdateBehavior,
         //      new ManualUpdateSelectionPropertyBehavior<SingleSelectionVM<TItemSource>, TSourceObject>()
         //   );

         return property;
      }


      /// <summary>
      ///   If no source for 'AllItems' was specified, all items are by default 
      ///   resolved by asking the <see cref="IServiceLocator"/> of the source
      ///   object for an <see cref="IEnumerable{T}"/>.
      /// </summary>
      private Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<IEnumerable<TItemSource>>> CreateLocatingPropertyFactory() {
         return delegate(IVMPropertyBuilder<TSourceObject> factory) {
            return factory.Custom.Property(valueAccessor: new LocatingItemSourceBehavior());
         };
      }

      private class SingleSelectionFactory<TItemVM> :
         CachedAccessorBehavior<SingleSelectionVM<TItemSource, TItemVM>>,
         IRefreshBehavior
         where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

         private SingleSelectionVMDescriptor<TItemSource, TItemVM> _descriptor;
         private Func<TItemSource, bool> _filter;

         public SingleSelectionFactory(SingleSelectionVMDescriptor<TItemSource, TItemVM> descriptor, Func<TItemSource, bool> filter) {
            _descriptor = descriptor;
            _filter = filter;
         }

         public override void SetValue(IBehaviorContext context, SingleSelectionVM<TItemSource, TItemVM> value) {
            throw new NotSupportedException();
         }

         protected override SingleSelectionVM<TItemSource, TItemVM> ProvideValue(IBehaviorContext context) {
            TSourceObject sourceObject = this.GetValueNext<TSourceObject>(context);

            var vm = new SingleSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>(
               _descriptor,
               context.ServiceLocator
            );

            vm.ActiveItemFilter = _filter;
            vm.InitializeFrom(sourceObject);

            return vm;
         }

         public void Refresh(IBehaviorContext context) {
            IViewModel vm = GetValue(context);
            vm.Kernel.Refresh();
            this.RefreshNext(context);
         }
      }

      private class SingleSelectionFactory :
         CachedAccessorBehavior<SingleSelectionVM<TItemSource>>,
         IRefreshBehavior {

         private SingleSelectionVMDescriptor<TItemSource> _descriptor;
         private Func<TItemSource, bool> _filter;

         public SingleSelectionFactory(SingleSelectionVMDescriptor<TItemSource> descriptor, Func<TItemSource, bool> filter) {
            _descriptor = descriptor;
            _filter = filter;
         }

         protected override SingleSelectionVM<TItemSource> ProvideValue(IBehaviorContext context) {
            TSourceObject sourceObject = this.GetValueNext<TSourceObject>(context);

            var vm = new SingleSelectionWithSourceVM<TSourceObject, TItemSource>(
               _descriptor,
               context.ServiceLocator
            );

            vm.ActiveItemFilter = _filter;
            vm.InitializeFrom(sourceObject);

            return vm;
         }

         public override void SetValue(IBehaviorContext context, SingleSelectionVM<TItemSource> value) {
            throw new NotSupportedException();
         }

         public void Refresh(IBehaviorContext context) {
            IViewModel vm = GetValue(context);
            vm.Kernel.Refresh();
            this.RefreshNext(context);
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
