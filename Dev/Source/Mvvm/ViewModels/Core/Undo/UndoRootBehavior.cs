namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class UndoRootBehavior :
      InitializableBehavior,
      IBehaviorInitializationBehavior {

      private static readonly FieldDefinitionGroup UndoManagerGroup = new FieldDefinitionGroup();
      private DynamicFieldAccessor<UndoManager> _undoManagerField;

      public void Initialize(BehaviorInitializationContext context) {
         _undoManagerField = new DynamicFieldAccessor<UndoManager>(context, UndoManagerGroup);
         SetInitialized();
         this.InitializeNext(context);
      }

      public UndoManager GetUndoManager(IBehaviorContext context) {
         if (!_undoManagerField.HasValue(context)) {
            _undoManagerField.Set(context, new UndoManager());
         }
         return _undoManagerField.Get(context);
      }
   }
}
