namespace ValidationTest {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Windows;
   using System.Windows.Controls;
   using System.Windows.Media;
   using System.Windows.Media.Media3D;

   public static class View {
      public static readonly DependencyProperty ChildErrorMonitoringIsEnabledProperty = DependencyProperty.RegisterAttached(
         "ChildErrorMonitoringIsEnabled",
         typeof(bool),
         typeof(View),
         new PropertyMetadata(HandleChildErrorMonitoringIsEnabledChanged)
      );

      public static readonly DependencyProperty HasChildErrorsProperty = DependencyProperty.RegisterAttached(
         "HasChildErrors",
         typeof(bool),
         typeof(View)
      );

      private static readonly DependencyProperty InvalidElementsProperty = DependencyProperty.RegisterAttached(
         "InvalidElements",
         typeof(InvalidElementCollection),
         typeof(View)
      );

      // ACCESSORS

      public static bool GetChildErrorMonitoringIsEnabled(DependencyObject obj) {
         return (bool)obj.GetValue(ChildErrorMonitoringIsEnabledProperty);
      }

      public static void SetChildErrorMonitoringIsEnabled(DependencyObject obj, bool value) {
         obj.SetValue(ChildErrorMonitoringIsEnabledProperty, value);
      }

      public static bool GetHasChildErrors(DependencyObject obj) {
         return (bool)obj.GetValue(HasChildErrorsProperty);
      }

      private static void SetHasChildErrors(DependencyObject obj, bool value) {
         obj.SetValue(HasChildErrorsProperty, value);
      }

      private static InvalidElementCollection GetInvalidElements(DependencyObject obj) {
         return (InvalidElementCollection)obj.GetValue(InvalidElementsProperty);
      }

      private static void SetInvalidElements(DependencyObject obj, InvalidElementCollection value) {
         obj.SetValue(InvalidElementsProperty, value);
      }

      // METHODS

      private static void HandleChildErrorMonitoringIsEnabledChanged(
         DependencyObject sender,
         DependencyPropertyChangedEventArgs e
      ) {
         if ((bool)e.NewValue) {
            Validation.AddErrorHandler(sender, HandleErrorEvent);
         } else {
            Validation.RemoveErrorHandler(sender, HandleErrorEvent);
         }
      }

      private static void HandleErrorEvent(object sender, ValidationErrorEventArgs e) {
         var parent = (DependencyObject)sender;
         var invalidElement = (DependencyObject)e.OriginalSource;

         var childErrors = EnsureCollection(parent);

         switch (e.Action) {
            case ValidationErrorEventAction.Added:
               childErrors.Add(invalidElement);
               break;
            case ValidationErrorEventAction.Removed:
               childErrors.Remove(invalidElement);
               break;
            default:
               throw new NotSupportedException();
         }

         SetHasChildErrors(parent, childErrors.HasErrors);
      }

      private static InvalidElementCollection EnsureCollection(DependencyObject parent) {
         var errors = GetInvalidElements(parent);

         if (errors == null) {
            errors = new InvalidElementCollection(parent);
            SetInvalidElements(parent, errors);
         }

         return errors;
      }

      // INVALID ELEMENT COLLECTION

      private class InvalidElementCollection {
         private DependencyObject _commonParent;
         private List<DependencyObject> _invalidElements;

         public InvalidElementCollection(DependencyObject commonParent) {
            _commonParent = commonParent;
            _invalidElements = new List<DependencyObject>();
         }

         public bool HasErrors { get; private set; }

         public void Add(DependencyObject element) {
            _invalidElements.Add(element);
            Update();
         }

         public void Remove(DependencyObject element) {
            _invalidElements.Remove(element);
            Update();
         }

         private void Update() {
            RemoveAbandonedElements();
            HasErrors = _invalidElements.Any(HasError);
         }

         private void RemoveAbandonedElements() {
            for (int i = _invalidElements.Count - 1; i >= 0; i--) {
               DependencyObject element = _invalidElements[i];

               bool containedInVisualTree = IsDescendantOf(_commonParent, element);
               bool hasError = HasError(element);

               if (!containedInVisualTree && !hasError) {
                  _invalidElements.RemoveAt(i);
               }
            }
         }

         private bool HasError(DependencyObject element) {
            return Validation.GetHasError(element);
         }

         private static bool IsDescendantOf(
            DependencyObject parent,
            DependencyObject descendant
         ) {
            while ((descendant = GetParent(descendant)) != null) {
               if (descendant == parent) {
                  return true;
               }
            }
            return false;
         }

         private static DependencyObject GetParent(DependencyObject element) {
            return IsVisalElement(element) ?
               VisualTreeHelper.GetParent(element) :
               LogicalTreeHelper.GetParent(element);
         }

         private static bool IsVisalElement(DependencyObject element) {
            return element is Visual || element is Visual3D;
         }
      }
   }
}