namespace Inspiring.Mvvm.Views {
   using System;
   using System.ComponentModel;
   using System.Windows;
   using System.Windows.Input;

   public static class CommonBehaviors {
      public static readonly DependencyProperty DisableIfCommandIsNullProperty = DependencyProperty.RegisterAttached(
         "DisableIfCommandIsNull",
         typeof(bool),
         typeof(CommonBehaviors),
         new PropertyMetadata(HandleDisableIfCommandIsNullChanged)
      );

      public static bool GetDisableIfCommandIsNull(DependencyObject obj) {
         return (bool)obj.GetValue(DisableIfCommandIsNullProperty);
      }

      public static void SetDisableIfCommandIsNull(DependencyObject obj, bool value) {
         obj.SetValue(DisableIfCommandIsNullProperty, value);
      }

      private static void HandleDisableIfCommandIsNullChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
         ICommandSource commandSource = sender as ICommandSource;
         UIElement element = sender as UIElement;

         if (commandSource == null || sender == null) {
            throw new ArgumentException(
               ExceptionTexts.InvalidTargetObjectForCommonBehavior.FormatWith("DisableIfCommandIsNull")
            );
         }

         var descriptor = DependencyPropertyDescriptor.FromName(
            "Command",
            sender.GetType(),
            sender.GetType()
         );

         descriptor.RemoveValueChanged(sender, HandleCommandChanged);
         if ((bool)e.NewValue) {
            descriptor.AddValueChanged(sender, HandleCommandChanged);
         }

         UpdateIsEnabled(element);
      }

      private static void HandleCommandChanged(object sender, EventArgs e) {
         UpdateIsEnabled((UIElement)sender);
      }

      private static void UpdateIsEnabled(UIElement element) {
         ICommandSource commandSource = (ICommandSource)element;

         if (commandSource.Command == null) {
            element.IsEnabled = false;
         } else {
            element.ClearValue(UIElement.IsEnabledProperty);
            element.CoerceValue(UIElement.IsEnabledProperty);
         }
      }
   }
}
