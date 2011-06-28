namespace Inspiring.Mvvm.Common.Behaviors {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;

   internal abstract class BehaviorFactoryConfiguration<T> :
      IBehaviorFactoryConfiguration
      where T : class, IBehaviorFactoryProvider {

      IBehaviorFactory IBehaviorFactoryConfiguration.GetFactory(
         IBehaviorFactoryProvider factoryProvider
      ) {
         T concreteFactoryProvider = factoryProvider as T;

         if (concreteFactoryProvider == null) {
            var message = ECommon
               .WrongBehaviorFactoryProviderType
               .FormatWith(
                  TypeService.GetFriendlyName(typeof(T)),
                  TypeService.GetFriendlyTypeName(factoryProvider)
               );

            throw new ArgumentException(message);
         }

         return GetFactory(concreteFactoryProvider);
      }

      protected abstract IBehaviorFactory GetFactory(T factoryProvider);
   }
}
