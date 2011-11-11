﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   internal sealed class RefreshAction : DependencyAction {
      private readonly PathDefinition _targetPath;
      private readonly IList<IPropertySelector> _targetProperties;

      public RefreshAction(
         PathDefinition targetPath,
         IList<IPropertySelector> targetProperties
      ) {
         _targetPath = targetPath;
         _targetProperties = targetProperties;
      }

      internal PathDefinition TargetPath { get { return _targetPath; } }

      internal IList<IPropertySelector> TargetProperties {
         get { return _targetProperties; }
      }

      public override void Execute(IViewModel ownerVM, ChangeArgs args) {
         if (TargetPath.IsEmpty) {
            RefreshProperties(ownerVM);
         } else {
            var viewModels = TargetPath.GetDescendants(ownerVM);

            foreach (var viewModel in viewModels) {
               if (_targetProperties.Count > 0) {
                  RefreshProperties(viewModel);
               } else {
                  viewModel.Kernel.Refresh();
               }
            }
         }
      }

      private void RefreshProperties(IViewModel ownerVM) {
         foreach (var propertySelector in _targetProperties) {
            var property = propertySelector.GetProperty(ownerVM.Descriptor);
            ownerVM.Kernel.Refresh(property);
         }
      }
   }
}