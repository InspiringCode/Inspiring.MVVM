﻿namespace Inspiring.Mvvm.Common {
   public interface ISaveDiscardHandler {
      bool CanSave();
      void Save();

      bool CanDiscard();
      void Discard();
   }
}
