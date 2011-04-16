namespace Inspiring.Mvvm.ViewModels.Core {

   public interface IUntypedValueSetterBehavior : IBehavior {
      void SetValue(IBehaviorContext context, object value);
   }
}
