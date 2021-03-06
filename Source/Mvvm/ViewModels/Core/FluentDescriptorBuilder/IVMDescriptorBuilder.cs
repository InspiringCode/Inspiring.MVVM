﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Specialized;
   using System.ComponentModel;

   /// <summary>
   ///   Fluent interface. See <see cref="VMDescriptorBuilder"/>.
   /// </summary>
   public interface IVMDescriptorBuilder<TDescriptor, TVM>
      where TDescriptor : VMDescriptor, new()
      where TVM : IViewModel {

      /// <summary>
      ///   Configures or overrides the VM properties and their mappings (required).
      /// </summary>
      /// <param name="propertyConfigurator">
      ///   A delegate that should assign <see cref="IVMPropertyDescriptor"/> instances to
      ///   all properties of the <see cref="VMDescriptor"/> passed to the 
      ///   delegate (or override some of them). Use the passed in <see 
      ///   cref="IVMPropertyBuilderProvider{TVM}"/> to get a <see 
      ///   cref="IVMPropertyBuilder{TSourceObject}"/> with which <see cref="IVMPropertyDescriptor"/>
      ///   objects can be created and configured.
      /// </param>
      IVMDescriptorBuilderWithProperties<TDescriptor, TVM> WithProperties(Action<TDescriptor, IVMPropertyBuilderProvider<TVM>> propertyConfigurator);
   }

   /// <summary>
   ///   Fluent interface. See <see cref="VMDescriptorBuilder"/>.
   /// </summary>
   public interface IVMDescriptorBuilderWithProperties<TDescriptor, TVM> :
      IVMDescriptorBuilder<TDescriptor, TVM>
      where TDescriptor : VMDescriptor, new()
      where TVM : IViewModel {

      /// <summary>
      ///   Configures dependencies between properties to allow correct change
      ///   notification (optional).
      /// </summary>
      /// <param name="dependencyConfigurator">
      ///   A delegate that should use the passed in <see cref="IVMDependencyBuilder{TRootVM, TRootDescriptor}"/>
      ///   to setup dependencies between VM properties.
      /// </param>
      /// <remarks>
      ///   <para>A property 'A' depends on 'B' if a change of 'B' also results 
      ///      in a change of 'A'. Example: If 'Name' is calculated by 
      ///      'FirstName + " "  + LastName' the property 'Name' depends on the 
      ///      properties 'FirstName' and 'LastName'.</para>
      ///   <para>This feature allows us to avoid implementing <see 
      ///      cref="INotifyPropertyChanged"/> and <see cref="INotifyCollectionChanged"/>
      ///      in our domain models because the VM takes care of this concern.</para>
      /// </remarks>
      IVMDescriptorBuilderWithProperties<TDescriptor, TVM> WithDependencies(
         Action<IVMDependencyBuilder<TVM, TDescriptor>> dependencyConfigurator
      );

      /// <summary>
      ///   Configures how the VM should be validated (optional).
      /// </summary>
      /// <param name="validatorConfigurator">
      ///   A delegate that should define validators using the <see 
      ///   cref="RootValidatorBuilder{TOwner, TTarget, TDescriptor}"/> passed to the delegate.
      /// </param>
      IVMDescriptorBuilderWithProperties<TDescriptor, TVM> WithValidators(
         Action<RootValidatorBuilder<TVM, TVM, TDescriptor>> validatorConfigurator
      );

      /// <summary>
      ///   Configures additional "behaviors" that modify how the properties
      ///   of the view model behave (optional).
      /// </summary>
      /// <param name="behaviorConfigurator">
      ///   A delegate that should enable or configure behaviors using the
      ///   <see cref="IVMBehaviorBuilder{TVM, TDescriptor}"/> passed to the delegate.
      /// </param>
      IVMDescriptorBuilderWithProperties<TDescriptor, TVM> WithBehaviors(
         Action<IVMBehaviorBuilder<TVM, TDescriptor>> behaviorConfigurator
      );

      /// <summary>
      ///   Configures additional "behaviors" that modify how the view model 
      ///   behaves (optional).
      /// </summary>
      /// <param name="behaviorConfigurator">
      ///   A delegate that should enable or configure behaviors using the
      ///   <see cref="ViewModelBehaviorBuilder{TVM, TDescriptor}"/> passed to the delegate.
      /// </param>
      IVMDescriptorBuilderWithProperties<TDescriptor, TVM> WithViewModelBehaviors(
         Action<ViewModelBehaviorBuilder<TVM, TDescriptor>> behaviorConfigurator
      );

      /// <summary>
      ///   Builds and returns the fully configured <see cref="VMDescriptor"/>. 
      ///   Usually the returned descriptor is assigned to a public static 
      ///   readonly field (named ClassDescriptor) and passed to the constructor
      ///   of the <see cref="ViewModel"/> base class.
      /// </summary>  
      TDescriptor Build();
   }
}
