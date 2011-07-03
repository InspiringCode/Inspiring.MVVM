namespace Inspiring.Mvvm.Views {

   public interface IView<TModel> : IBindableView<TModel> {
      TModel Model { set; }
   }

   public interface IBindableView<out TModel> {
   }
}
