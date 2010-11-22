namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Linq;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.ViewModels.Core;

   public interface IEnumSelectionPropertyBuilder<TParentVM> where TParenTVM : IViewModel {
      SingleSelectionProperty<TEnum> Mapped<TEnum>(
         Expression<Func<TParentVM, TEnum>> selectedItemSelector,
         Action<
            SingleSelectionVMDescriptor<TEnum, SelectionItemVM<TEnum>>,
            IValidationBuilder<SingleSelectionVM<TEnum, SelectionItemVM<TEnum>>>
         > validationConfigurator = null
      );
   }

   internal sealed class EnumSelectionPropertyBuilder<TParentVM> :
      IEnumSelectionPropertyBuilder<TParentVM>
      where TParenTVM : IViewModel {

      private IRootVMPropertyFactory<TParentVM> _propertyFactory;

      public EnumSelectionPropertyBuilder(
         IRootVMPropertyFactory<TParentVM> propertyFactory
      ) {
         _propertyFactory = propertyFactory;
      }



      public SingleSelectionProperty<TEnum> Mapped<TEnum>(
         Expression<Func<TParentVM, TEnum>> selectedItemSelector,
         Action<
            SingleSelectionVMDescriptor<TEnum, SelectionItemVM<TEnum>>,
            IValidationBuilder<SingleSelectionVM<TEnum, SelectionItemVM<TEnum>>>
         > validationConfigurator = null
      ) {
         return _propertyFactory.SingleSelection()
            .WithItems(vm => GetEnumValues<TEnum>())
            .WithSelection(selectedItemSelector)
            .Of(
               i => {
                  return new SelectionItemVMDescriptor {
                     Caption = i.Calculated(x => EnumLocalizer.GetCaption(x))
                  };
               },
               validationConfigurator
            );
      }

      private static TEnum[] GetEnumValues<TEnum>() {
         return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
      }
   }
}
