namespace Inspiring.Mvvm.Screens {
   /// <summary>
   ///   See <see cref="ScreenCreationBehavior"/>.
   /// </summary>
   public interface ILocatableScreen<in TSubject> {
      /// <summary>
      ///   Returns true, if the screen that implements this interface presents the
      ///   data specified by the parameter <paramref name="subject"/>.
      /// </summary>
      bool PresentsSubject(TSubject subject);
   }
}
