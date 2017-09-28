namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Collections.ObjectModel;
   using System.Linq;

   /// <summary>
   ///   Simulates a one-to-many relation between B and A if there is only a
   ///   one-to-many relation between A and B.
   /// </summary>
   /// <typeparam name="TSource">
   ///   The source object from which the reverse one-to-many relation should
   ///   originate (B in the above example).
   /// </typeparam>
   /// <typeparam name="TTarget">
   ///   The target object that should be referenced by the reverse one-to-many
   ///   relation (A in the above example).
   /// </typeparam>
   /// <remarks>
   ///   <para>This class implements <see cref="IList{T}"/>. It initially contains all 
   ///      target objects, that reference the source object of this reverse one-to-many
   ///      collection (see constructor).</para>
   ///   <para>If a target object is added to this collection, the source object is 
   ///     added to the back reference collection of the added target item.</para>
   ///   <para>If a target object is remove from this collection, the source object is
   ///      removed from the back reference collection of the added target item.</para>
   /// </remarks>
   public class ReverseOneToManyCollection<TSource, TTarget> : Collection<TTarget> {
      private TSource _sourceObject;
      private IEnumerable<TTarget> _possibleTargets;
      private Func<TTarget, ICollection<TSource>> _backReferennceSelector;
      private bool _isPopulating = false;

      /// <summary>
      ///   This collection contains all target objects, whose one-to-many reference
      ///   to its source objects (selected with the <paramref name="backReferenceSelector"/>)
      ///   contains the give <paramref name="sourceObject"/>.
      /// </summary>
      /// <param name="sourceObject">
      ///   The "one" side of the reverse one-to-many collection.
      /// </param>
      /// <param name="possibleTargetObjects">
      ///   All possible objects that may be referenced by the reverse one-to-many
      ///   side (the "many" side of the reverse one-to-many collection).
      /// </param>
      /// <param name="backReferenceSelector">
      ///   A delegate that returns a list of source objects referenced by a possible
      ///   target object.
      /// </param>
      public ReverseOneToManyCollection(
         TSource sourceObject,
         IEnumerable<TTarget> possibleTargetObjects,
         Func<TTarget, ICollection<TSource>> backReferenceSelector
      ) {
         Check.NotNull(sourceObject, nameof(sourceObject));
         Check.NotNull(possibleTargetObjects, nameof(possibleTargetObjects));
         Check.NotNull(backReferenceSelector, nameof(backReferenceSelector));

         _sourceObject = sourceObject;
         _possibleTargets = possibleTargetObjects;
         _backReferennceSelector = backReferenceSelector;

         Populate();
      }

      /// <inheritdoc />
      protected override void InsertItem(int index, TTarget item) {
         OnTargetItemAdded(item);

         base.InsertItem(index, item);
      }

      /// <inheritdoc />
      protected override void RemoveItem(int index) {
         TTarget item = this[index];
         OnTargetItemRemoved(item);

         base.RemoveItem(index);
      }

      /// <inheritdoc />
      protected override void ClearItems() {
         this.ForEach(OnTargetItemRemoved);

         base.ClearItems();
      }

      /// <inheritdoc />
      protected override void SetItem(int index, TTarget item) {
         OnTargetItemRemoved(this[index]);
         OnTargetItemAdded(item);

         base.SetItem(index, item);
      }

      private void Populate() {
         try {
            _isPopulating = true;
            PopulateCore();
         } finally {
            _isPopulating = false;
         }
      }

      private void PopulateCore() {
         _possibleTargets
            .Where(ReferencesSourceObject)
            .ForEach(Add);
      }

      private bool ReferencesSourceObject(TTarget targetObject) {
         ICollection<TSource> referencedSources = _backReferennceSelector(targetObject);
         return referencedSources.Contains(_sourceObject);
      }

      private void OnTargetItemAdded(TTarget item) {
         if (!_isPopulating) {
            ICollection<TSource> referencedSources = _backReferennceSelector(item);
            referencedSources.Add(_sourceObject);
         }
      }

      private void OnTargetItemRemoved(TTarget item) {
         if (!_isPopulating) {
            ICollection<TSource> referencedSources = _backReferennceSelector(item);
            referencedSources.Remove(_sourceObject);
         }
      }
   }
}
