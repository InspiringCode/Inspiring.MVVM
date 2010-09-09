namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels.Behaviors;

   /// <summary>
   ///   A factory that creates <see cref="VMProperty"/> instances configured 
   ///   with a certain value source. You can get an instance of this interface
   ///   by calling 'VMDescriptorBuilder.For&lt;PersonVM&gt;().CreateDescriptor(c => {
   ///      var p = c.GetPropertyFactory(x => x.Person);
   ///      [...]
   ///   })'.
   /// </summary>
   /// <remarks>
   ///   Note that simple instance properties can only be created with the 
   ///   <see cref="IRootPropertyFactory"/> because it does not make sense on 
   ///   factories that are created for a certain model source object.
   /// </remarks>
   public interface IVMPropertyFactory<TSource> : IHideObjectMembers {
      /// <summary>
      ///   Creates a property that returns und updates the value of a normal
      ///   property of a model object that is wrapped by the view model.
      /// </summary>
      /// <param name="sourcePropertySelector">
      ///   <para>An expression of the form 'x => x.Age' that specifies the 
      ///      property path of the property whose value should be returned and
      ///      updated by the returned 'VMProperty'.</para>
      ///   <para>The first argument of the expression ('x' in the example) is
      ///      the the value of the model property this factory was created for.
      ///      If this factory was created by 'GetPropertyFactory(x => x.Person)'
      ///      the first argument would be the value of the 'Person' property of
      ///      the current 'ViewModel'.</para>
      /// </param>
      VMProperty<T> Mapped<T>(Expression<Func<TSource, T>> sourcePropertySelector);

      /// <summary>
      ///   Creates a property that uses the passed 'getter' and 'setter' 
      ///   delegates to return and update its value.
      /// </summary>
      /// <param name="getter">
      ///   <para>A delegate that is called by the 'VMProperty' to get its value 
      ///      which should perform some logic to get the current value of the
      ///      returned 'VMProperty'. Example: 'x => x.GetFee("Default")'.</para>
      ///   <para>The first argument of the delegate is the value of the model 
      ///      property this factory was created for. If this factory was created 
      ///      by 'GetPropertyFactory(x => x.Person)' the first argument would be 
      ///      the value of the 'Person' property of the current 'ViewModel'.</para>
      /// </param>
      /// <param name="setter">
      ///   <para>A delegate that is called by the 'VMProperty' to update its 
      ///      value which should perform perform some logic to update the current 
      ///      value of the returned 'VMProperty'. Example: 
      ///      '(x, value) => x.UpdateFee("Default", value)'.</para>
      ///   <para>The first argument of the delegate is the value of the model 
      ///      property this factory was created for. If this factory was created 
      ///      by 'GetPropertyFactory(x => x.Person)' the first argument would be 
      ///      the value of the 'Person' property of the current 'ViewModel'. The
      ///      second argument is the new value of the 'VMProperty'.</para>
      /// </param>
      VMProperty<T> Calculated<T>(Func<TSource, T> getter, Action<TSource, T> setter = null);

      // TODO: Document me
      IVMCollectionPropertyFactory<TItem> MappedCollection<TItem>(
         Expression<Func<TSource, IEnumerable<TItem>>> sourceCollectionSelector
      );
   }

   /// <summary>
   ///   A factory that creates <see cref="VMProperty"/> instances configured 
   ///   with a certain value source. You can get an instance of this interface
   ///   by calling 'VMDescriptorBuilder.For&lt;PersonVM&gt;().CreateDescriptor(c => {
   ///      var v = c.GetPropertyFactory();
   ///      [...]
   ///   })'.
   /// </summary>
   public interface IRootVMPropertyFactory<TVM> : IHideObjectMembers {
      /// <summary>
      ///   Creates a property that returns und updates the value of a normal
      ///   property of a model object that is wrapped by the view model.
      /// </summary>
      /// <param name="sourcePropertySelector">
      ///   <para>An expression of the form 'x => x.Person.Age' that specifies 
      ///      the property path of the property whose value should be returned
      ///      and updated by the returned 'VMProperty'.</para>
      ///   <para>The first argument of the expression ('x' in the example) is
      ///      the current 'ViewModel' for which this property is created. This
      ///      means that the specified path has to start with a CLR property 
      ///      ('Person') declared on the view model.</para>
      /// </param>
      VMProperty<T> Mapped<T>(Expression<Func<TVM, T>> sourcePropertySelector);

      /// <summary>
      ///   Creates a property that uses the passed 'getter' and 'setter' 
      ///   delegates to return and update its value.
      /// </summary>
      /// <param name="getter">
      ///   <para>A delegate that is called by the 'VMProperty' to get its value.
      ///      The current 'ViewModel' is passed to the delegate which should
      ///      perform some logic to get the current value of the returned 
      ///      'VMProperty'.</para>
      ///   <para>Example: 'vm => vm.Person.GetFee(vm.CurrentProject)'.</para>
      /// </param>
      /// <param name="setter">
      ///   <para>A delegate that is called by the 'VMProperty' to update its 
      ///      value. The current 'ViewModel' and the new value is passed
      ///      to the delegate which should perform some logic to update the 
      ///      current value of the returned 'VMProperty'.
      ///   </para>
      ///   <para>Example: 
      ///      '(vm, value) => vm.Person.UpdateFee(vm.CurrentProject, value)'.
      ///   </para>
      /// </param>
      VMProperty<T> Calculated<T>(Func<TVM, T> getter, Action<TVM, T> setter = null);

      /// <summary>
      ///   Creates a simple property that uses the view model to store its 
      ///   value. You can think of it as a normal instance property enhanced
      ///   with all the 'VMProperty' features.
      /// </summary>
      VMProperty<TValue> Simple<TValue>();

      // TODO: Document me
      IVMCollectionPropertyFactory<TItem> MappedCollection<TItem>(
         Expression<Func<TVM, IEnumerable<TItem>>> sourceCollectionSelector
      );
   }

   public interface IVMCollectionPropertyFactory<TSourceItem> {
      VMCollectionProperty<TVM> Of<TVM>() where TVM : ICanInitializeFrom<TSourceItem>;
   }
}
