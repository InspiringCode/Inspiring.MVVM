namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   A template that defines the ordering of <see cref="IBehavior"/> objects
   ///   in a <see cref="BehaviorChain"/>. It also defines how the behavior are
   ///   created and whether the are enabled by default.
   /// </summary>
   /// <remarks>
   ///   A template does not create a <see cref="BehaviorChain"/> directly. Create
   ///   <see cref="BehaviorChainConfiguration"/> by calling <see 
   ///   cref="CreateConfiguration"/> first to create a concrete chain.
   /// </remarks>
   public sealed class BehaviorChainTemplate {
      private readonly List<BehaviorChainItemTemplate> _itemTemplates = new List<BehaviorChainItemTemplate>();

      /// <summary>
      ///   Appends a disabled behavior chain item to the end of the behavior 
      ///   template list of this behavior chain template.
      /// </summary>
      public void Append(BehaviorKey disabledBehaviorKey) {
         Contract.Requires<ArgumentNullException>(disabledBehaviorKey != null);

         _itemTemplates.Add(new BehaviorChainItemTemplate(disabledBehaviorKey));
      }

      /// <summary>
      ///   Adds a behavior chain item template to the end of the behavior template
      ///   list of this behavior chain template.
      /// </summary>
      /// <param name="factory">
      ///   The <see cref="IBehaviorFactory"/> that is used to create a concreate
      ///   <see cref="IBehavior"/> instance when a <see cref="BehaviorChainConfiguration"/>
      ///   is created from this template.
      /// </param>
      /// <param name="isDisabledByDefault">
      ///   If true, the behavior is not enabled in a <see cref="BehaviorChainConfiguration"/> 
      ///   created from this template. This means it will not be included in the finally 
      ///   created <see cref="BehaviorChain"/> unless <see 
      ///   cref="BehaviorChainConfiguration.Enable"/> is called.
      /// </param>
      public void Append(BehaviorKey key, IBehaviorFactory factory, bool isDisabledByDefault = true) {
         Contract.Requires<ArgumentNullException>(key != null);
         Contract.Requires<ArgumentNullException>(factory != null);

         _itemTemplates.Add(
            new BehaviorChainItemTemplate(key) {
               Factory = factory,
               IsDisabledByDefault = isDisabledByDefault
            }
         );
      }

      /// <summary>
      ///   Creates a new <see cref="BehaviorChainConfiguration"/> from this
      ///   template.
      /// </summary>
      /// <typeparam name="T">
      ///   The type the should be used for <see cref="IBehaviorFactory.Create"/>
      ///   when the behaviors are instantiated. Pass the value type of the 
      ///   VM property (e.g. String) for property behavior chains and the type
      ///   of the View Model (e.g. EmployeeVM) for view model behavior chains.
      /// </typeparam>
      internal BehaviorChainConfiguration CreateConfiguration<T>() {
         Contract.Ensures(Contract.Result<BehaviorChainConfiguration>() != null);

         var config = new BehaviorChainConfiguration();

         foreach (BehaviorChainItemTemplate itemTemplate in _itemTemplates) {
            if (itemTemplate.Factory != null) {
               IBehavior instance = itemTemplate.Factory.Create<T>();
               config.Append(itemTemplate.Key, instance, itemTemplate.IsDisabledByDefault);
            } else {
               config.Append(itemTemplate.Key);
            }
         }

         return config;
      }

      private class BehaviorChainItemTemplate {
         public IBehaviorFactory Factory { get; set; }
         public BehaviorKey Key { get; set; }
         public bool IsDisabledByDefault { get; set; }

         public BehaviorChainItemTemplate(BehaviorKey key) {
            Key = key;
            IsDisabledByDefault = true;
            Factory = null;
         }

         [ContractInvariantMethod]
         private void ObjectInvariant() {
            Contract.Invariant(!IsDisabledByDefault ? Factory != null : true);
         }

         public override string ToString() {
            return Key.ToString();
         }
      }
   }
}
