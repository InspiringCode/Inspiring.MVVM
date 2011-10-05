﻿//namespace Inspiring.Mvvm.ViewModels {
//   using System;
//   using System.Linq;
//   using System.Linq.Expressions;
//   using Inspiring.Mvvm.ViewModels.Core;
//   using Inspiring.Mvvm.ViewModels.Core;

//   public interface IEnumSelectionPropertyBuilder<TParentVM> where TParentVM : IViewModel {
//      SingleSelectionProperty<TEnum> Mapped<TEnum>(
//         Expression<Func<TParentVM, TEnum>> selectedItemSelector,
//         Action<
//            SingleSelectionVMDescriptor<TEnum, SelectionItemVM<TEnum>>,
//            IValidationBuilder<SingleSelectionVM<TEnum, SelectionItemVM<TEnum>>>
//         > validationConfigurator = null
//      );
//   }

//   internal sealed class EnumSelectionPropertyBuilder<TParentVM> :
//      IEnumSelectionPropertyBuilder<TParentVM>
//      where TParentVM : IViewModel {

//      private IVMPropertyBuilder<TParentVM> _propertyFactory;

//      public EnumSelectionPropertyBuilder(
//         IVMPropertyBuilder<TParentVM> propertyFactory
//      ) {
//         _propertyFactory = propertyFactory;
//      }



//      public SingleSelectionProperty<TEnum> Mapped<TEnum>(
//         Expression<Func<TParentVM, TEnum>> selectedItemSelector,
//         Action<
//            SingleSelectionVMDescriptor<TEnum, SelectionItemVM<TEnum>>,
//            IValidationBuilder<SingleSelectionVM<TEnum, SelectionItemVM<TEnum>>>
//         > validationConfigurator = null
//      ) {
//         return _propertyFactory.SingleSelection()
//            .WithItems(vm => GetEnumValues<TEnum>())
//            .WithSelection(selectedItemSelector)
//            .Of(
//               i => {
//                  return new SelectionItemVMDescriptor {
//                     Caption = i.Property.DelegatesTo(x => EnumLocalizer.GetCaption(x))
//                  };
//               },
//               validationConfigurator
//            );
//      }

//      private static TEnum[] GetEnumValues<TEnum>() {
//         return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
//      }
//   }
//}