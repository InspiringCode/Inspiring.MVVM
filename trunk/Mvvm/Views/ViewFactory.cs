namespace Inspiring.Mvvm.Views {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using System.Reflection;

   internal static class ViewFactory {
      public static bool TryInitializeView(object view, object withModel) {
         if (view == null) {
            return false;
         }

         Type[] interfaces = view.GetType().GetInterfaces();

         if (withModel != null) {
            // Get the most specialized IView<T> implementation
            foreach (Type viewInterface in GetPossibleViewInterfaces(withModel.GetType())) {
               if (interfaces.Contains(viewInterface)) {
                  SetModelProperty(view, withModel, viewInterface);
                  return true;
               }
            }
         } else {
            // Set all Model properties to null and return true if at least one 
            // IView<T> interface is implemented.
            Type[] implementedViewInterfaces = interfaces
               .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IView<>))
               .ToArray();

            if (implementedViewInterfaces.Length > 0) {
               foreach (Type viewInterface in implementedViewInterfaces) {
                  SetModelProperty(view, null, viewInterface);
               }

               return true;
            }
         }

         return false;
      }

      public static object CreateView(object forModel) {
         Contract.Requires<ArgumentNullException>(forModel != null);

         Type modelType = forModel.GetType();
         object view = null;

         foreach (Type viewInterface in GetPossibleViewInterfaces(modelType)) {
            if ((view = ServiceLocator.Current.TryGetInstance(viewInterface)) != null) {
               bool initializationWasSuccessful = TryInitializeView(view, forModel);
               Contract.Assert(initializationWasSuccessful);
               return view;
            }
         }

         throw new ArgumentException(
            ExceptionTexts.CouldNotResolveView.FormatWith(modelType.Name)
         );
      }

      private static IEnumerable<Type> GetPossibleViewInterfaces(Type forModelType) {
         for (Type t = forModelType; t != null; t = t.BaseType) {
            yield return typeof(IView<>).MakeGenericType(t);
         }
      }

      private static void SetModelProperty(object ofView, object toModel, Type viewInterface) {
         PropertyInfo viewInterfaceModelProperty = viewInterface.GetProperty("Model");
         viewInterfaceModelProperty.SetValue(ofView, toModel, null);
      }
   }
}
