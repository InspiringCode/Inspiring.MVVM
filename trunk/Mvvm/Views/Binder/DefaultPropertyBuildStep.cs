namespace Inspiring.Mvvm.Views.Binder {
   using System;
   using System.Collections.Generic;
   using System.Windows;
   using System.Windows.Controls;
   using System.Windows.Controls.Primitives;
   using System.Windows.Input;
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.ViewModels;

   public sealed class DefaultPropertyBuildStep : IBinderBuildStep {
      private static Dictionary<Type, DependencyProperty> _defaultProperties =
         new Dictionary<Type, DependencyProperty>();

      private static Dictionary<Type, DependencyProperty> _commandProperties =
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

         DefineCommandProperty(typeof(ButtonBase), ButtonBase.CommandProperty);
      }

      public static void DefineDefaultProperty(Type controlType, DependencyProperty defaultProperty) {
         _defaultProperties[controlType] = defaultProperty;
      }

      public static void DefineCommandProperty(Type controlType, DependencyProperty commandProperty) {
         _commandProperties[controlType] = commandProperty;
      }

      public void Execute(BinderContext context) {
         if (context.TargetProperty == null) {
            if (typeof(IViewModel).IsAssignableFrom(context.SourcePropertyType) ||
                typeof(IScreen).IsAssignableFrom(context.SourcePropertyType)) {
               context.TargetProperty = View.ModelProperty;
            } else {
               context.TargetProperty = GetDefaultProperty(context);
            }
         }
      }

      private DependencyProperty GetDefaultProperty(BinderContext context) {
         DependencyProperty defaultProperty;

         var lookupDictionary = IsCommand(context.SourcePropertyType) ?
            _commandProperties :
            _defaultProperties;

         for (Type t = context.TargetObject.GetType(); t != null; t = t.BaseType) {
            if (lookupDictionary.TryGetValue(t, out defaultProperty)) {
               return defaultProperty;
            }
         }

         return null;
      }

      private bool IsCommand(Type type) {
         return typeof(ICommand).IsAssignableFrom(type);
      }
   }
}
