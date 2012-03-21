namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

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

      public bool IsEmpty {
         get { return _steps.Length == 0; }
      }

      public PathStep this[int index] {
         get { return _steps[index]; }
      }

      public PathStep Last {
         get { return _steps[_steps.Length - 1]; }
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

      public Path Append(IViewModel viewModel) {
         return Append(new PathStep(viewModel));
      }

      public Path Append(IVMCollection collection) {
         return Append(new PathStep(collection));
      }

      public Path Append(IVMPropertyDescriptor property) {
         return Append(new PathStep(property));
      }

      public PathIterator GetIterator() {
         return new PathIterator(_steps);
      }

      public override string ToString() {
         return String.Format(
            "[{0}]",
            String.Join(", ", _steps)
         );
      }

      private Path Prepend(PathStep step) {
         var s = new PathStep[Length + 1];
         s[0] = step;
         _steps.CopyTo(s, 1);

         return new Path(s);
      }

      // TODO: Use array utils!
      private Path Append(PathStep step) {
         var s = new PathStep[Length + 1];
         _steps.CopyTo(s, 0);
         s[Length] = step;

         return new Path(s);
      }
   }
}
