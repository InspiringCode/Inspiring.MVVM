namespace Inspiring.Mvvm.ViewModels {

   public interface IHasSourceObject<TSourceObject> {
      TSourceObject Source { get; set; }
   }
}
