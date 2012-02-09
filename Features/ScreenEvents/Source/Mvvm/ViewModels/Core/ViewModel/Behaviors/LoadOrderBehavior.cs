namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Tracing;

   // TODO: Test Refresh order etc.!
   internal sealed class LoadOrderBehavior :
      InitializableBehavior,
      IBehaviorInitializationBehavior,
      IManualUpdateCoordinatorBehavior,
      IRefreshControllerBehavior {

      public IEnumerable<IVMPropertyDescriptor> UpdateFromSourceProperties {
         get;
         set;
      }

      public IEnumerable<IVMPropertyDescriptor> UpdateSourceProperties {
         get;
         set;
      }

      public void UpdateFromSource(IBehaviorContext context) {
         RequireInitialized();
         foreach (IVMPropertyDescriptor property in UpdateFromSourceProperties) {
            UpdateFromSource(context, property);
         }

         this.UpdateFromSourceNext(context);
      }

      public void UpdateFromSource(IBehaviorContext context, IVMPropertyDescriptor property) {
         RequireInitialized();
         throw new NotImplementedException();

         this.UpdateFromSourceNext(context, property);
      }

      public void UpdateSource(IBehaviorContext context) {
         RequireInitialized();
         foreach (IVMPropertyDescriptor property in UpdateSourceProperties) {
            UpdateSource(context, property);
         }

         this.UpdateSourceNext(context);
      }

      public void UpdateSource(IBehaviorContext context, IVMPropertyDescriptor property) {
         RequireInitialized();
         throw new NotImplementedException();

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

      public void Refresh(IBehaviorContext context, bool executeRefreshDependencies) {
         RefreshTrace.BeginRefresh(context.VM);

         RequireInitialized();

         foreach (IVMPropertyDescriptor property in UpdateFromSourceProperties) {
            // We call ALL behaviors and not only succeeding ones, because a client
            // may want to register a behavior that gets called BEFORE this behavior.
            context
               .VM
               .Descriptor
               .Behaviors
               .ViewModelRefreshNext(context, property, new RefreshOptions(executeRefreshDependencies));
         }

         this.ViewModelRefreshNext(context, executeRefreshDependencies);

         RefreshTrace.EndLastRefresh();
      }

      public void Refresh(IBehaviorContext context, IVMPropertyDescriptor property, RefreshOptions options) {
         RefreshTrace.BeginRefresh(property);

         RequireInitialized();
         property.Behaviors.RefreshNext(context, options);

         this.ViewModelRefreshNext(context, property, options);

         RefreshTrace.EndLastRefresh();
      }
   }
}
