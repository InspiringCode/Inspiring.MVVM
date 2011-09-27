namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core;

   public class MultiSelectionBuilder<TSourceObject, TItemSource> {
      private readonly IVMPropertyBuilder<TSourceObject> _sourceObjectPropertyBuilder;
      private Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<ICollection<TItemSource>>> _selectedSourceItemsPropertyFactory;
      private Func<IVMPropertyBuilder<TSourceObject>, IVMPropertyDescriptor<IEnumerable<TItemSource>>> _allSourceItemsPropertyFactory;
      private Func<TItemSource, bool> _filter;
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

      public MultiSelectionBuilder<TSourceObject, TItemSource> WithFilter(Func<TItemSource, bool> filter) {
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

      public IVMPropertyDescriptor<MultiSelectionVM<TItemSource, TItemVM>> Of<TItemVM>()
         where TItemVM : IViewModel, IHasSourceObject<TItemSource> {
         Contract.Assert(_selectedSourceItemsPropertyFactory != null);

         var allSourceItemsPropertyFactory =
            _allSourceItemsPropertyFactory ??
            CreateLocatingPropertyFactory();

         var descriptorBuilder = new SelectableItemMultiSelectionDescriptorBuilder<TSourceObject, TItemSource, TItemVM>(
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

         var descriptor = descriptorBuilder.Build();

         var property = _sourceObjectPropertyBuilder.Custom.ViewModelProperty(
            valueAccessor: new MultSelectionAccessor<TItemVM>(descriptor, _filter),
            sourceAccessor: _sourceObjectPropertyBuilder.Custom.CreateSourceObjectAccessor()
         );

         return property;
      }

      public IVMPropertyDescriptor<MultiSelectionVM<TItemSource>> WithCaption(
         Func<TItemSource, string> captionGetter
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

         var descriptorBuilder = new CaptionMultiSelectionDescriptorBuilder<TSourceObject, TItemSource>(
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

         var descriptor = descriptorBuilder.Build();

         var property = _sourceObjectPropertyBuilder.Custom.ViewModelProperty(
            valueAccessor: new MultSelectionAccessor(descriptor, _filter),
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
            return factory.Custom.Property(valueAccessor: new LocatingItemSourceBehavior());
         };
      }

      private class MultSelectionAccessor<TItemVM> :
         CachedAccessorBehavior<MultiSelectionVM<TItemSource, TItemVM>>,
         IRefreshBehavior
         where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

         private MultiSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>> _descriptor;
         private Func<TItemSource, bool> _filter;

         public MultSelectionAccessor(
            MultiSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>> descriptor,
            Func<TItemSource, bool> filter
         ) {
            _descriptor = descriptor;
            _filter = filter;
         }

         protected override MultiSelectionVM<TItemSource, TItemVM> ProvideValue(IBehaviorContext context) {
            TSourceObject sourceObject = this.GetValueNext<TSourceObject>(context);

            var vm = new MultiSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM>(
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

      private class MultSelectionAccessor :
         CachedAccessorBehavior<MultiSelectionVM<TItemSource>>,
         IRefreshBehavior {

         private MultiSelectionVMDescriptor<TItemSource> _descriptor;
         private Func<TItemSource, bool> _filter;

         public MultSelectionAccessor(MultiSelectionVMDescriptor<TItemSource> descriptor, Func<TItemSource, bool> filter) {
            _descriptor = descriptor;
            _filter = filter;
         }

         protected override MultiSelectionVM<TItemSource> ProvideValue(IBehaviorContext context) {
            TSourceObject sourceObject = this.GetValueNext<TSourceObject>(context);

            var vm = new MultiSelectionWithSourceVM<TSourceObject, TItemSource>(
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
