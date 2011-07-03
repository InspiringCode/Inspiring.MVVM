namespace Inspiring.Mvvm.Common {
   using System;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   A base class for objects that may be sealed. Once an object is sealed 
   ///   it is immutable and cannot be modified.
   /// </summary>
   /// <remarks>
   ///   Call <see cref="Seal"/> in every method that should seal the object 
   ///   and call <see cref="RequireNotSealed"/> in every method that modifies
   ///   the state of the object.
   /// </remarks>
   public abstract class SealableObject {
      /// <summary>
      ///   Gets whether the object is sealed. Once an object is sealed it is
      ///   immutable and cannot be modified.
      /// </summary>
      public bool IsSealed { get; private set; }

      /// <summary>
      ///   Seals the object. Once an object is sealed it is immutable and cannot
      ///   be modified.
      /// </summary>
      public void Seal() {
         Contract.Ensures(IsSealed);
         IsSealed = true;
      }

      /// <summary>
      ///   Thorws an <see cref="InvalidOperationException"/> if the object is
      ///   sealed.
      /// </summary>
      protected void RequireNotSealed() {
         Contract.Requires<InvalidOperationException>(
            !IsSealed,
            ExceptionTexts.ObjectIsSealed
         );
      }
   }
}
