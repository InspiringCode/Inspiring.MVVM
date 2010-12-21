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
         int targetIndex = 0;
         foreach (TItemVM sourceItem in sourceCollection) {
            if (targetIndex >= targetCollection.Count) {
               targetCollection.Add(sourceItem);
            } else {
               if (!Object.ReferenceEquals(targetCollection[targetIndex], sourceItem)) {
                  targetCollection[targetIndex] = sourceItem;
               }
            }

            targetIndex++;
         }
      }

      private IVMCollection<TItemVM> GetTargetCollection(IBehaviorContext context) {
         return this.GetValueNext<IVMCollection<TItemVM>>(context, ValueStage.None);
      }
   }
}
