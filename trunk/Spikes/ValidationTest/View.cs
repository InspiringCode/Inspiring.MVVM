namespace ValidationTest {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Windows;
   using System.Windows.Controls;
   using System.Windows.Media;
   using System.Windows.Media.Media3D;

   public static class View {
      public static readonly DependencyProperty MirrorChildValidationErrorsProperty = DependencyProperty.RegisterAttached(
         "MirrorChildValidationErrors",
         typeof(bool),
         typeof(View),
         new PropertyMetadata(HandleMirrorChildValidationErrorsChanged)
      );

      public static readonly DependencyProperty HasChildErrorsProperty = DependencyProperty.RegisterAttached(
         "HasChildErrors",
         typeof(bool),
         typeof(View)
      );

      private static readonly DependencyProperty ChildErrorsProperty = DependencyProperty.RegisterAttached(
         "ChildErrors",
         typeof(ErrorSet),
         typeof(View)
      );

      public static bool GetMirrorChildValidationErrors(DependencyObject obj) {
         return (bool)obj.GetValue(MirrorChildValidationErrorsProperty);
      }

      public static void SetMirrorChildValidationErrors(DependencyObject obj, bool value) {
         obj.SetValue(MirrorChildValidationErrorsProperty, value);
      }

      public static bool GetHasChildErrors(DependencyObject obj) {
         return (bool)obj.GetValue(HasChildErrorsProperty);
      }

      private static void SetHasChildErrors(DependencyObject obj, bool value) {
         obj.SetValue(HasChildErrorsProperty, value);
      }

      private static ErrorSet GetChildErrors(DependencyObject obj) {
         return (ErrorSet)obj.GetValue(ChildErrorsProperty);
      }

      private static void SetChildErrors(DependencyObject obj, ErrorSet value) {
         obj.SetValue(ChildErrorsProperty, value);
      }

      private static ErrorSet EnsureChildErrors(DependencyObject obj, DependencyObject commonParent) {
         var errors = GetChildErrors(obj);

         if (errors == null) {
            errors = new ErrorSet(commonParent);
            SetChildErrors(obj, errors);
         }

         return errors;
      }

      private static void HandleMirrorChildValidationErrorsChanged(
         DependencyObject sender,
         DependencyPropertyChangedEventArgs e
      ) {
         EventHandler<ValidationErrorEventArgs> errorHandler = (s, args) => {
            var handlerSender = (DependencyObject)s;
            var childErrors = EnsureChildErrors(handlerSender, (DependencyObject)((TabItem)handlerSender).Content);

            switch (args.Action) {
               case ValidationErrorEventAction.Added:
                  childErrors.Add((DependencyObject)args.OriginalSource);
                  break;
               case ValidationErrorEventAction.Removed:
                  childErrors.Remove((DependencyObject)args.OriginalSource);
                  break;
               default:
                  throw new NotSupportedException();
            }

            SetHasChildErrors(handlerSender, childErrors.HasErrors);
         };

         Validation.AddErrorHandler(sender, errorHandler);
      }

      private class ErrorSet {
         private const int NotFound = -1;
         private List<ValidationError> _errors = new List<ValidationError>();
         private List<Object> _invalidDataItems = new List<object>();

         //public bool HasErrors {
         //   //get { return _errors.Any(); }
         //   get { return _invalidDataItems.Any(); }
         //}

         //public void Add(ValidationError error) {

         //   var exp = (BindingExpression)error.BindingInError;
         //   if (exp.DataItem != null && !_invalidDataItems.Contains(exp.DataItem)) {
         //      _invalidDataItems.Add(exp.DataItem);
         //   }

         //   //if (GetErrorIndex(error) == NotFound) {
         //   //   _errors.Add(error);
         //   //}

         //   Cleanup();
         //}

         //public void Remove(ValidationError error) {


         //   var exp = (BindingExpression)error.BindingInError;
         //   if (exp.DataItem != null && _invalidDataItems.Contains(exp.DataItem)) {
         //      _invalidDataItems.Remove(exp.DataItem);
         //   }

         //   //int index = GetErrorIndex(error);

         //   //if (index != NotFound) {
         //   //   _errors.RemoveAt(index);
         //   //}

         //   Cleanup();
         //}

         //private void Cleanup() {
         //   bool allNull = _errors.Select(x => x.BindingInError).OfType<BindingExpression>().All(x => x.DataItem == null);
         //   Debug.WriteLine("{0}: {1}", DateTime.Now.Ticks, allNull);

         //   //for (int i = _errors.Count - 1; i >= 0; i--) {
         //   //   var expression = _errors[i].BindingInError as BindingExpression;

         //   //   if (expression == null) {
         //   //      continue;
         //   //   }

         //   //   if (expression.DataItem == null) {
         //   //      _errors.RemoveAt(i);
         //   //   }
         //   //}
         //}

         //private int GetErrorIndex(ValidationError error) {
         //   return _errors.FindIndex(x => ErrorsAreEqual(error, x));
         //}

         //private static bool ErrorsAreEqual(ValidationError first, ValidationError second) {
         //   return
         //      first.BindingInError == second.BindingInError &&
         //      first.ErrorContent == second.ErrorContent &&
         //      first.RuleInError == second.RuleInError;
         //}

         private DependencyObject _commonParent;
         private List<DependencyObject> _possiblyInvalidElements = new List<DependencyObject>();

         public bool HasErrors { get; private set; }

         public ErrorSet(DependencyObject commonParent) {
            _commonParent = commonParent;
         }

         public void Add(DependencyObject element) {
            _possiblyInvalidElements.Add(element);
            Update();
         }

         public void Remove(DependencyObject element) {
            _possiblyInvalidElements.Remove(element);
            Update();
         }

         private void Update() {
            HasErrors = false;

            for (int i = _possiblyInvalidElements.Count - 1; i >= 0; i--) {
               DependencyObject element = _possiblyInvalidElements[i];

               if (!LogicalParentContainsVisualDescendant(_commonParent, element) && !Validation.GetHasError(element)) {
                  _possiblyInvalidElements.RemoveAt(i);
                  continue;
               }

               if (Validation.GetHasError(element)) {
                  HasErrors = true;
                  break;
               }
            }

            ;
         }

         private bool LogicalParentContainsVisualDescendant(DependencyObject parent, DependencyObject visualDescendant) {
            var possibleVisualAncestors = GetVisualChildren(parent).ToList();

            foreach (DependencyObject ancestor in GetVisualAncestors(visualDescendant)) {
               if (possibleVisualAncestors.Contains(ancestor)) {
                  return true;
               }
            }

            return false;
         }

         private static IEnumerable<DependencyObject> GetVisualChildren(DependencyObject parent) {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++) {
               yield return VisualTreeHelper.GetChild(parent, i);
            }
         }


         private static IEnumerable<DependencyObject> GetVisualAncestors(DependencyObject element) {
            // The 'element' may not be in the visual tree in which case the
            // 'VisualTreeHelper' would not work. If so, walk up the logical
            // tree until we have reached the visual tree.
            while (element != null && !IsVisalElement(element)) {
               element = LogicalTreeHelper.GetParent(element);
            }

            if (element != null) {
               element = VisualTreeHelper.GetParent(element);

               while (element != null) {
                  element = VisualTreeHelper.GetParent(element);
                  yield return element;
               }
            }
         }

         private static bool IsVisalElement(DependencyObject element) {
            return element is Visual || element is Visual3D;
         }
      }
   }
}
