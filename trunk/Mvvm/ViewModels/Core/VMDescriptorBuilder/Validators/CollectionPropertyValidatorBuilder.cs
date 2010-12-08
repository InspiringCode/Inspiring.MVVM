﻿namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core.Builder;

   public sealed class CollectionPropertyValidatorBuilder<TItemValue> : ConfigurationProvider {
      internal CollectionPropertyValidatorBuilder(VMDescriptorConfiguration configuration)
         : base(configuration) {
      }

      /// <summary>
      ///   Defines a custom validator that is executed when the selected property
      ///   is about to change on any item of the selected collection.
      /// </summary>
      /// <remarks>
      ///   The validator is also executed when a revalidation is performed, or
      ///   the VM is added to/removed from a collection.
      /// </remarks>
      /// <param name="validator">
      ///   An action that performs the validation. The first argument is the
      ///   new value of the changin property, the second argument contains the
      ///   property values of all collection items.
      /// </param>
      public void Custom(Action<TItemValue, IEnumerable<TItemValue>, ValidationArgs> validator) {
         throw new NotImplementedException();
      }
   }
}
