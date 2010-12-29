namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   internal sealed class ManualUpdateCoordinatorBehavior :
      InitializableBehavior,
      IBehaviorInitializationBehavior,
      IManualUpdateCoordinatorBehavior {

      public IEnumerable<IVMProperty> UpdateFromSourceProperties {
         get;
         set;
      }

      public IEnumerable<IVMProperty> UpdateSourceProperties {
         get;
         set;
      }

      public void UpdateFromSource(IBehaviorContext context) {
         RequireInitialized();
         foreach (IVMProperty property in UpdateFromSourceProperties) {
            UpdateFromSource(context, property);
         }

         this.UpdateFromSourceNext(context);
      }

      public void UpdateFromSource(IBehaviorContext context, IVMProperty property) {
         RequireInitialized();
         property.Behaviors.UpdatePropertyFromSourceNext(context);

         this.UpdateFromSourceNext(context, property);
      }

      public void UpdateSource(IBehaviorContext context) {
         RequireInitialized();
         foreach (IVMProperty property in UpdateSourceProperties) {
            UpdateSource(context, property);
         }

         this.UpdateSourceNext(context);
      }

      public void UpdateSource(IBehaviorContext context, IVMProperty property) {
         RequireInitialized();
         property.Behaviors.UpdatePropertySourceNext(context);

         IManualUpdateCoordinatorBehavior next;
         if (TryGetBehavior(out next)) {
            next.UpdateSource(context, property);
         }

         this.UpdateSourceNext(context, property);
      }

      public void Initialize(BehaviorInitializationContext context) {
         if (UpdateFromSourceProperties == null) {
            UpdateFromSourceProperties = context.Descriptor.Properties;
         }

         if (UpdateSourceProperties == null) {
            UpdateSourceProperties = context.Descriptor.Properties;
         }

         SetInitialized();
         this.InitializeNext(context);
      }
   }
}
