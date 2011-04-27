namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.Common;

   internal sealed class CollectionResultTarget {
      public CollectionResultTarget(
         ValidationStep step,
         IVMCollection collection,
         IVMPropertyDescriptor property = null
      ) {
         Step = step;
         Collection = collection;
         Property = property;
      }

      public ValidationStep Step { get; private set; }
      public IVMCollection Collection { get; private set; }
      public IVMPropertyDescriptor Property { get; private set; }

      public ValidationRequest CreateValidationRequest() {
         var requestPath = Path.Empty
            .Append(Collection.OwnerVM)
            .Append(Collection);

         if (Property != null) {
            requestPath = requestPath.Append(Property);
         }

         return new ValidationRequest(Step, requestPath);
      }

      public override bool Equals(object obj) {
         var other = obj as CollectionResultTarget;

         return
            other != null &&
            other.Step == Step &&
            other.Collection == Collection &&
            other.Property == Property;
      }

      public override int GetHashCode() {
         return HashCodeService.CalculateHashCode(
            this,
            Step,
            Collection,
            Property
         );
      }

      public override string ToString() {
         if (Property != null) {
            return String.Format(
               "{{Property target:  Step = {0}, Collection = {1}, Property = {2}}}",
               Step,
               Collection,
               Property
            );
         } else {
            return String.Format(
               "{{View model target:  Step = {0}, Collection = {1}}}",
               Step,
               Collection
            );
         }
      }
   }
}
