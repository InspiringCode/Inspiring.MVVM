namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   Creates 'VMProperty' factories.
   /// </summary>
   public interface IVMPropertyFactoryProvider<TVM> : IHideObjectMembers {
      /// <summary>
      ///   Returns a factory that creates 'VMProperty' instances. Assign them
      ///   to a 'VMProperty' property of a 'VMDescriptor'.
      /// </summary>
      IRootVMPropertyFactory<TVM> GetPropertyFactory();

      /// <summary>
      ///   Returns a factory that creates 'VMProperty' instances. Assign them
      ///   to a 'VMProperty' property of a 'VMDescriptor'.
      /// </summary>
      /// <param name="sourceObjectSelector">
      ///   <para>An expression of the form 'x => x.Person' that specifies the 
      ///      property that serves as the path root for mapped properties and 
      ///      whose value is passed to the getter/setter delegate of a 
      ///      calculated property.</para>
      ///   <para>This shortens the property path for mapped properties (instead
      ///      of 'Mapped(x => x.Person.Age)' you can use 'Mapped(x => x.Age)' 
      ///      and instead of 'Calculated(x => x.Person.CalculateReward(...))' 
      ///      you can use 'Calculated(x => x.CalculateReward(...))'.</para>
      /// </param>
      IVMPropertyFactory<TSource> GetPropertyFactory<TSource>(Expression<Func<TVM, TSource>> sourceObjectSelector);
   }
}
