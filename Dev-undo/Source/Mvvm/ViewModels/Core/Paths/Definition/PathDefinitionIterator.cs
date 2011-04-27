﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   internal struct PathDefinitionIterator {
      private readonly PathDefinitionStep[] _steps;
      private int _index;

      public PathDefinitionIterator(PathDefinitionStep[] steps) {
         _steps = steps;
         _index = 0;
      }

      private bool HasStep {
         get { return _index < _steps.Length; }
      }

      private PathDefinitionStep Current {
         get { return _steps[_index]; }
      }

      public PathMatch MatchesNext(PathIterator step) {
         var nextDefinitionStep = HasStep ?
            Current :
            PathTerminationStep.Instance;

         return nextDefinitionStep.Matches(MoveNext(), step);
      }

      private PathDefinitionIterator MoveNext() {
         var next = this;
         next._index++;
         return next;
      }
   }
}