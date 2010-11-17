namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using System.Linq;

   /// <summary>
   ///   A class that describes a list of VM proprerties that need to be get in
   ///   the order specified by the VMPropertyPath to get from one VM to an 
   ///   descendant VM in a VM hierarchy.
   /// </summary>
   public sealed class VMPropertyPath {
      public static readonly VMPropertyPath Empty = new VMPropertyPath();

      public VMPropertyPath(params IVMProperty[] properties) {
         Contract.Requires<ArgumentNullException>(properties != null);
         Contract.Requires(Contract.ForAll(properties, x => x != null));

         Properties = properties;
      }

      /// <summary>
      ///   Gets the properties of the path in sequence. Example [Address, Street].
      /// </summary>
      public IVMProperty[] Properties { get; private set; }

      public int Length {
         get { return Properties.Length; }
      }

      /// <inheritdoc />
      public override string ToString() {
         return String.Join(".", Properties.Select(x => x.PropertyName));
      }
   }
}
