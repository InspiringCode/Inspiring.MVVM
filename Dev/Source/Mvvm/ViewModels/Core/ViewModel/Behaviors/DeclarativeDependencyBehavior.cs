﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class DeclarativeDependencyBehavior :
      Behavior,
      IChangeHandlerBehavior {

      private List<DeclarativeDependency> _dependencies = new List<DeclarativeDependency>();

      public void HandleChange(IBehaviorContext context, ChangeArgs args) {
         throw new System.NotImplementedException();
      }

      public void AddDependency(DeclarativeDependency dependency) {
         _dependencies.Add(dependency);
      }

      internal ReadOnlyCollection<DeclarativeDependency> Dependencies {
         get { return _dependencies.AsReadOnly(); }
      }
   }
}