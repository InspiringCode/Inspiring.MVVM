namespace Inspiring.Mvvm.ViewModels.Core {

   internal struct PathIterator {
      private readonly PathStep[] _steps;
      private int _index;

      internal PathIterator(PathStep[] steps) {
         _steps = steps;
         _index = 0;
      }

      public bool IsDone {
         get { return _index >= _steps.Length; }
      }

      public PathStepType Type {
         get { return _steps[_index].Type; }
      }

      public IViewModel ViewModel {
         get { return _steps[_index].ViewModel; }
      }

      public IVMCollection Collection {
         get { return _steps[_index].Collection; }
      }

      public IVMPropertyDescriptor Property {
         get { return _steps[_index].Property; }
      }

      public void Next() {
         _index++;
      }
   }
}
