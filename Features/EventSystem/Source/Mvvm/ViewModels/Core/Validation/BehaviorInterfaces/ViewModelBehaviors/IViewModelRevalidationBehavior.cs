namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IViewModelRevalidationBehavior : IBehavior {
      void Revalidate(IBehaviorContext context, ValidationController controller);
   }
}
