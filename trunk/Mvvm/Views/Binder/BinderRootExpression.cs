namespace Inspiring.Mvvm.Views.Binder {
   using System;

   public abstract class BinderRootExpression {
      public void InsertBuildStep(IBinderBuildStep step) {
         throw new NotImplementedException();
      }

      public virtual BinderContext QueueBuilderExecution() {
         throw new NotImplementedException();
      }

      public void Execute() {
         // foreach build step execute with context...
      }
   }
}
