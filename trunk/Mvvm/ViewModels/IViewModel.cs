namespace Inspiring.Mvvm.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;

   public interface IViewModel {
      VMKernel Kernel { get; }
      object GetValue(IVMProperty property, ValueStage stage = ValueStage.PreValidation);
      void SetValue(IVMProperty property, object value);
      IBehaviorContext GetContext();

      [Obsolete]
      bool IsValid(bool validateChildren);

      [Obsolete]
      void Revalidate();

      [Obsolete]
      event EventHandler<ValidationEventArgs> Validating;

      [Obsolete]
      event EventHandler<ValidationEventArgs> Validated;

      [Obsolete]
      void InvokeValidate(IViewModel changedVM, VMPropertyBase changedProperty);

      [Obsolete]
      IViewModel Parent { get; set; }
   }
}
