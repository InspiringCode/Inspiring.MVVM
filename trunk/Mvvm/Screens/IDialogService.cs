namespace Inspiring.Mvvm.Screens {
   // TODO: Save dialog
   public interface IDialogService {
      bool OpenFile(
         IScreenBase parent,
         out string fileName,
         string filter = null,
         string initialDirectory = null
      );

      DialogScreenResult Open<TScreen>(
         IScreenFactory<TScreen> screen,
         IScreenBase parent = null,
         string title = null
      ) where TScreen : IScreenBase;
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
