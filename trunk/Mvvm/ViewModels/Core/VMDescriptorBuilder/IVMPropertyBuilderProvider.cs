namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels.Fluent;

   /// <summary>
   ///   A provider that returns <see cref="IVMPropertyBuilder"/> objects. Different
   ///   property factories can be created for different source objects.
   /// </summary>
   public interface IVMPropertyBuilderProvider<TVM> : IHideObjectMembers where TVM : IViewModel {
      /// <summary>
      ///   <para>Returns a <see cref="IVMPropertyBuilder"/> which creates <see
      ///      cref="VMProperty"/> objects.</para>
      ///   <para>Mapped properties created with the returned factory have to 
      ///      specify the property path relative to the VM. For delegated 
      ///      properties the VM instance is passed to their getter/setter 
      ///      delegates.</para>
      /// </summary>
      IVMPropertyBuilder<TVM> GetPropertyBuilder();

      /// <summary>
      ///   Returns a <see cref="IVMPropertyBuilder"/> which creates <see
      ///   cref="VMProperty"/> objects.
      /// </summary>
      /// <param name="sourceObjectSelector">
      ///   An expression of the form 'x => x.Person' that returns an object
      ///   referenced by the VM. Mapped properties created with the returned 
      ///   factory have to specify the property path relative to the selected
      ///   objects. For delegated properties the selected object is passed to
      ///   their getter/setter delegates.
      /// </param>
      /// <remarks>
      ///   This shortens the property path for mapped properties (instead
      ///   of 'MapsTo(x => x.Person.Age)' you can use 'MapsTo(x => x.Age)' 
      ///   and instead of 'DelegatesTo(x => x.Person.CalculateReward(...))' 
      ///   you can use 'DelegatesTo(x => x.CalculateReward(...))'.
      /// </remarks>
      IVMPropertyBuilder<TSource> GetPropertyBuilder<TSource>(
         Expression<Func<TVM, TSource>> sourceObjectSelector
      );
   }
}
