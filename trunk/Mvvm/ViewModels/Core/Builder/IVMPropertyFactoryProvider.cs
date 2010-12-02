namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   A provider that creates <see cref="IVMPropertyFactory"/> objects. Different
   ///   property factories can be created for different source objects.
   /// </summary>
   public interface IVMPropertyFactoryProvider<TVM> : IHideObjectMembers {
      /// <summary>
      ///   <para>Returns a <see cref="IVMPropertyFactory"/> which creates <see
      ///      cref="VMProperty"/> objects.</para>
      ///   <para>Mapped properties created with the returned factory have to 
      ///      specify the property path relative to the VM. For calculated 
      ///      properties the VM instance is passed to their getter/setter 
      ///      delegates.</para>
      /// </summary>
      IRootVMPropertyFactory<TVM> GetPropertyFactory();

      /// <summary>
      ///   Returns a <see cref="IVMPropertyFactory"/> which creates <see
      ///   cref="VMProperty"/> objects.
      /// </summary>
      /// <param name="sourceObjectSelector">
      ///   An expression of the form 'x => x.Person' that returns an object
      ///   referenced by the VM. Mapped properties created with the returned 
      ///   factory have to specify the property path relative to the selected
      ///   objects. For calculated properties the selected object is passed to
      ///   their getter/setter delegates.
      /// </param>
      /// <remarks>
      ///   This shortens the property path for mapped properties (instead
      ///   of 'Mapped(x => x.Person.Age)' you can use 'Mapped(x => x.Age)' 
      ///   and instead of 'Calculated(x => x.Person.CalculateReward(...))' 
      ///   you can use 'Calculated(x => x.CalculateReward(...))'.
      /// </remarks>
      IVMPropertyFactory<TVM, TSource> GetPropertyFactory<TSource>(Expression<Func<TVM, TSource>> sourceObjectSelector);
   }
}
