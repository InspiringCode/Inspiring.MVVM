namespace Inspiring.Mvvm.Views {
   using System;
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

               var targetUIElement = ctx.TargetObject as UIElement;
               var targetContentElement = ctx.TargetObject as ContentElement;

               if (targetUIElement != null) {
                  targetUIElement.InputBindings.Add(binding);
               } else if (targetContentElement != null) {
                  targetContentElement.InputBindings.Add(binding);
               } else {
                  throw new NotSupportedException();
               }
            }
         );

         return expression;
      }
   }
}
