namespace Inspiring.Mvvm.Screens {
   /// <summary>
   ///   Specifies the behavior of the <see cref="ScreenConductor.OpenScreen{TScreen}"/>
   ///   method.
   /// </summary>
   public enum ScreenCreationBehavior {
      /// <summary>
      ///   An arbitrary number of instances may be opened at the same time.
      /// </summary>
      MultipleInstances,

      /// <summary>
      ///   If a screen of the same type is already open, no new screen is created
      ///   and the existing screen is activated.
      /// </summary>
      SingleInstance,

      /// <summary>
      ///   For every open screen the <see cref="ILocatableScreen{TSubject}.PresentsSubject"/>
      ///   method is called. The first open screen that returns true is activated. If 
      ///   no screen returns true, a new instance is created and initialized with the
      ///   subject.
      /// </summary>
      UseScreenLocation
   }
}
