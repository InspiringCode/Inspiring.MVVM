namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Core;

   /// <summary>
   ///   An alternative implementation of the standard display value accessor 
   ///   behavior that enables that any <see cref="IEnumerable{T}"/> can be
   ///   assigned to the display value of a VM collection property.
   /// </summary>
   /// <typeparam name="TItemVM"></typeparam>
   internal sealed class SettableListDisplayValueBehavior<TItemVM> :
      Behavior,
      IDisplayValueAccessorBehavior
      where TItemVM : IViewModel {

      /// <inheritdoc />
      public object GetDisplayValue(IBehaviorContext context) {
         return GetTargetCollection(context);
      }

      /// <inheritdoc />
      public void SetDisplayValue(IBehaviorContext context, object value) {
         IEnumerable<TItemVM> sourceCollection = (IEnumerable<TItemVM>)value;
         IVMCollection<TItemVM> targetCollection = GetTargetCollection(context);

         SynchronizeCollections(sourceCollection, targetCollection);
      }

      /// <summary>
      ///   Ensures that the <paramref name="targetCollection"/> has the same
      ///   elements in the same sequence as the <paramref name="sourceCollection"/>.
      /// </summary>
      private static void SynchronizeCollections(
         IEnumerable<TItemVM> sourceCollection,
         IList<TItemVM> targetCollection
      ) {
         if (sourceCollection == null) {
            targetCollection.Clear();
            return;
         }

         int targetIndex = 0;

         foreach (TItemVM sourceItem in sourceCollection) {
            if (targetIndex >= targetCollection.Count) {
               targetCollection.Add(sourceItem);
            } else {
               // Update all target collection position that currently hold a different
               // item the same position in the source collection.
               if (!Object.ReferenceEquals(targetCollection[targetIndex], sourceItem)) {
                  targetCollection[targetIndex] = sourceItem;
               }
            }

            targetIndex++;
         }

         // Remove left over items if the number of items of the source collection is
         // less than the number of items of the target collection.
         while (targetIndex < targetCollection.Count) {
            targetCollection.RemoveAt(targetCollection.Count - 1);
         }
      }

      private IVMCollection<TItemVM> GetTargetCollection(IBehaviorContext context) {
         return this.GetValueNext<IVMCollection<TItemVM>>(context, ValueStage.None);
      }
   }
}
