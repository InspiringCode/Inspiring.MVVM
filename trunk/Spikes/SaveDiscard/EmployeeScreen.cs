using System.Diagnostics;
namespace SaveDiscard {

   public sealed class EmployeeScreen : ISaveDiscardScreen {
      public string Name { get; set; }

      public void Save() {
         Debug.WriteLine("Save");
      }

      public void Discard() {
         Debug.WriteLine("Discard");
      }
   }
}
