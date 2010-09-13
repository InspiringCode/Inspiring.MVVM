using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
namespace Inspiring.Mvvm.Views.Binder {

   public sealed class DefaultPropertyBuildStep : IBinderBuildStep {
      private static Dictionary<Type, DependencyProperty> _defaultProperties =
         new Dictionary<Type, DependencyProperty>();

      static DefaultPropertyBuildStep() {
         DefineDefaultProperty(typeof(ItemsControl), ItemsControl.ItemsSourceProperty);
         DefineDefaultProperty(typeof(Button), Button.ContentProperty);
         DefineDefaultProperty(typeof(Label), Label.ContentProperty);
         DefineDefaultProperty(typeof(TextBlock), TextBlock.TextProperty);
         DefineDefaultProperty(typeof(TextBox), TextBox.TextProperty);
         DefineDefaultProperty(typeof(GroupBox), GroupBox.HeaderProperty);
         DefineDefaultProperty(typeof(ContentPresenter), ContentPresenter.ContentProperty);
         DefineDefaultProperty(typeof(RadioButton), RadioButton.IsCheckedProperty);
         DefineDefaultProperty(typeof(CheckBox), CheckBox.IsCheckedProperty);
      }

      public static void DefineDefaultProperty(Type controlType, DependencyProperty defaultProperty) {
         _defaultProperties[controlType] = defaultProperty;
      }

      public void Execute(BinderContext context) {
         if (context.TargetProperty == null) {
            context.TargetProperty = GetDefaultProperty(context.TargetObject);
         }
      }

      private DependencyProperty GetDefaultProperty(DependencyObject forControl) {
         DependencyProperty defaultProperty;

         for (Type t = forControl.GetType(); t != null; t = t.BaseType) {
            if (_defaultProperties.TryGetValue(t, out defaultProperty)) {
               return defaultProperty;
            }
         }

         return null;
      }
   }
}
