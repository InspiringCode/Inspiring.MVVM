namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core;

   public class SingleSelectionBuilder<TSourceObject, TItemSource> {
      private readonly IVMPropertyBuilder<TSourceObject> _sourceObjectPropertyBuilder;
      private Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<TItemSource>> _selectedSourceItemPropertyFactory;
      private Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<IEnumerable<TItemSource>>> _allSourceItemsPropertyFactory;
      private Func<TSourceObject, TItemSource, bool> _filter;
      private bool _validationIsEnabled;
      private bool _undoIsEnabled;

      /// <param name="sourceObjectPropertyBuilder">
      ///   The original <see cref="VMPropertyBuilder"/> that was extended by
      ///   with the extension method.
      /// </param>
      internal SingleSelectionBuilder(
         IVMPropertyBuilder<TSourceObject> sourceObjectPropertyBuilder,
         Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<TItemSource>> selectedSourceItemPropertyFactory
      ) {
         Check.NotNull(sourceObjectPropertyBuilder, nameof(sourceObjectPropertyBuilder));
         _sourceObjectPropertyBuilder = sourceObjectPropertyBuilder;
         _selectedSourceItemPropertyFactory = selectedSourceItemPropertyFactory;
      }

      public SingleSelectionBuilder<TSourceObject, TItemSource> EnableValidations() {
         _validationIsEnabled = true;
         return this;
      }

      public SingleSelectionBuilder<TSourceObject, TItemSource> EnableUndo() {
         _undoIsEnabled = true;
         return this;
      }

      public SingleSelectionBuilder<TSourceObject, TItemSource> WithFilter(Func<TSourceObject, TItemSource, bool> filter) {
         _filter = filter;
         return this;
      }

      public SingleSelectionBuilder<TSourceObject, TItemSource> WithItems(
         Func<TSourceObject, IEnumerable<TItemSource>> allSourceItemsSelector
      ) {
         _allSourceItemsPropertyFactory = delegate (IVMPropertyBuilder<TSourceObject> factory) {
            return factory.Property.DelegatesTo(allSourceItemsSelector);
         };

         return this;
      }

      public IVMPropertyDescriptor<SingleSelectionVM<TItemSource, TItemVM>> Of<TItemVM>(
         Action<SingleSelectionDescriptorBuilder<TSourceObject, TItemSource, TItemVM>> descriptorConfigurationAction = null
      )
         where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

         Check.Requires<InvalidOperationException>(_selectedSourceItemPropertyFactory != null);

         var allSourceItemsPropertyFactory =
            _allSourceItemsPropertyFactory ??
            CreateLocatingPropertyFactory();

         var descriptorBuilder = new SingleSelectionDescriptorBuilder<TSourceObject, TItemSource, TItemVM>(_filter);

         descriptorBuilder.WithProperties((d, b) => {
            var s = b.GetPropertyBuilder(x => x.SourceObject);

            d.AllSourceItems = allSourceItemsPropertyFactory(s);
            d.SelectedSourceItem = _selectedSourceItemPropertyFactory(s);
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

         var property = _sourceObjectPropertyBuilder.Custom.ViewModelProperty<SingleSelectionVM<TItemSource, TItemVM>>(
            valueAccessor: new SingleSelectionFactory<TItemVM>(descriptor),
            sourceAccessor: _sourceObjectPropertyBuilder.Custom.CreateSourceObjectAccessor()
         );

         return property;
      }

      public IVMPropertyDescriptor<SingleSelectionVM<TItemSource>> WithCaption(
         Func<TItemSource, string> captionGetter,
         Action<SingleSelectionWithCaptionDescriptorBuilder<TSourceObject, TItemSource>> descriptorConfigurationAction = null
      ) {
         Check.NotNull(captionGetter, nameof(captionGetter));
         Check.Requires<InvalidOperationException>(_selectedSourceItemPropertyFactory != null);

         var builder = VMDescriptorBuilder
            .OfType<SelectionItemVMDescriptor>()
            .For<SelectionItemVM<TItemSource>>()
            .WithProperties((d, c) => {
               var s = c.GetPropertyBuilder(x => x.Source);

               d.Caption = s.Property.DelegatesTo(captionGetter);
            });

         if (_validationIsEnabled) {
            builder = builder.WithValidators(b => b.EnableParentValidation(x => x.Caption));
         }

         SelectionItemVMDescriptor itemDescriptor = builder.Build();

         var allSourceItemsPropertyFactory =
            _allSourceItemsPropertyFactory ??
            CreateLocatingPropertyFactory();

         var descriptorBuilder = new SingleSelectionWithCaptionDescriptorBuilder<TSourceObject, TItemSource>(
            _filter,
            itemDescriptor
         );

         descriptorBuilder.WithProperties((d, b) => {
            var s = b.GetPropertyBuilder(x => x.Source);

            d.AllSourceItems = allSourceItemsPropertyFactory(s);
            d.SelectedSourceItem = _selectedSourceItemPropertyFactory(s);
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

         var property = _sourceObjectPropertyBuilder.Custom.ViewModelProperty<SingleSelectionVM<TItemSource>>(
            valueAccessor: new SingleSelectionFactory(descriptor),
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
         return delegate (IVMPropertyBuilder<TSourceObject> factory) {
            // Only PropertyWithSource is cachable!
            return factory.Custom.PropertyWithSource<IEnumerable<TItemSource>>(valueAccessor: new LocatingItemSourceBehavior());
         };
      }

      private class SingleSelectionFactory<TItemVM> :
         CachedAccessorBehavior<SingleSelectionVM<TItemSource, TItemVM>>,
         IRefreshBehavior
         where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

         private SingleSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>> _descriptor;

         public SingleSelectionFactory(
            SingleSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>> descriptor
         ) {
            _descriptor = descriptor;
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

            vm.InitializeFrom(sourceObject);

            return vm;
         }

         public void Refresh(IBehaviorContext context, RefreshOptions options) {
            IViewModel vm = GetValue(context);
            vm.Kernel.Refresh(options.ExecuteRefreshDependencies);
            this.RefreshNext(context, options);
         }
      }

      private class SingleSelectionFactory :
         CachedAccessorBehavior<SingleSelectionVM<TItemSource>>,
         IRefreshBehavior {

         private SingleSelectionVMDescriptor<TItemSource> _descriptor;

         public SingleSelectionFactory(SingleSelectionVMDescriptor<TItemSource> descriptor) {
            _descriptor = descriptor;
         }

         protected override SingleSelectionVM<TItemSource> ProvideValue(IBehaviorContext context) {
            TSourceObject sourceObject = this.GetValueNext<TSourceObject>(context);

            var vm = new SingleSelectionWithSourceVM<TSourceObject, TItemSource>(
               _descriptor,
               context.ServiceLocator
            );
            vm.InitializeFrom(sourceObject);

            return vm;
         }

         public override void SetValue(IBehaviorContext context, SingleSelectionVM<TItemSource> value) {
            throw new NotSupportedException();
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