namespace Inspiring.Mvvm.ViewModels.Core {

   public sealed class FieldDefinition<T> {
      internal FieldDefinition(
         FieldDefinitionCollection parent,
         int groupIndex,
         int fieldIndex
      ) {
         Check.NotNull(parent, nameof(parent));

         Parent = parent;
         GroupIndex = groupIndex;
         FieldIndex = fieldIndex;
      }

      internal FieldDefinitionCollection Parent { get; private set; }

      internal int GroupIndex { get; private set; }

      internal int FieldIndex { get; private set; }
   }
}
