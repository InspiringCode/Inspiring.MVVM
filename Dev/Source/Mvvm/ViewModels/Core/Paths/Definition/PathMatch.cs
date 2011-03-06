namespace Inspiring.Mvvm.ViewModels.Core {

   internal struct PathMatch {
      private readonly bool _success;
      private readonly int _length;

      private PathMatch(bool success, int length) {
         _success = success;
         _length = length;
      }

      public static PathMatch Success(int length) {
         return new PathMatch(true, length);
      }

      public static PathMatch Failure() {
         return new PathMatch(false, 0);
      }
   }
}
