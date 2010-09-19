namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Linq;
   using System.Linq.Expressions;

   public interface IEnumSelectionPropertyBuilder<TParentVM> where TParentVM : ViewModel {
      SingleSelectionProperty<TEnum> Mapped<TEnum>(
         Expression<Func<TParentVM, TEnum>> selectedItemSelector
      );
   }

   internal sealed class EnumSelectionPropertyBuilder<TParentVM> :
      IEnumSelectionPropertyBuilder<TParentVM>
      where TParentVM : ViewModel {

      private IRootVMPropertyFactory<TParentVM> _propertyFactory;

      public EnumSelectionPropertyBuilder(
         IRootVMPropertyFactory<TParentVM> propertyFactory
      ) {
         _propertyFactory = propertyFactory;
      }

      public SingleSelectionProperty<TEnum> Mapped<TEnum>(
         Expression<Func<TParentVM, TEnum>> selectedItemSelector
      ) {
         return _propertyFactory.SingleSelection()
            .WithItems(vm => GetEnumValues<TEnum>())
            .WithSelection(selectedItemSelector)
            .Of(i => {
               return new SelectionItemVMDescriptor {
                  Caption = i.Calculated(x => EnumLocalizer.GetCaption(x))
               };
            });
      }

      private static TEnum[] GetEnumValues<TEnum>() {
         return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
      }
   }
}
