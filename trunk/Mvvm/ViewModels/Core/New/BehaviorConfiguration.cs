namespace Inspiring.Mvvm.ViewModels.Core.New {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels.Behaviors;

   public interface IBehaviorConfigurationExpression {
      IBehaviorConfigurationExpression Add(
         IBehaviorFactory behavior,
         BehaviorOrderModifier relativeTo,
         VMBehaviors position,
         bool addLazily = false
      );

      IBehaviorConfigurationExpression Override(VMBehaviors behavior, IBehaviorFactory withBehavior);

      IBehaviorConfigurationExpression ReplaceBehaviors(BehaviorConfiguration withBehaviors);

      IBehaviorConfigurationExpression ConfigureBehavior<TBehavior>(
         VMBehaviors behavior,
         Action<TBehavior> configurationAction
      );
   }

   public sealed class BehaviorConfiguration : IBehaviorConfigurationExpression {
      private List<BehaviorEntry> _behaviors = new List<BehaviorEntry>();

      public IBehaviorConfigurationExpression Add(
         IBehaviorFactory behavior,
         BehaviorOrderModifier relativeTo,
         VMBehaviors position,
         bool addLazily
      ) {
         int relatedBehaviorIndex = _behaviors.FindIndex(e => e.Key == position);

         if (relatedBehaviorIndex == -1) {
            throw new ArgumentException(
               ExceptionTexts.BehaviorNotContainedByConfiguration.FormatWith(position)
            );
         }

         BehaviorEntry entry = new BehaviorEntry {
            Key = VMBehaviors.Custom,
            Factory = behavior,
            AddLazily = addLazily
         };

         switch (relativeTo) {
            case BehaviorOrderModifier.Before:
               _behaviors.Insert(relatedBehaviorIndex, entry);
               break;
            case BehaviorOrderModifier.After:
               _behaviors.Insert(relatedBehaviorIndex + 1, entry);
               break;
         }

         return this;
      }

      public IBehaviorConfigurationExpression Override(VMBehaviors behavior, IBehaviorFactory withBehavior) {
         BehaviorEntry entry = _behaviors.Find(e => e.Key == behavior);

         if (entry == null) {
            throw new ArgumentException(
               ExceptionTexts.BehaviorNotContainedByConfiguration.FormatWith(behavior)
            );
         }

         entry.Factory = withBehavior;

         return this;
      }

      public IBehaviorConfigurationExpression ReplaceBehaviors(BehaviorConfiguration withBehaviors) {
         IEnumerable<BehaviorEntry> clones = withBehaviors
            ._behaviors
            .Select(e => new BehaviorEntry {
               Key = e.Key,
               Factory = e.Factory,
               AddLazily = e.AddLazily,
            });

         _behaviors.Clear();
         _behaviors.AddRange(clones);

         return this;
      }

      public IBehaviorConfigurationExpression ConfigureBehavior<TBehavior>(
         VMBehaviors behavior,
         Action<TBehavior> configurationAction
      ) {
         List<BehaviorEntry> entries = _behaviors.FindAll(e => e.Key == behavior);

         if (entries.Count == 0) {
            throw new ArgumentException(
               ExceptionTexts.BehaviorNotContainedByConfiguration.FormatWith(behavior)
            );
         }

         entries.ForEach(e => {
            e.ConfigurationAction = b => {
               TBehavior typed = (TBehavior)b;
               configurationAction(typed);
            };
         });

         return this;
      }

      public BehaviorConfiguration Clone() {
         BehaviorConfiguration clone = new BehaviorConfiguration();
         clone.ReplaceBehaviors(this);
         return clone;
      }

      public VMPropertyBehaviorChain CreateBehaviorChain<TValue>() {
         VMPropertyBehaviorChain chain = new VMPropertyBehaviorChain();
         IBehavior previous = chain;

         foreach (BehaviorEntry entry in _behaviors) {
            if (entry.ConfigurationAction != null || !entry.AddLazily) {
               IBehavior b = entry.Factory.Create<TValue>();

               if (entry.ConfigurationAction != null) {
                  entry.ConfigurationAction(b);
               }

               previous.Successor = b;
               previous = b;
            }
         }

         return chain;
      }

      private class BehaviorEntry {
         public VMBehaviors Key { get; set; }
         public IBehaviorFactory Factory { get; set; }
         public bool AddLazily { get; set; }
         public Action<IBehavior> ConfigurationAction { get; set; }
      }
   }

   public enum BehaviorOrderModifier {
      Before,
      After
   }
}
