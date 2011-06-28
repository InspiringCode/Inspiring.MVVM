namespace Inspiring.Mvvm.Testability {
   using System;
   using System.Collections;
   using System.Diagnostics.Contracts;

   /// <summary>
   ///   Helper class for implementing <see cref="Object.GetHashCode"/> in a 
   ///   consistent and optimal way.
   /// </summary>
   /// <remarks>
   ///   The method is based on code from Sharp-Architecture.
   /// </remarks>
   internal static class HashCodeService {
      /// <summary>
      ///   To improve the uniqueness of hash codes, a multiplier is used. According 
      ///   to the book 'Data Structures and Algorithms in Java' from Michael T. Goodrich 
      ///   and Roberto Tamassia, 31, 33, 37, 39 and 41 give the least number of duplicate
      ///   values. More information can be found here:
      ///   http://computinglife.wordpress.com/2008/11/20/why-do-hash-functions-use-prime-numbers
      /// </summary>
      private const int HashMultiplier = 31;

      /// <summary>
      ///   Calculates the hash code of the object 'obj' using its 'propertyValues'.
      /// </summary>
      /// <param name="obj">
      ///   The object for which the hash code should be calculated.
      /// </param>
      /// <param name="propertyValues">
      ///   <para>The value of the properties which should influence the hash code.
      ///      These should be the same property values that are compared in the 
      ///      <see cref="Object.Equals"/> method.</para>
      ///   <para>If one of your properties is a collection and you want to base
      ///      the hash code on all items of the collection, you can pass the 
      ///      result of <see cref="CalculateCollectionHashCode"/> as one of the
      ///      property values.</para>
      ///  </param>
      public static int CalculateHashCode(object obj, params object[] propertyValues) {
         Contract.Requires(obj != null);
         Contract.Requires(propertyValues != null);
         Contract.Requires(propertyValues.Length > 0);

         unchecked {
            // If two objects of different types have the same property values, they
            // would return the same hash code. Include the hashcode of the type to
            // avoid this scenario.
            int objectTypeHashCode = obj.GetType().GetHashCode();

            return
               (objectTypeHashCode * HashMultiplier) ^
               CalculateCollectionHashCode(propertyValues);
         }
      }

      /// <summary>
      ///   Calculates the hash code of a collection. The resulting hash code is
      ///   based solely on the hash codes of its items. The type of the collection
      ///   does not matter.
      /// </summary>
      public static int CalculateCollectionHashCode(IEnumerable collection) {
         Contract.Requires(collection != null);

         unchecked {
            int hashCode = 0;

            foreach (object item in collection) {
               if (item != null) {
                  hashCode = (hashCode * HashMultiplier) ^ item.GetHashCode();
               }
            }

            return hashCode;
         }
      }
   }
}
