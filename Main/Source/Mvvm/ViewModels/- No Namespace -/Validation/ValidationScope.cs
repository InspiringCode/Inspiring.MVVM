namespace Inspiring.Mvvm.ViewModels {
   /// <summary>
   ///   Determines what part of the VM hierarchy is revalidated.
   /// </summary>
   public enum ValidationScope {
      /// <summary>
      ///   Only the current VM itself is validated. All validations that 
      ///   ancestors define for this VM are also performed.
      /// </summary>
      SelfOnly,

      /// <summary>
      ///   The current VM and all its descendants are revalidated.
      /// </summary>
      FullSubtree,

      /// <summary>
      ///   The current VM is revalidated and all children, for which the 
      ///   current VM defines validators.
      /// </summary>
      SelfAndValidatedChildren,

      SelfAndLoadedDescendants
   }
}
