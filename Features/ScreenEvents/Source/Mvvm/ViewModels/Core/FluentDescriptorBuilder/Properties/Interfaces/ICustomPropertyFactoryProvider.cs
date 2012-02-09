namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   public interface ICustomPropertyFactoryProvider<TSourceObject> {
      /// <summary>
      ///   Provides an API to create properties with custom behaviors. This API
      ///   is indended for very ADVANCED scenarios and should ONLY be used if you
      ///   cannot implement your requirements with the standard API or you are
      ///   extending the framework.
      /// </summary>
      ICustomPropertyFactory<TSourceObject> Custom { get; }
   }
}
