﻿namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Fluent;

   internal sealed class SingleSelectionPropertyBuilder<TParentVM> :
      ISingleSelectionPropertyBuilder<TParentVM>
      where TParentVM : IViewModel {

      private IVMPropertyFactory< TParentVM> _propertyFactory;

      public SingleSelectionPropertyBuilder(IVMPropertyFactory<TParentVM> propertyFactory) {
         _propertyFactory = propertyFactory;
      }

      public ISingleSelectionPropertyBuilder<TParentVM, TSourceItem> WithItems<TSourceItem>(
         Func<TParentVM, IEnumerable<TSourceItem>> sourceItemsGetter,
         Func<TSourceItem, bool> currentlySelectablePredicate = null
      ) {
         return new SingleSelectionPropertyBuilder<TParentVM, TSourceItem>(
            _propertyFactory,
            sourceItemsGetter,
            currentlySelectablePredicate
         );
      }
   }

   internal sealed class SingleSelectionPropertyBuilder<TParentVM, TSourceItem> :
      ISingleSelectionPropertyBuilder<TParentVM, TSourceItem>
      where TParentVM : IViewModel {

      private IVMPropertyFactory<TParentVM> _propertyFactory;
      private Func<TSourceItem, bool> _selectableItemFilter;

      private VMProperty<IEnumerable<TSourceItem>> _unfilteredSourceItemsProperty;
      private VMProperty<TSourceItem> _selectedSourceItemProperty;

      public SingleSelectionPropertyBuilder(
         IVMPropertyFactory<TParentVM> propertyFactory,
         Func<TParentVM, IEnumerable<TSourceItem>> sourceItemsGetter,
         Func<TSourceItem, bool> currentlySelectablePredicate
      ) {
         _propertyFactory = propertyFactory;
         _selectableItemFilter = currentlySelectablePredicate;
         _unfilteredSourceItemsProperty = _propertyFactory.Calculated(sourceItemsGetter).Property();
      }

      public ISingleSelectionPropertyBuilder<TParentVM, TSourceItem> WithSelection(
         Expression<Func<TParentVM, TSourceItem>> selectedItemSelector
      ) {
         _selectedSourceItemProperty = _propertyFactory.Mapped(selectedItemSelector).Property();
         return this;
      }

      public ISingleSelectionPropertyBuilder<TParentVM, TSourceItem> WithSelection(
         Func<TParentVM, TSourceItem> selectionGetter,
         Action<TParentVM, TSourceItem> selectionSetter
      ) {
         _selectedSourceItemProperty = _propertyFactory.Calculated(
            selectionGetter,
            selectionSetter
         ).Property();
         return this;
      }

      public SingleSelectionProperty<TSourceItem> Of(
         Func<IVMPropertyFactory<TSourceItem>, VMDescriptor> descriptorFactory,
         Action<
            SingleSelectionVMDescriptor<TSourceItem, SelectionItemVM<TSourceItem>>,
            IValidationBuilder<SingleSelectionVM<TSourceItem, SelectionItemVM<TSourceItem>>>
         > validationConfigurator = null
      ) {
         VMDescriptor itemDescriptor = VMDescriptorBuilder
            .For<SelectionItemVM<TSourceItem>>()
            .CreateDescriptor(c => {
               var factory = c.GetPropertyBuilder(x => x.SourceItem);

               return descriptorFactory(factory);
            })
            .Build();

         SingleSelectionVMDescriptor<TSourceItem, SelectionItemVM<TSourceItem>> descriptor =
               SingleSelectionVM<TSourceItem, SelectionItemVM<TSourceItem>>.CreateDescriptor(
                  _unfilteredSourceItemsProperty,
                  _selectedSourceItemProperty,
                  itemDescriptor,
                  validationConfigurator
               );

         BehaviorConfiguration config = new BehaviorConfiguration();
         config.Enable(VMBehaviorKey.PropertyValueCache);

         throw new NotImplementedException();
         //IRootVMPropertyFactory<TParentVM> configuredFactory = ViewModelExtensibility
         //   .ConfigurePropertyFactory(_propertyFactory, config);

         //return configuredFactory.Calculated<
         //   SingleSelectionProperty<TSourceItem>,
         //   SingleSelectionVM<TSourceItem, SelectionItemVM<TSourceItem>>>
         //(
         //   parentVM =>
         //      new SingleSelectionVM<TSourceItem, SelectionItemVM<TSourceItem>>(
         //         descriptor,
         //         parentVM,
         //         _selectableItemFilter
         //      )
         //);
      }

      public SingleSelectionProperty<TSourceItem, TItemVM> Of<TItemVM>(
         Func<_IVMPropertyFactory<SelectionItemVM<TSourceItem>, TSourceItem>, VMDescriptor> descriptorFactory
      ) where TItemVM : SelectionItemVM<TSourceItem> {
         throw new NotImplementedException("Not implemented yet: please tell me if you need it...");
      }


      public SingleSelectionProperty<TSourceItem> Of(Func<_IVMPropertyFactory<SelectionItemVM<TSourceItem>, TSourceItem>, VMDescriptor> descriptorFactory, Action<SingleSelectionVMDescriptor<TSourceItem, SelectionItemVM<TSourceItem>>, IValidationBuilder<SingleSelectionVM<TSourceItem, SelectionItemVM<TSourceItem>>>> validationConfigurator = null) {
         throw new NotImplementedException();
      }
   }


}
