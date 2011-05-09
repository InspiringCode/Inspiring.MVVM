namespace Inspiring.Mvvm.ViewModels.Core.Validation {
   using System;

   public interface IValidationTarget {
      ValidationStep Step { get; }
   }

   public interface IInstanceValidationTarget : IValidationTarget {
      IViewModel VM { get; }
      IVMPropertyDescriptor Property { get; }

      ValidationRequest CreateRequest();
   }

   public interface ICollectionValidationTarget : IValidationTarget {
      IVMCollection Collection { get; }
      IVMPropertyDescriptor Property { get; }

      ValidationRequest CreateRequest();
   }

   public interface IValidationErrorTarget : IValidationTarget {
      IVMCollection Collection { get; }
      IViewModel VM { get; }
      IVMPropertyDescriptor Property { get; }
   }

   internal sealed class ValidationTarget :
      IInstanceValidationTarget,
      ICollectionValidationTarget,
      IValidationErrorTarget {

      public ValidationStep Step { get; private set; }

      public IViewModel VM { get; private set; }

      public IVMCollection Collection { get; private set; }

      public IVMPropertyDescriptor Property { get; private set; }

      ValidationRequest IInstanceValidationTarget.CreateRequest() {
         throw new NotImplementedException();
      }

      ValidationRequest ICollectionValidationTarget.CreateRequest() {
         throw new NotImplementedException();
      }
   }
}