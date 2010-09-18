namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   public sealed class BehaviorConfiguration : IBehaviorConfigurationExpression {
      private List<Action<List<BehaviorEntry>>> _configurationActions = new List<Action<List<BehaviorEntry>>>();

      public BehaviorConfiguration Append(VMBehaviorKey behaviorKey, bool disabled = false) {
         Insert(behaviorKey, RelativePosition.After, VMBehaviorKey.Last);

         if (!disabled) {
            Enable(behaviorKey);
         }

         return this;
      }

      public IBehaviorConfigurationExpression Insert(
         VMBehaviorKey behaviorKey,
         RelativePosition relativeTo,
         VMBehaviorKey other
      ) {
         _configurationActions.Add(entries => {
            int otherBehaviorIndex = 0;

            if (entries.Any()) {
               otherBehaviorIndex = GetIndex(entries, other);

               if (relativeTo == RelativePosition.After) {
                  otherBehaviorIndex++;
               }
            }

            BehaviorEntry entry = new BehaviorEntry {
               Key = behaviorKey,
               IsEnabled = false
            };

            entries.Insert(otherBehaviorIndex, entry);
         });

         return this;
      }

      public IBehaviorConfigurationExpression OverrideFactory(
         VMBehaviorKey behaviorKey,
         IBehaviorFactory factory
      ) {
         _configurationActions.Add(entries => {
            GetEntry(entries, behaviorKey).Factory = factory;
         });

         return this;
      }

      public IBehaviorConfigurationExpression Enable(
         VMBehaviorKey behaviorKey
      ) {
         _configurationActions.Add(entries => {
            GetEntry(entries, behaviorKey).IsEnabled = true;
         });

         return this;
      }

      public IBehaviorConfigurationExpression Configure<TBehavior>(
         VMBehaviorKey behaviorKey,
         Action<TBehavior> configurationAction
      ) {
         _configurationActions.Add(entries => {
            List<BehaviorEntry> found = entries.FindAll(e => e.Key == behaviorKey);

            if (found.Count == 0) {
               throw new ArgumentException(
                  ExceptionTexts.BehaviorNotContainedByConfiguration.FormatWith(behaviorKey)
               );
            }

            found.ForEach(e => {
               e.ConfigurationAction += b => {
                  TBehavior typed = (TBehavior)b;
                  configurationAction(typed);
               };
            });
         });

         return this;
      }

      public IBehaviorConfigurationExpression MergeFrom(
         BehaviorConfiguration additionalConfiguration
      ) {
         _configurationActions.AddRange(
            additionalConfiguration._configurationActions
         );

         return this;
      }

      public BehaviorConfiguration Clone() {
         BehaviorConfiguration clone = new BehaviorConfiguration();
         clone.MergeFrom(this);
         return clone;
      }

      public Behavior CreateBehaviorChain<TValue>() {
         List<BehaviorEntry> entries = new List<BehaviorEntry>();

         foreach (var action in _configurationActions) {
            action(entries);
         }

         Behavior chain = new Behavior();
         IBehavior previous = chain;

         foreach (BehaviorEntry entry in entries) {
            if (entry.IsEnabled) {
               IBehaviorFactory factory =
                  entry.Factory ??
                  new DefaultBehaviorFactory(entry.Key);

               IBehavior b = factory.Create<TValue>();

               if (entry.ConfigurationAction != null) {
                  entry.ConfigurationAction(b);
               }

               previous.Successor = b;
               previous = b;
            }
         }

         return chain;
      }

      private static BehaviorEntry GetEntry(List<BehaviorEntry> entries, VMBehaviorKey key) {
         return entries[GetIndex(entries, key)];
      }

      private static int GetIndex(List<BehaviorEntry> entries, VMBehaviorKey key) {
         if (entries.Count > 0) {
            if (key == VMBehaviorKey.First) {
               return 0;
            }
            if (key == VMBehaviorKey.Last) {
               return entries.Count - 1;
            }
         }

         int index = entries.FindIndex(e => e.Key == key);

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
         public bool IsEnabled { get; set; }
         public Action<IBehavior> ConfigurationAction { get; set; }

         public BehaviorEntry Clone() {
            return new BehaviorEntry {
               Key = Key,
               Factory = Factory,
               IsEnabled = IsEnabled,
               ConfigurationAction = ConfigurationAction
            };
         }
      }
   }
}
