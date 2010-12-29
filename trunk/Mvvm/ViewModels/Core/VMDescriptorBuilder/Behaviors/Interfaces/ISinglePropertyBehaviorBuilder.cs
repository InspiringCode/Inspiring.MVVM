namespace Inspiring.Mvvm.ViewModels.Core {
   using System;

   /// <summary>
   ///   Implements a fluent interface to configure property behaviors. Returned
   ///   by <see cref="IPropertyBehaviorBuilder"/>
   /// </summary>
   /// <typeparam name="TValue">
   ///   The type of the property (such as String or EmployeeVM).
   /// </typeparam>
   public interface ISinglePropertyBehaviorBuilder<TVM, TValue> {
      /// <summary>
      ///   Enables the behavior with the given key.
      /// </summary>
      /// <param name="behaviorInstance">
      ///   Specifies or overrides the default behavior instance provided by the
      ///   <see cref="BehaviorChainTemplate"/>. This parameter is required if
      ///   the template has defined the behavior with the <see 
      ///   cref="DefaultBehaviorState.DisabledWithoutFactory"/> option.
      /// </param>
      ISinglePropertyBehaviorBuilder<TVM, TValue> Enable(
         BehaviorKey key,
         IBehavior behaviorInstance = null
      );

      /// <summary>
      ///   Configures the collection behaviors of a collection property.
      /// </summary>
      ISinglePropertyBehaviorBuilder<TVM, TValue> CollectionBehaviors { get; }

      /// <summary>
      ///   Calls the '<paramref name="configurationAction"/>' with the behavior
      ///   specified by '<paramref name="key"/>' and enables it.
      /// </summary>
      /// <typeparam name="TBehavior">
      ///   The type of the behavior to configure. This may be the concrete type
      ///   of the behavior or a base type/interface.
      /// </typeparam>
      ISinglePropertyBehaviorBuilder<TVM, TValue> Configure<TBehavior>(
         BehaviorKey key,
         Action<TBehavior> configurationAction
      ) where TBehavior : IBehavior;

      // TODO!
      /// <summary>
      ///  Warning: This will change soon.
      /// </summary>
      ISinglePropertyBehaviorBuilder<TVM, TValue> AddChangeHandler(
         Action<TVM, ChangeArgs, InstancePath> changeHandler
      );
   }
}
