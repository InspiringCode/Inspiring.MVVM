using System.ComponentModel;
using Inspiring.Mvvm.Common;
using Inspiring.Mvvm.ViewModels.Core;
namespace Inspiring.Mvvm.ViewModels.Fluent {

   /// <summary>
   ///   Creates various <see cref="VMProperty"/> objects that are either mapped
   ///   or calculated.
   /// </summary>
   /// <typeparam name="TVM">
   ///   The type of the VM for which properties are created.
   /// </typeparam>
   /// <typeparam name="TSourceValue">
   ///   The type of the source value as selected by the <see 
   ///   cref="IVMPropertyFactory.Mapped"/> or <see 
   ///   cref="IVMPropertyFactory.Calculated"/> method.
   /// </typeparam>
   /// <remarks>
   ///   Create an extension method on this interface if you want to support
   ///   mapped/calcualted properties for you own property type.
   /// </remarks>
   public interface IVMPropertyFactoryWithSource<TSourceValue> :
      IHideObjectMembers,
      IConfigurationProvider {

      /// <summary>
      ///   Creates a simple <see cref="VMProperty"/>.
      /// </summary>
      VMProperty<TSourceValue> Property();

      /// <summary>
      ///   <para>Creates a <see cref="VMProperty"/> that holds a child view model.
      ///      The child VM is resolved via the service locator of the parent VM 
      ///      and is initialized properly (for example its Parent is set).</para>
      ///   <para><typeparamref name="TChildVM"/> specifies the type of the child
      ///      VM (for example PersonVM). A new instance of <typeparamref name="TChildVM"/>
      ///      is created using the service locator of the parent VM and  <see 
      ///      cref="ICanInitializeFrom.InitializeFrom"/> is called with the source 
      ///      object returned by the mapped or calculated source.</para>
      /// </summary>
      /// <typeparam name="TChildVM">
      ///   The type of the child VM (for example PersonVM). A new instance of 
      ///   <typeparamref name="TChildVM"/> is created using the service locator 
      ///   of the parent VM and  <see cref="ICanInitializeFrom.InitializeFrom"/> 
      ///   is called with the source object returned by the mapped or calculated 
      ///   source.
      /// </typeparam>
      VMProperty<TChildVM> VM<TChildVM>() where TChildVM : IViewModel, ICanInitializeFrom<TSourceValue>;

      // TODO: Comment me
      [EditorBrowsable(EditorBrowsableState.Never)]
      IValueAccessorBehavior<TSourceValue> GetSourceAccessor();
   }
}
