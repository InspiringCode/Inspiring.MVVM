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

         if (commandSource == null || !(sender is UIElement || sender is ContentElement)) {
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

         UpdateIsEnabled(sender);
      }

      private static void HandleCommandChanged(object sender, EventArgs e) {
         UpdateIsEnabled(sender);
      }

      private static void UpdateIsEnabled(object element) {
         ICommandSource commandSource = (ICommandSource)element;

         UIElement uiElement = element as UIElement;
         ContentElement contentElement = element as ContentElement;

         if (uiElement != null) {
            if (commandSource.Command == null) {
               uiElement.IsEnabled = false;
            } else {
               uiElement.ClearValue(UIElement.IsEnabledProperty);
               uiElement.CoerceValue(UIElement.IsEnabledProperty);
            }
         } else if (contentElement != null) {
            if (commandSource.Command == null) {
               contentElement.IsEnabled = false;
            } else {
               contentElement.CoerceValue(ContentElement.IsEnabledProperty);
               contentElement.CoerceValue(ContentElement.IsEnabledProperty);
            }
         } else {
            throw new ArgumentException();
         }
      }
   }
}
