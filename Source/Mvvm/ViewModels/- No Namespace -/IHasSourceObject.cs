﻿namespace Inspiring.Mvvm.ViewModels {

   public interface IHasReadonlySourceObject<out TSourceObject> : IViewModel {
      TSourceObject Source { get; }
   }

   public interface IHasSourceObject<TSourceObject> : IHasReadonlySourceObject<TSourceObject> {
      new TSourceObject Source { get; set; }
   }
}
