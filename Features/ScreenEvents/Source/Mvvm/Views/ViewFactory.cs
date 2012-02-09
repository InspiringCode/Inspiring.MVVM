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

         Type[] interfaces = view
            .GetType()
            .GetInterfaces();

         Type[] supportedModelTypes = interfaces
            .Where(i => IsIViewInterface(i))
            .Select(i => GetModelTypeOfIViewInterface(i))
            .ToArray();

         if (withModel != null) {
            // Get the most specialized IView<T> implementation
            foreach (Type modelBaseType in GetModelBaseTypes(withModel.GetType())) {
               Type supportedModelType = supportedModelTypes
                  .FirstOrDefault(t => t.IsAssignableFrom(modelBaseType));

               if (supportedModelType != null) {
                  Type implementedViewInterface = typeof(IView<>).MakeGenericType(supportedModelType);
                  SetModelProperty(view, withModel, implementedViewInterface);
                  return true;
               }
            }
         } else {
            // Set all Model properties to null and return true if at least one 
            // IView<T> interface is implemented.
            Type[] implementedViewInterfaces = interfaces
               .Where(i => IsIViewInterface(i))
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

         //foreach (Type itf in forModelType.GetInterfaces()) {
         //   yield return typeof(IView<>).MakeGenericType(itf);
         //}
      }

      private static IEnumerable<Type> GetModelBaseTypes(Type modelType) {
         for (Type t = modelType; t != null; t = t.BaseType) {
            yield return t;
         }

         foreach (Type itf in modelType.GetInterfaces()) {
            yield return itf;
         }
      }

      private static void SetModelProperty(object ofView, object toModel, Type viewInterface) {
         PropertyInfo viewInterfaceModelProperty = viewInterface.GetProperty("Model");
         viewInterfaceModelProperty.SetValue(ofView, toModel, null);
      }

      private static bool IsIViewInterface(Type itf) {
         return
            itf.IsGenericType &&
            itf.GetGenericTypeDefinition() == typeof(IView<>);
      }

      private static Type GetModelTypeOfIViewInterface(Type viewInterface) {
         return viewInterface
            .GetGenericArguments()
            .First();
      }
   }
}
