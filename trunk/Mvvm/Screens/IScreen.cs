namespace Inspiring.Mvvm.Screens {
   public interface IScreen {
      //string Title { get; }
      IScreen Parent { get; set; }

      void Initialize();
      void Activate();
      void Deactivate();
      bool RequestClose();
      void Close();
   }

   public interface IScreen<TSubject> : IScreen {
      void Initialize(TSubject subject);
   }
}
