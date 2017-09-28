namespace Inspiring.Mvvm.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;


   public interface IVMPropertyDescriptor<out T> : IVMPropertyDescriptor {
   }

   public interface IVMPropertyDescriptor {
      /// <summary>
      ///   Gets the name of the VM Property. This is the same as the name
      ///   of the <see cref="VMDescriptor"/> CLR property that defines the
      ///   VM property.
      /// </summary>
      string PropertyName { get; }

      /// <summary>
      ///   The type of the property value.
      /// </summary>
      Type PropertyType { get; }

      /// <summary>
      ///   Gets the value that this property has on the given view model
      ///   (specified by the <paramref name="context"/> parameter).
      /// </summary>
      object GetValue(IBehaviorContext context);

      //object GetDisplayValue(IBehaviorContext context);

      ///// <summary>
      /////   Sets the value of this property on the given view model (specified
      /////   by the <paramref name="context"/> parameter).
      ///// </summary>
      //void SetValue(IBehaviorContext context, object value);

      /// <summary>
      ///   Gets the head of the chain of property behaviors.
      /// </summary>
      BehaviorChain Behaviors { get; set; }

      void Initialize(string propertyName);

      //object GetValueAsObject(IBehaviorContext context);
   }
}
