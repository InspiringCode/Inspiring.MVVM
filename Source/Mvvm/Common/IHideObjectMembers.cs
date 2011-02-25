namespace Inspiring.Mvvm.Common {
   using System;
   using System.ComponentModel;

   /// <summary> 
   ///   Helper interface used to hide the base <see cref="Object"/> members from 
   ///   your classes to make intellisense much cleaner. Note: This ONLY works from
   ///   other assemblies AND you MUST NOT have the project using this interface in 
   ///   your solution (else VS will still show them). 
   /// </summary>
   [EditorBrowsable(EditorBrowsableState.Never)]
   public interface IHideObjectMembers {
      [EditorBrowsable(EditorBrowsableState.Never)]
      Type GetType();

      [EditorBrowsable(EditorBrowsableState.Never)]
      int GetHashCode();

      [EditorBrowsable(EditorBrowsableState.Never)]
      string ToString();

      [EditorBrowsable(EditorBrowsableState.Never)]
      bool Equals(object obj);
   }
}
