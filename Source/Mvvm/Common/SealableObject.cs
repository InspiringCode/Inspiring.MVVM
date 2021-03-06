﻿namespace Inspiring.Mvvm.Common {
   using System;

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
         IsSealed = true;
      }

      /// <summary>
      ///   Throws an <see cref="InvalidOperationException"/> if the object is
      ///   sealed.
      /// </summary>
      protected void RequireNotSealed() {
         if (IsSealed) {
            throw new InvalidOperationException(ExceptionTexts.ObjectIsSealed);
         }
      }
   }
}
