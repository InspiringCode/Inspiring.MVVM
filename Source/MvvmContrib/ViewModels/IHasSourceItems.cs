namespace Inspiring.Mvvm.ViewModels {

   internal interface IHasSourceItems<TItemSource> {
      SourceItemCollections<TItemSource> SourceItems { get; }
   }
}
