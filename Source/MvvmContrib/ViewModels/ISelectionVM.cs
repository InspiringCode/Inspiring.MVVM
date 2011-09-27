namespace Inspiring.Mvvm.ViewModels {
   using System.Collections;

   internal interface ISelectionVM {
      IEnumerable AllSourceItems { get; }
      IEnumerable SelectedSourceItems { get; }
   }
}
