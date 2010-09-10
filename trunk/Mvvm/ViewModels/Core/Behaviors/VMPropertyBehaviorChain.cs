namespace Inspiring.Mvvm.ViewModels.Behaviors {
   using System;

   public class VMPropertyBehaviorChain : VMPropertyBehavior {
      public override BehaviorPosition Position {
         get { return BehaviorPosition.ChainHeader; }
      }

      protected TBehavior EnsureBehavior<TBehavior>(Func<TBehavior> behaviorFactory)
         where TBehavior : IBehavior {
         TBehavior behavior;

         if (!TryGetBehavior<TBehavior>(out behavior)) {
            behavior = behaviorFactory();
            InsertBehavior(behavior);
         }

         return behavior;
      }

      protected void InsertBehavior(IBehavior insert) {
         IBehavior insertAfter = this;

         while (insertAfter.Successor != null &&
                insertAfter.Successor.Position >= insert.Position
         ) {
            insertAfter = insertAfter.Successor;
         }

         insert.Successor = insertAfter.Successor;
         insertAfter.Successor = insert;
      }

      // TODO: Clean up?
      internal override void Initialize(FieldDefinitionCollection fields, string propertyName) {
         base.Initialize(fields, propertyName);
         for (IBehavior b = this.Successor; b != null; b = b.Successor) {
            ((VMPropertyBehavior)b).Initialize(fields, propertyName);
         }
      }
   }
}
