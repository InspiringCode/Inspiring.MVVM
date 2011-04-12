namespace Inspiring.Mvvm.ViewModels {
   /// <summary>
   ///   <para>Provides a base class for VMs that have a source object.</para>
   /// </summary>
   /// <remarks>
   ///   There are two ways a VM may be initialized:
   ///   <list type="number">
   ///      <item>By the framework: If a VM is contained in a VM collection or
   ///         in a child VM property, the source is automatically set and the 
   ///         child items are validated by the framework when a collection
   ///         is populated or a child VM is created.</item>
   ///      <item>Manually: If the VM is the root of a VM hierarchy (the VM is
   ///         not contained by a collection or a VM property) the developer is
   ///         responsible for setting its source and for validating it 
   ///         initially.</item>
   ///   </list>
   ///   <para>To initialize a VM manually, call <see cref="InitializeFrom"/>. If
   ///      you want to add code that is executed only on manual initialization,
   ///      override <see cref="InitializeFrom"/>. If you want to add code that is
   ///      executed for manual and automatic initialization, override <see 
   ///      cref="SetSource"/>.
   /// </remarks>
   public abstract class DefaultViewModelWithSourceBase<TDescriptor, TSourceObject> :
      ViewModel<TDescriptor>,
      IHasSourceObject<TSourceObject>
      where TDescriptor : IVMDescriptor {

      private TSourceObject _source;

      public DefaultViewModelWithSourceBase(IServiceLocator serviceLocator = null)
         : base(serviceLocator) {
      }

      public DefaultViewModelWithSourceBase(TDescriptor descriptor, IServiceLocator serviceLocator = null)
         : base(descriptor, serviceLocator) {
      }

      /// <inheritdoc />
      TSourceObject IHasSourceObject<TSourceObject>.Source {
         get { return _source; }
         set { SetSource(value); }
      }

      /// <summary>
      ///   Gets source object.
      /// </summary>
      public TSourceObject Source {
         get { return _source; }
      }

      /// <summary>
      ///   Call this method to initialize a VM manually. See remarks of <see 
      ///   cref="DefaultViewModelWithSourceBase"/>.
      /// </summary>
      /// <remarks>
      ///   This method calls <see cref="SetSource"/> and <see cref="Revalidate"/>.
      /// </remarks>
      public virtual void InitializeFrom(TSourceObject source) {
         SetSource(source);
         Revalidate();
      }

      /// <summary>
      ///   See remarks of <see cref="DefaultViewModelWithSourceBase"/>.
      /// </summary>
      protected virtual void SetSource(TSourceObject source) {
         _source = source;
      }
   }
}
