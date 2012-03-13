namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   
   public abstract class HierarchicalEvent<TTarget, TArgs> :
      HierarchicalEventBase<TTarget, TArgs>
      where TArgs : HierarchicalEventArgs<TTarget> {
   }
}
