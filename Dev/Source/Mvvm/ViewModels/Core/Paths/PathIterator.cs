namespace Inspiring.Mvvm.ViewModels.Core {

   public struct PathIterator {
      private readonly PathStep[] _steps;
      private int _index;

      internal PathIterator(PathStep[] steps) {
         _steps = steps;
         _index = 0;
      }

      public bool HasStep {
         get { return _index < _steps.Length; }
      }

      public bool IsViewModel {
         get { return _steps[_index].Type == PathStepType.ViewModel; }
      }

      public bool IsCollection {
         get { return _steps[_index].Type == PathStepType.Collection; }
      }

      public bool IsProperty {
         get { return _steps[_index].Type == PathStepType.Property; }
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

      public void MoveNext() {
         _index++;
      }

      public int GetIndex() {
         return _index;
      }
   }
}
