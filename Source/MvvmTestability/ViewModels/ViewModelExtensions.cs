﻿namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Windows.Input;
   using Inspiring.Mvvm.Testability;

   public static class ViewModelExtensions {
      public static void ExecuteCommand<TDescriptor>(
         this IViewModel<TDescriptor> viewModel,
         Func<TDescriptor, IVMPropertyDescriptor<ICommand>> commandPropertySelector
      ) where TDescriptor : IVMDescriptor {
         if (viewModel == null) {
            throw new ArgumentNullException("viewModel");
         }

         if (commandPropertySelector == null) {
            throw new ArgumentNullException("commandPropertySelector");
         }

         if (!CanExecuteCommand(viewModel, commandPropertySelector)) {
            TestFrameworkAdapter.Current.Fail("Das Kommando kann nicht ausgeführt werden.");
         }

         var property = commandPropertySelector((TDescriptor)viewModel.Descriptor);
         var command = viewModel.Kernel.GetValue(property);
         command.Execute(null);
      }

      public static bool CanExecuteCommand<TDescriptor>(
         this IViewModel<TDescriptor> viewModel,
         Func<TDescriptor, IVMPropertyDescriptor<ICommand>> commandPropertySelector
      ) where TDescriptor : IVMDescriptor {
         if (viewModel == null) {
            throw new ArgumentNullException("viewModel");
         }

         if (commandPropertySelector == null) {
            throw new ArgumentNullException("commandPropertySelector");
         }

         var property = commandPropertySelector((TDescriptor)viewModel.Descriptor);
         var command = viewModel.Kernel.GetValue(property);
         return command.CanExecute(null);
      }

      public static T GetValue<TDescriptor, T>(
         this IViewModel<TDescriptor> viewModel,
         Func<TDescriptor, IVMPropertyDescriptor<T>> propertySelector
      ) where TDescriptor : IVMDescriptor {
         if (viewModel == null) {
            throw new ArgumentNullException("viewModel");
         }

         if (propertySelector == null) {
            throw new ArgumentNullException("propertySelector");
         }

         var property = propertySelector((TDescriptor)viewModel.Descriptor);
         return viewModel.Kernel.GetValue(property);
      }

      public static IVMPropertyDescriptor<T> GetProperty<TDescriptor, T>(
          this IViewModel<TDescriptor> viewModel,
          Func<TDescriptor, IVMPropertyDescriptor<T>> propertySelector
       ) where TDescriptor : IVMDescriptor {
         if (viewModel == null) {
            throw new ArgumentNullException("viewModel");
         }

         if (propertySelector == null) {
            throw new ArgumentNullException("propertySelector");
         }

         var property = propertySelector((TDescriptor)viewModel.Descriptor);
         return property;
      }


      public static void SetValue<TDescriptor, T>(
         this IViewModel<TDescriptor> viewModel,
         Func<TDescriptor, IVMPropertyDescriptor<T>> propertySelector,
         T value
      ) where TDescriptor : IVMDescriptor {
         if (viewModel == null) {
            throw new ArgumentNullException("viewModel");
         }

         if (propertySelector == null) {
            throw new ArgumentNullException("propertySelector");
         }

         var property = propertySelector((TDescriptor)viewModel.Descriptor);
         viewModel.Kernel.SetValue(property, value);
      }


      public static void Load<TDescriptor>(
         this IViewModel<TDescriptor> viewModel,
         Func<TDescriptor, IVMPropertyDescriptor> propertySelector
      ) where TDescriptor : IVMDescriptor {
         if (viewModel == null) {
            throw new ArgumentNullException("viewModel");
         }

         if (propertySelector == null) {
            throw new ArgumentNullException("propertySelector");
         }

         var property = propertySelector((TDescriptor)viewModel.Descriptor);
         viewModel.Kernel.Load(property);
      }

      public static void Refresh<TDescriptor>(
         this IViewModel<TDescriptor> viewModel,
         bool executeRefreshDependencies = false
      ) where TDescriptor : IVMDescriptor {
         if (viewModel == null) {
            throw new ArgumentNullException("viewModel");
         }

         viewModel.Kernel.Refresh(executeRefreshDependencies);
      }

      public static void Refresh<TDescriptor>(
         this IViewModel<TDescriptor> viewModel,
         Func<TDescriptor, IVMPropertyDescriptor> propertySelector,
         bool executeRefreshDependencies = false
      ) where TDescriptor : IVMDescriptor {
         if (viewModel == null) {
            throw new ArgumentNullException("viewModel");
         }

         if (propertySelector == null) {
            throw new ArgumentNullException("propertySelector");
         }

         var property = propertySelector((TDescriptor)viewModel.Descriptor);
         viewModel.Kernel.Refresh(property, executeRefreshDependencies);
      }

      public static void RefreshContainer<TDescriptor>(
         this IViewModel<TDescriptor> viewModel,
         Func<TDescriptor, IVMPropertyDescriptor> propertySelector,
         bool executeRefreshDependencies = false
      ) where TDescriptor : IVMDescriptor {
         if (viewModel == null) {
            throw new ArgumentNullException("viewModel");
         }

         if (propertySelector == null) {
            throw new ArgumentNullException("propertySelector");
         }

         var property = propertySelector((TDescriptor)viewModel.Descriptor);
         viewModel.Kernel.RefreshContainer(property, executeRefreshDependencies);
      }

      // TODO
      //public static void Refresh<TDescriptor>(
      //   this IViewModel<TDescriptor> viewModel,
      //   Action<IPathDefinitionBuilder<TDescriptor>> refreshSelector,
      //   bool executeRefreshDependencies = false
      //) where TDescriptor : IVMDescriptor {
      //   if (viewModel == null) {
      //      throw new ArgumentNullException("viewModel");
      //   }

      //   if (refreshSelector == null) {
      //      throw new ArgumentNullException("propertySelector");
      //   }
      //}

      public static void Revalidate(this IViewModel viewModel, ValidationScope scope = ValidationScope.Self) {
         viewModel.Kernel.Revalidate(scope);
      }
   }
}
