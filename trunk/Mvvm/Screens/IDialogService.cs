namespace Inspiring.Mvvm.Screens {
   // TODO: Save dialog
   public interface IDialogService {
      bool OpenFile(
         IScreen parent,
         out string fileName,
         string filter = null,
         string initialDirectory = null
      );

      DialogScreenResult Open<TScreen>(
         IScreenFactory<TScreen> screen,
         IScreen parent,
         string title = null
      ) where TScreen : IScreen;
   }

   public class DialogScreenResult {
      public bool Result { get; set; }
      public object Data { get; set; }
   }
}
