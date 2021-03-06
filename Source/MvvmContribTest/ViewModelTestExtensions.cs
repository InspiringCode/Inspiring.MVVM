﻿namespace Inspiring.MvvmContribTest {
   using Inspiring.Mvvm.ViewModels;

   internal static class ViewModelTestExtensions {
      public static void SetDisplayValue(
         this IViewModel viewModel,
         IVMPropertyDescriptor property,
         object value
      ) {
         viewModel.Kernel.SetDisplayValue(property, value);
      }
   }
}
