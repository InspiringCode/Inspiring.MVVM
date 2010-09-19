namespace Inspiring.Mvvm.ViewModels {
   public interface ICreatableItem<TParentVM, TItemSource> where TParentVM : ViewModel {
      void OnNewItem(ItemCreationArguments<TItemSource> args, TParentVM parent);
   }

   public class ItemCreationArguments<TItemSource> {
      internal ItemCreationArguments() {
         AddToSourceCollection = AddItemTime.AfterEndNew;
      }

      public TItemSource NewSoureObject { get; set; }
      public AddItemTime AddToSourceCollection { get; set; }

      public bool IsStartNewItem { get; internal set; }
      public bool IsEndNewItem { get; internal set; }
      public bool IsCancelNewItem { get; internal set; }
   }

   public enum AddItemTime {
      Never,
      AfterStartNew,
      AfterEndNew
   }
}
