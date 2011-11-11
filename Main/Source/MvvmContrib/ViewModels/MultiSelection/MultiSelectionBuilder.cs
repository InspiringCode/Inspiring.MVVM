namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core;

   public class MultiSelectionBuilder<TSourceObject, TItemSource> {
      private readonly IVMPropertyBuilder<TSourceObject> _sourceObjectPropertyBuilder;
      private Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<ICollection<TItemSource>>> _selectedSourceItemsPropertyFactory;
      private Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<IEnumerable<TItemSource>>> _allSourceItemsPropertyFactory;
      private Func<TSourceObject, TItemSource, bool> _filter;
      private bool _validationIsEnabled;
      private bool _undoIsEnabled;

      /// <param name="sourceObjectPropertyBuilder">
      ///   The original <see cref="VMPropertyBuilder"/> that was extended by
      ///   with the extension method.
      /// </param>
      internal MultiSelectionBuilder(
         IVMPropertyBuilder<TSourceObject> sourceObjectPropertyBuilder,
         Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<ICollection<TItemSource>>> selectedSourceItemsPropertyFactory
       ) {
         Contract.Requires(sourceObjectPropertyBuilder != null);
         _sourceObjectPropertyBuilder = sourceObjectPropertyBuilder;
         _selectedSourceItemsPropertyFactory = selectedSourceItemsPropertyFactory;
      }

      public MultiSelectionBuilder<TSourceObject, TItemSource> EnableValidations() {
         _validationIsEnabled = true;
         return this;
      }

      public MultiSelectionBuilder<TSourceObject, TItemSource> EnableUndo() {
         _undoIsEnabled = true;
         return this;
      }

      public MultiSelectionBuilder<TSourceObject, TItemSource> WithFilter(Func<TSourceObject, TItemSource, bool> filter) {
         _filter = filter;
         return this;
      }

      public MultiSelectionBuilder<TSourceObject, TItemSource> WithItems(
         Func<TSourceObject, IEnumerable<TItemSource>> allSourceItemsSelector
      ) {
         _allSourceItemsPropertyFactory = delegate(IVMPropertyBuilder<TSourceObject> factory) {
            return factory.Property.DelegatesTo(allSourceItemsSelector);
         };

         return this;
      }

      public IVMPropertyDescriptor<MultiSelectionVM<TItemSource, TItemVM>> Of<TItemVM>(
         Action<MultiSelectionDescriptorBuilder<TSourceObject, TItemSource, TItemVM>> descriptorConfigurationAction = null
      )
         where TItemVM : IViewModel, IHasSourceObject<TItemSource> {
         Contract.Assert(_selectedSourceItemsPropertyFactory != null);

         var allSourceItemsPropertyFactory =
            _allSourceItemsPropertyFactory ??
            CreateLocatingPropertyFactory();

         var descriptorBuilder = new MultiSelectionDescriptorBuilder<TSourceObject, TItemSource, TItemVM>(
            _filter
         );

         descriptorBuilder.WithProperties((d, b) => {
            var s = b.GetPropertyBuilder(x => x.SourceObject);

            d.AllSourceItems = allSourceItemsPropertyFactory(s);
            d.SelectedSourceItems = _selectedSourceItemsPropertyFactory(s);
         });

         if (_undoIsEnabled) {
            descriptorBuilder.WithViewModelBehaviors(b => {
               b.EnableUndo();
            });
         }

         if (descriptorConfigurationAction != null) {
            descriptorConfigurationAction(descriptorBuilder);
         }

         var descriptor = descriptorBuilder.Build();

         var property = _sourceObjectPropertyBuilder.Custom.ViewModelProperty(
            valueAccessor: new MultSelectionAccessor<TItemVM>(descriptor),
            sourceAccessor: _sourceObjectPropertyBuilder.Custom.CreateSourceObjectAccessor()
         );

         return property;
      }

      public IVMPropertyDescriptor<MultiSelectionVM<TItemSource>> WithCaption(
         Func<TItemSource, string> captionGetter,
         Action<MultiSelectionWithCaptionDescriptorBuilder<TSourceObject, TItemSource>> descriptorConfigurationAction = null
      ) {
         Contract.Requires<ArgumentNullException>(captionGetter != null);
         Contract.Assert(_selectedSourceItemsPropertyFactory != null);

         SelectionItemVMDescriptor itemDescriptor = VMDescriptorBuilder
            .OfType<SelectionItemVMDescriptor>()
            .For<SelectionItemVM<TItemSource>>()
            .WithProperties((d, c) => {
               var s = c.GetPropertyBuilder(x => x.Source);

               d.Caption = s.Property.DelegatesTo(captionGetter);
            })
            .WithValidators(b => b.EnableParentViewModelValidation())
            .Build();


         var allSourceItemsPropertyFactory =
            _allSourceItemsPropertyFactory ??
            CreateLocatingPropertyFactory();

         var descriptorBuilder = new MultiSelectionWithCaptionDescriptorBuilder<TSourceObject, TItemSource>(
            _filter,
            itemDescriptor
         );

         descriptorBuilder.WithProperties((d, b) => {
            var s = b.GetPropertyBuilder(x => x.Source);

            d.AllSourceItems = allSourceItemsPropertyFactory(s);
            d.SelectedSourceItems = _selectedSourceItemsPropertyFactory(s);
         });

         if (_undoIsEnabled) {
            descriptorBuilder.WithViewModelBehaviors(b => {
               b.EnableUndo();
            });
         }

         if (descriptorConfigurationAction != null) {
            descriptorConfigurationAction(descriptorBuilder);
         }

         var descriptor = descriptorBuilder.Build();

         var property = _sourceObjectPropertyBuilder.Custom.ViewModelProperty(
            valueAccessor: new MultSelectionAccessor(descriptor),
            sourceAccessor: _sourceObjectPropertyBuilder.Custom.CreateSourceObjectAccessor()
         );

         return property;
      }


      /// <summary>
      ///   If no source for 'AllItems' was specified, all items are by default 
      ///   resolved by asking the <see cref="IServiceLocator"/> of the source
      ///   object for an <see cref="IEnumerable{T}"/>.
      /// </summary>
      private Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<IEnumerable<TItemSource>>> CreateLocatingPropertyFactory() {
         return delegate(IVMPropertyBuilder<TSourceObject> factory) {
            // Only PropertyWithSource is cachable!
            return factory.Custom.PropertyWithSource(valueAccessor: new LocatingItemSourceBehavior());
         };
      }

      private class MultSelectionAccessor<TItemVM> :
         CachedAccessorBehavior<MultiSelectionVM<TItemSource, TItemVM>>,
         IRefreshBehavior
         where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

         private MultiSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>> _descriptor;

         public MultSelectionAccessor(
            MultiSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>> descriptor
         ) {
            _descriptor = descriptor;
         }

         protected override MultiSelectionVM<TItemSource, TItemVM> ProvideValue(IBehaviorContext context) {
            TSourceObject sourceObject = this.GetValueNext<TSourceObject>(context);

            var vm = new MultiSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>(
               _descriptor,
               context.ServiceLocator
            );

            vm.InitializeFrom(sourceObject);

            return vm;
         }

         public void Refresh(IBehaviorContext context, RefreshOptions options) {
            IViewModel vm = GetValue(context);
            vm.Kernel.Refresh(options.ExecuteRefreshDependencies);
            this.RefreshNext(context, options);
         }
      }

      private class MultSelectionAccessor :
         CachedAccessorBehavior<MultiSelectionVM<TItemSource>>,
         IRefreshBehavior {

         private MultiSelectionVMDescriptor<TItemSource> _descriptor;

         public MultSelectionAccessor(MultiSelectionVMDescriptor<TItemSource> descriptor) {
            _descriptor = descriptor;
         }

         protected override MultiSelectionVM<TItemSource> ProvideValue(IBehaviorContext context) {
            TSourceObject sourceObject = this.GetValueNext<TSourceObject>(context);

            var vm = new MultiSelectionWithSourceVM<TSourceObject, TItemSource>(
               _descriptor,
               context.ServiceLocator
            );

            vm.InitializeFrom(sourceObject);

            return vm;
         }

         public void Refresh(IBehaviorContext context, RefreshOptions options) {
            IViewModel vm = GetValue(context);
            vm.Kernel.Refresh(options.ExecuteRefreshDependencies);
            this.RefreshNext(context, options);
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
