namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   public interface IManualUpdateCoordinatorBehavior : IBehavior {
      IEnumerable<IVMProperty> UpdateFromSourceProperties { get; set; }
      IEnumerable<IVMProperty> UpdateSourceProperties { get; set; }

      void UpdateFromSource(IBehaviorContext context);
      void UpdateFromSource(IBehaviorContext context, IVMProperty property);

      void UpdateSource(IBehaviorContext context);
      void UpdateSource(IBehaviorContext context, IVMProperty property);
   }
}
