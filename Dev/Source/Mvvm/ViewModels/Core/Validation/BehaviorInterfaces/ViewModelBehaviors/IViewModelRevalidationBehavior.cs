namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IViewModelRevalidationBehavior {
      void Revalidate(IBehaviorContext context, ValidationController controller);
   }
}
