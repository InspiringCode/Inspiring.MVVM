namespace Inspiring.MvvmTest {
   using System;
   using System.Windows;
   using System.Windows.Data;

   public static class WpfTestHelper {
      public static void SetupDataContext(DependencyObject rootElement, object dataContext) {
         ForEachDescendant(rootElement, e => {
            FrameworkElement fe = e as FrameworkElement;
            if (fe != null) {
               fe.DataContext = dataContext;
            }
         });
      }

      public static void UpdateSource(DependencyObject rootElement, DependencyProperty property) {
         ForEachDescendant(rootElement, e => {
            var exp = BindingOperations.GetBindingExpression(e, property);
            if (exp != null) {
               exp.UpdateSource();
            }
         });
      }

      private static void ForEachDescendant(DependencyObject root, Action<DependencyObject> action) {
         action(root);

         foreach (object child in LogicalTreeHelper.GetChildren(root)) {
            DependencyObject dp = child as DependencyObject;
            if (dp != null) {
               ForEachDescendant(dp, action);
            }
         }
      }
   }
}
