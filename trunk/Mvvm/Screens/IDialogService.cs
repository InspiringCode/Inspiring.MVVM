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
         IScreen parent = null,
         string title = null
      ) where TScreen : ScreenBase;
   }

   public class DialogScreenResult {
      public DialogScreenResult(bool result, object data = null) {
         Result = result;
         Data = data;
      }

      public bool Result { get; set; }
      public object Data { get; set; }
   }
}
