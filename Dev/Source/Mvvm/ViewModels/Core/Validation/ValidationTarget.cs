namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   public interface IValidationTarget {
      ValidationStep Step { get; }
   }

   public interface IInstanceValidationTarget : IValidationTarget {
      IViewModel VM { get; }
      IVMPropertyDescriptor Property { get; }

      ValidationRequest CreateValidationRequest();
   }

   public interface ICollectionValidationTarget : IValidationTarget {
      IVMCollection Collection { get; }
      IVMPropertyDescriptor Property { get; }

      ValidationRequest CreateValidationRequest();
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

      private ValidationTarget(ValidationStep step) {
         Step = step;
      }

      public static IInstanceValidationTarget ForInstance(
         ValidationStep step,
         IViewModel vm,
         IVMPropertyDescriptor property = null
      ) {
         Contract.Requires(vm != null);
         return new ValidationTarget(step) { VM = vm, Property = property };
      }

      public static ICollectionValidationTarget ForCollection(
         ValidationStep step,
         IVMCollection collection,
         IVMPropertyDescriptor property = null
      ) {
         Contract.Requires(collection != null);
         return new ValidationTarget(step) { Collection = collection, Property = property };
      }

      public static IValidationErrorTarget ForError(
         ValidationStep step,
         IViewModel vm,
         IVMCollection collection = null,
         IVMPropertyDescriptor property = null
      ) {
         Contract.Requires(vm != null);

         return new ValidationTarget(step) {
            VM = vm,
            Collection = collection,
            Property = property
         };
      }

      public ValidationStep Step { get; private set; }

      public IViewModel VM { get; private set; }

      public IVMCollection Collection { get; private set; }

      public IVMPropertyDescriptor Property { get; private set; }

      ValidationRequest IInstanceValidationTarget.CreateValidationRequest() {
         var requestPath = Path.Empty
            .Append(VM);

         if (Property != null) {
            requestPath = requestPath.Append(Property);
         }

         return new ValidationRequest(Step, requestPath);
      }

      ValidationRequest ICollectionValidationTarget.CreateValidationRequest() {
         var requestPath = Path.Empty
            .Append(Collection.OwnerVM)
            .Append(Collection);

         if (Property != null) {
            requestPath = requestPath.Append(Property);
         }

         return new ValidationRequest(Step, requestPath);
      }

      public override bool Equals(object obj) {
         var other = obj as ValidationTarget;

         return
            other != null &&
            other.Step == Step &&
            other.VM == VM &&
            other.Collection == Collection &&
            other.Property == Property;
      }

      public override int GetHashCode() {
         return HashCodeService.CalculateHashCode(
            this,
            Step,
            VM,
            Collection,
            Property
         );
      }

      public override string ToString() {
         return String.Format(
            "{{ValidationTarget: Step = {0}, VM = {1}, Collection = {2}, Property = {3}}}",
            Step,
            ToStringOrNullLiteral(Collection),
            ToStringOrNullLiteral(VM),
            ToStringOrNullLiteral(Property)
         );
      }

      // TODO: Move this to common class?
      private static string ToStringOrNullLiteral(object value) {
         return value != null ?
            value.ToString() :
            "<NULL>";
      }
   }
}