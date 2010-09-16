namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   public sealed class BehaviorConfiguration : IBehaviorConfigurationExpression2 {
      private List<BehaviorEntry> _behaviors = new List<BehaviorEntry>();

      /// <inheritdoc/>
      public IBehaviorConfigurationExpression2 Add(
         VMBehaviorKey key,
         IBehaviorFactory behavior,
         RelativePosition relativeTo,
         VMBehaviorKey position,
         bool addLazily = false
      ) {
         int relatedBehaviorIndex = 0;

         if (_behaviors.Any()) {
            relatedBehaviorIndex = GetIndex(position);

            if (relativeTo == RelativePosition.After) {
               relatedBehaviorIndex++;
            }
         }

         BehaviorEntry entry = new BehaviorEntry {
            Key = key,
            Factory = behavior,
            AddLazily = addLazily
         };

         _behaviors.Insert(relatedBehaviorIndex, entry);
         return this;
      }

      /// <inheritdoc/>
      public IBehaviorConfigurationExpression2 Override(
         VMBehaviorKey behavior,
         IBehaviorFactory withBehavior,
         bool addLazily = false
      ) {
         int index = GetIndex(behavior);
         BehaviorEntry entry = _behaviors[index];

         entry.Factory = withBehavior;
         entry.AddLazily = addLazily;

         return this;
      }

      /// <inheritdoc/>
      public IBehaviorConfigurationExpression2 OverridePermanently(
         VMBehaviorKey behavior,
         IBehaviorFactory withBehavior,
         bool addLazily = false
      ) {
         int index = GetIndex(behavior);
         BehaviorEntry entry = _behaviors[index];

         entry.Factory = withBehavior;
         entry.AddLazily = addLazily;
         entry.Permanent = true;

         return this;
      }

      /// <inheritdoc/>
      public IBehaviorConfigurationExpression2 ReplaceBehaviors(BehaviorConfiguration withBehaviors) {
         BehaviorEntry[] clones = withBehaviors
            ._behaviors
            .Select(e => {
               BehaviorEntry permanent = _behaviors
                  .SingleOrDefault(x => x.Permanent && x.Key == e.Key);

               return (permanent ?? e).Clone();
            })
            .ToArray();

         _behaviors.Clear();
         _behaviors.AddRange(clones);

         return this;
      }

      /// <inheritdoc/>
      public IBehaviorConfigurationExpression2 ConfigureBehavior<TBehavior>(
         VMBehaviorKey behavior,
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

      public Behavior CreateBehaviorChain<TValue>() {
         Behavior chain = new Behavior();
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

      private int GetIndex(VMBehaviorKey key) {
         if (_behaviors.Count > 0) {
            if (key == VMBehaviorKey.First) {
               return 0;
            }
            if (key == VMBehaviorKey.Last) {
               return _behaviors.Count - 1;
            }
         }

         int index = _behaviors.FindIndex(e => e.Key == key);

         if (index == -1) {
            throw new ArgumentException(
               ExceptionTexts.BehaviorNotContainedByConfiguration.FormatWith(key)
            );
         }

         return index;
      }

      private class BehaviorEntry {
         public VMBehaviorKey Key { get; set; }
         public IBehaviorFactory Factory { get; set; }
         public bool AddLazily { get; set; }
         public bool Permanent { get; set; }
         public Action<IBehavior> ConfigurationAction { get; set; }

         public BehaviorEntry Clone() {
            return new BehaviorEntry {
               Key = Key,
               Factory = Factory,
               AddLazily = AddLazily,
               Permanent = Permanent,
               ConfigurationAction = ConfigurationAction
            };
         }
      }
   }
}
