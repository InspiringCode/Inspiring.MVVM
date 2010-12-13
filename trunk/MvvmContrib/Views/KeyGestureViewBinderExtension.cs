namespace Inspiring.Mvvm.Views {
   using System.Windows;
   using System.Windows.Data;
   using System.Windows.Input;
   using Inspiring.Mvvm.Views.Binder;

   public static class KeyGestureViewBinderExtension {
      public static IOptionsExpression<T> KeyGesture<T>(
         this IOptionsExpression<T> expression,
         Key key,
         ModifierKeys modifiers = ModifierKeys.None
      ) where T : ICommand {
         KeyGesture gesture = CustomKeyGesture.Create(key, modifiers);
         InputBinding binding = new KeyBinding { Gesture = gesture };

         BinderExpression.ExposeContext(
            expression,
            ctx => {
               
               BindingOperations.SetBinding(binding, KeyBinding.CommandProperty, ctx.Binding);

               var target = (UIElement)ctx.TargetObject;
               target.InputBindings.Add(binding);
            }
         );

         return expression;
      }
   }
}
