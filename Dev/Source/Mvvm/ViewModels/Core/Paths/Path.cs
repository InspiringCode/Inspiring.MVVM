namespace Inspiring.Mvvm.ViewModels.Core {

   // TODO: Is it OK for me to be public?
   public sealed class Path {
      public static readonly Path Empty = new Path(new PathStep[0]);

      private readonly PathStep[] _steps;

      private Path(PathStep[] steps) {
         _steps = steps;
      }

      public int Length {
         get { return _steps.Length; }
      }

      public PathStep this[int index] {
         get { return _steps[index]; }
      }

      public Path Prepend(IViewModel viewModel) {
         return Prepend(new PathStep(viewModel));
      }

      public Path Prepend(IVMCollection collection) {
         return Prepend(new PathStep(collection));
      }

      public Path Prepend(IVMPropertyDescriptor property) {
         return Prepend(new PathStep(property));
      }

      public PathIterator GetIterator() {
         return new PathIterator(_steps);
      }

      private Path Prepend(PathStep step) {
         var s = new PathStep[Length + 1];
         s[0] = step;
         _steps.CopyTo(s, 1);

         return new Path(s);
      }
   }
}
