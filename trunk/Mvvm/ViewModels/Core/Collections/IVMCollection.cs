namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   /// <summary>
   ///   An interface that should be implemented by collection classes that are
   ///   be used in VMs to hold a collection of child VMs. This interface is
   ///   especially required by the predefined collection behaviors.
   /// </summary>
   public interface IVMCollection<TItemVM> : ICollection<TItemVM> {
      // TODO: Comment.
      BehaviorChain Behaviors { get; }

      /// <summary>
      ///   <para>Indicates wheather the collection is in the middle of a complete 
      ///      repoulation process.</para>
      ///   <para>The collection that implements this interface should not raise
      ///      collection/list changed events when this property is set to true and 
      ///      should raise a collection/list reset event when it changes from true 
      ///      to false.</para>
      ///   <para>Collection behaviors can use this property to suppress certain
      ///      action while a collection is repopulated (e.g. raising validation
      ///      events) to improve performance or avoid endless recursions.</para>
      /// </summary>
      bool IsPopulating { get; set; }

      /// <summary>
      ///   Gets the view model instance that holds this collection instance. 
      ///   The <see cref="Owner"/> is the <see cref="IViewModel.Parent"/>
      ///   of all items.
      /// </summary>
      IViewModel Owner { get; }
   }
}
