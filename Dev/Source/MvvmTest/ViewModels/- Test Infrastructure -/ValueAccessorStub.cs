namespace Inspiring.MvvmTest.ViewModels {
   using Inspiring.Mvvm.ViewModels.Core;

   public class ValueAccessorStub<T> : Behavior, IValueAccessorBehavior<T> {
      public T Value { get; set; }

      public T GetValue(IBehaviorContext context) {
         return Value;
      }

      public void SetValue(IBehaviorContext context, T value) {
         Value = value;
      }
   }
}
