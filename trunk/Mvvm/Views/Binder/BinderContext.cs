namespace Inspiring.Mvvm.Views.Binder {
   using System;
   using System.Windows;
   using System.Windows.Data;

   public class BinderContext {
      public BinderContext() {
         Binding = new Binding();
      }

      public string PropertyPath { get; private set; }

      public Type SourcePropertyType { get; set; }

      public DependencyObject TargetObject { get; set; }

      public DependencyProperty TargetProperty { get; set; }

      public Binding Binding { get; set; }

      public bool Complete { get; set; }

      public void ExtendPropertyPath(string propertyPathPostfix) {
         if (String.IsNullOrWhiteSpace(propertyPathPostfix)) {
            return;
         }

         PropertyPath = String.IsNullOrWhiteSpace(PropertyPath) ?
            propertyPathPostfix :
            PropertyPath + "." + propertyPathPostfix;
      }

      public void PrepareBinding() {
         Binding.Path = new PropertyPath(PropertyPath);
      }
   }
}
