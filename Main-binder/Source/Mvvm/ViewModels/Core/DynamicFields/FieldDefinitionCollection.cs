namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;

   public sealed class FieldDefinitionCollection {
      private List<FieldGroupInfo> _containedGroups = new List<FieldGroupInfo>();

      public FieldDefinitionCollection() {

      }

      public FieldDefinition<T> DefineField<T>(FieldDefinitionGroup group) {
         Contract.Requires<ArgumentNullException>(group != null);
         Contract.Ensures(Contract.Result<FieldDefinition<T>>() != null);

         FieldGroupInfo info = EnsureGroup(group);

         FieldDefinition<T> field = new FieldDefinition<T>(
            parent: this,
            groupIndex: info.GroupIndex,
            fieldIndex: info.FieldCount
         );

         info.FieldCount++;

         return field;
      }

      public FieldValueHolder CreateValueHolder() {
         return new FieldValueHolder(this);
      }

      internal int GetGroupCount() {
         return _containedGroups.Count;
      }

      internal int GetFieldCount(int groupIndex) {
         FieldGroupInfo info = _containedGroups.Find(g => g.GroupIndex == groupIndex);

         Contract.Assert(
            info != null,
            "This 'FieldDefintionCollection' does not contain a group with the specifed index."
         );

         return info.FieldCount;
      }

      private FieldGroupInfo EnsureGroup(FieldDefinitionGroup group) {
         FieldGroupInfo info = _containedGroups.Find(g => g.Group == group);

         if (info == null) {
            info = new FieldGroupInfo {
               Group = group,
               GroupIndex = _containedGroups.Count,
               FieldCount = 0
            };

            _containedGroups.Add(info);
         }

         return info;
      }

      private class FieldGroupInfo {
         public FieldDefinitionGroup Group { get; set; }
         public int GroupIndex { get; set; }
         public int FieldCount { get; set; }
      }
   }
}
