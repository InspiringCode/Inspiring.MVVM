namespace Inspiring.Mvvm.ViewModels.Core {

   internal struct PathMatch {
      private readonly bool _success;
      private readonly int _length;

      private PathMatch(bool success, int length) {
         _success = success;
         _length = length;
      }

      public bool Success {
         get { return _success; }
      }

      public int Length {
         get { return _length; }
      }

      public static PathMatch Succeed(int length) {
         return new PathMatch(true, length);
      }

      public static PathMatch Fail(int length = 0) {
         return new PathMatch(false, length);
      }

      public static PathMatch Combine(PathMatch first, PathMatch second) {
         return new PathMatch(
            first.Success && second.Success,
            first.Length + second.Length
         );
      }
   }
}
