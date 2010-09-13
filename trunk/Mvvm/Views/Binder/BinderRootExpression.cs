namespace Inspiring.Mvvm.Views.Binder {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public abstract class BinderRootExpression {
      private List<BinderContext> _queuedExecutions = new List<BinderContext>();
      private List<IBinderBuildStep> _buildSteps = new List<IBinderBuildStep>();

      public void InsertBuildStep(IBinderBuildStep step) {
         Contract.Requires<ArgumentNullException>(step != null);
         _buildSteps.Insert(0, step);
      }

      public virtual BinderContext QueueBuilderExecution() {
         BinderContext execution = new BinderContext();
         _queuedExecutions.Add(execution);
         return execution;
      }

      public void Execute() {
         foreach (BinderContext context in _queuedExecutions) {
            foreach (IBinderBuildStep step in _buildSteps) {
               step.Execute(context);

               if (context.Complete) {
                  break;
               }
            }
         }
      }
   }
}
