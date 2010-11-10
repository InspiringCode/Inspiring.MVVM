namespace Inspiring.Mvvm.ViewModels {
   using System;

   public interface IVMProperty {
      string PropertyName { get; }
      Type PropertyType { get; }
   }
}
