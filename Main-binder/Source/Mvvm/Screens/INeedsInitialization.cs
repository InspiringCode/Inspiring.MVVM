namespace Inspiring.Mvvm.Screens {
   public interface INeedsInitialization {
      void Initialize();
   }

   public interface INeedsInitialization<TSubject> {
      void Initialize(TSubject subject);
   }
}
