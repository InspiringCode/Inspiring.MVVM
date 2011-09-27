namespace Inspiring.Mvvm.ViewModels {

   public sealed class MultiSelectionWithSourceVM<TSourceObject, TItemSource, TItemVM> :
      MultiSelectionVM<TItemSource, TItemVM>,
      IHasReadonlySourceObject<TSourceObject>,
      IHasSourceObject<TSourceObject>
      where TItemVM : IViewModel, IHasSourceObject<TItemSource> {

      /// <param name="descriptor">
      ///   Use <see cref="CreateDescriptor"/> to create one.
      /// </param>
      internal MultiSelectionWithSourceVM(
         MultiSelectionVMDescriptor<TItemSource, SelectableItemVM<TItemSource, TItemVM>> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
      }

      TSourceObject IHasReadonlySourceObject<TSourceObject>.Source {
         get { return SourceObject; }
      }

      TSourceObject IHasSourceObject<TSourceObject>.Source {
         get { return SourceObject; }
         set { SourceObject = value; }
      }

      /// <summary>
      ///   Gets the object that holds the source items. This references the
      ///   view model that holds the <see cref="MultiSelectionWithSourceVM"/> (the parent
      ///   VM is simply forwarded with this property).
      /// </summary>
      internal TSourceObject SourceObject { get; set; }

      /// <inheritdoc />
      public void InitializeFrom(TSourceObject source) {
         SourceObject = source;
      }

      internal void LoadSelectedItems() {
         Load(Descriptor.SelectedItems);
      }
   }

   public sealed class MultiSelectionWithSourceVM<TSourceObject, TItemSource> :
      MultiSelectionVM<TItemSource>,
      IHasSourceObject<TSourceObject> {

      public MultiSelectionWithSourceVM(
         MultiSelectionVMDescriptor<TItemSource> descriptor,
         IServiceLocator serviceLocator
      )
         : base(descriptor, serviceLocator) {
      }

      /// <summary>
      ///   Gets the object that holds the source items. This references the
      ///   view model that holds the <see cref="MultiSelectionWithSourceVM"/> (the parent
      ///   VM is simply forwarded with this property).
      /// </summary>
      public TSourceObject Source { get; set; }

      /// <inheritdoc />
      public void InitializeFrom(TSourceObject source) {
         Source = source;
      }
   }
}
