namespace Inspiring.Mvvm.Views {
   using System;
   using System.Diagnostics.Contracts;
   using System.Windows;
   using System.Windows.Controls;

   public static class View {
      public static readonly DependencyProperty ModelProperty = DependencyProperty.RegisterAttached(
         "Model",
         typeof(object),
         typeof(View),
         new PropertyMetadata(HandleModelChanged)
      );

      public static readonly DependencyProperty TitleProperty = DependencyProperty.RegisterAttached(
         "Title",
         typeof(string),
         typeof(View),
         new PropertyMetadata(String.Empty)
      );

      public static object GetModel(DependencyObject obj) {
         return (object)obj.GetValue(ModelProperty);
      }

      public static void SetModel(DependencyObject obj, object value) {
         obj.SetValue(ModelProperty, value);
      }

      public static string GetTitle(DependencyObject obj) {
         return (string)obj.GetValue(TitleProperty);
      }

      public static void SetTitle(DependencyObject obj, string value) {
         obj.SetValue(TitleProperty, value);
      }

      public static void HandleModelChanged(DependencyObject view, DependencyPropertyChangedEventArgs e) {
         Contract.Assert(view != null);

         object model = e.NewValue;

         //if (ViewFactory.TryInitializeView(view, model)) {
         //   return;
         //}

         ContentControl contentControl = view as ContentControl;
         if (contentControl != null) {
            if (!ViewFactory.TryInitializeView(contentControl.Content, model)) {
               contentControl.Content = ViewFactory.CreateView(model);
            }
            return;
         }

         ContentPresenter presenter = view as ContentPresenter;
         if (presenter != null) {
            if (!ViewFactory.TryInitializeView(presenter.Content, model)) {
               presenter.Content = ViewFactory.CreateView(model);
            }
            return;
         }

         // TODO: Update exception text...
         throw new ArgumentException(
            ExceptionTexts.UnsupportedTargetTypeForModelProperty.FormatWith(
               view.GetType().Name,
               e.NewValue != null ? model.GetType().Name : "ANYTYPE"
            )
         );
      }

      //   private static Type FindViewInterface(object view, object model) {
      //      Type viewType = view.GetType();
      //      Type viewInterface = viewType
      //         .GetInterfaces()
      //         .FirstOrDefault(i =>
      //            i.IsGenericType &&
      //            i.GetGenericTypeDefinition() == typeof(IView<>) && (
      //               model == null ||
      //               i.GetGenericArguments().Single().IsAssignableFrom(model.GetType())
      //            )
      //         );

      //      return viewInterface;
      //   }

      //   private static object CreateView(Type modelType, out Type actualViewInterface) {
      //      object view = null;

      //      Type t = modelType;
      //      do {
      //         actualViewInterface = typeof(IView<>).MakeGenericType(t);
      //         view = ServiceLocator.Current.TryGetInstance(actualViewInterface);
      //         t = t.BaseType;
      //      } while (t != null && view == null);

      //      if (view == null) {
      //         throw new ArgumentException(
      //            ExceptionTexts.CouldNotResolveView.FormatWith(modelType.Name)
      //         );
      //      }

      //      return view;
      //   }

   }
}
