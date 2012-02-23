namespace Inspiring.Mvvm.Screens {
   using System.Collections.Generic;

   public interface IScreenBase {
      IScreenBase Parent { get; set; }
      ICollection<object> Children { get; }
   }
}
