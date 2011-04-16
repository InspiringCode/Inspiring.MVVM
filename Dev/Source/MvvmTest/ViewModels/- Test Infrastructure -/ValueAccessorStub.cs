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

   public class ValueAccessorStub {
      public static ValueAccessorStub<T> Of<T>() {
         return new ValueAccessorStub<T>();
      }

      public static ValueAccessorStub<T> WithInitialValue<T>(T value) {
         return new ValueAccessorStub<T>() { Value = value };
      }
   }
}
