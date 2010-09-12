namespace Inspiring.Mvvm.ViewModels.Core {
   using System;
   using System.Diagnostics.Contracts;
   using System.Linq;

   // TODO: modifiing definition after creating target holder
   public sealed class FieldValueHolder {
      private static object _unsetValue = new Object();
      private FieldDefinitionCollection _parent;
      private object[][] _fieldValues;

      internal FieldValueHolder(FieldDefinitionCollection parent) {
         Contract.Requires<ArgumentNullException>(parent != null);
         _parent = parent;
      }

      public bool HasValue<T>(FieldDefinition<T> field) {
         CheckField(field);

         T value;
         return TryGetValue(field, out value);
      }

      public T GetValue<T>(FieldDefinition<T> field) {
         CheckField(field);

         T value;
         if (TryGetValue(field, out value)) {
            return value;
         }

         throw new InvalidOperationException(ExceptionTexts.FieldNotSet);
      }

      public T GetValueOrDefault<T>(FieldDefinition<T> field) {
         CheckField(field);

         T value;
         if (TryGetValue(field, out value)) {
            return value;
         }

         return default(T);
      }

      public bool TryGetValue<T>(FieldDefinition<T> field, out T value) {
         value = default(T);

         if (_fieldValues == null) {
            return false;
         }

         if (_fieldValues[field.GroupIndex] == null) {
            return false;
         }

         object rawValue = _fieldValues[field.GroupIndex][field.FieldIndex];

         if (rawValue == _unsetValue) {
            return false;
         }

         value = (T)rawValue;
         return true;
      }

      public void SetValue<T>(FieldDefinition<T> field, T value) {
         CheckField(field);

         EnsureFieldValues(field.GroupIndex);
         _fieldValues[field.GroupIndex][field.FieldIndex] = value;
      }

      public void ClearField<T>(FieldDefinition<T> field) {
         CheckField(field);

         if (HasValue(field)) {
            _fieldValues[field.GroupIndex][field.FieldIndex] = _unsetValue;
            // TODO: Reclaim unused memory
         }
      }

      private void CheckField<T>(FieldDefinition<T> field) {
         if (field.Parent != _parent) {
            throw new ArgumentException(ExceptionTexts.ForeignField);
         }
      }

      private void EnsureFieldValues(int groupIndex) {
         int groupCount = _parent.GetGroupCount();
         Contract.Assert(groupCount > groupIndex);

         if (_fieldValues == null) {
            _fieldValues = new object[groupCount][];
         }

         if (_fieldValues[groupIndex] == null) {
            int fieldCount = _parent.GetFieldCount(groupIndex);

            // Initialize all elements with a marker object so we can distinguish
            // unset values and null values. Note that an array of _unsetValue
            // not require more memory than an array of nulls.
            _fieldValues[groupIndex] = Enumerable.Repeat(_unsetValue, fieldCount).ToArray();
         }
      }
   }
}
