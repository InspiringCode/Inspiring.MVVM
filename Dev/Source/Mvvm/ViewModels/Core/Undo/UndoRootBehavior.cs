namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class UndoRootBehavior :
      InitializableBehavior,
      IBehaviorInitializationBehavior {

      public void Initialize(BehaviorInitializationContext context) {
         SetInitialized();
         this.InitializeNext(context);
      }
   }
}
