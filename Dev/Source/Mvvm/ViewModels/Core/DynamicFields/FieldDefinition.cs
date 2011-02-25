namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;

   public sealed class FieldDefinition<T> {
      internal FieldDefinition(
         FieldDefinitionCollection parent,
         int groupIndex,
         int fieldIndex
      ) {
         Contract.Requires(parent != null);

         Parent = parent;
         GroupIndex = groupIndex;
         FieldIndex = fieldIndex;
      }

      internal FieldDefinitionCollection Parent { get; private set; }

      internal int GroupIndex { get; private set; }

      internal int FieldIndex { get; private set; }
   }
}
