namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   // TODO: Split this up (LoadOrder / UpdateController)!
   public interface IManualUpdateCoordinatorBehavior : IBehavior {
      IEnumerable<IVMPropertyDescriptor> UpdateFromSourceProperties { get; set; }
      IEnumerable<IVMPropertyDescriptor> UpdateSourceProperties { get; set; }

      void UpdateFromSource(IBehaviorContext context);
      void UpdateFromSource(IBehaviorContext context, IVMPropertyDescriptor property);

      void UpdateSource(IBehaviorContext context);
      void UpdateSource(IBehaviorContext context, IVMPropertyDescriptor property);
   }
}
