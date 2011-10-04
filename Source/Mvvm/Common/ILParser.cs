namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using System.Reflection;
   using System.Reflection.Emit;

   internal sealed class ILParser {
      private static readonly Dictionary<short, OpCode> _opCodes = CreateOpCodeLookup();
      private readonly byte[] _ilCode;
      private readonly Type _declaringType;

      public ILParser(MethodInfo method) {
         Contract.Requires(method != null);

         _ilCode = method
            .GetMethodBody()
            .GetILAsByteArray();

         _declaringType = method.DeclaringType;
      }

      public IEnumerable<MemberInfo> GetInvokedMethods() {
         List<byte> stream = _ilCode.ToList();

         while (stream.Any()) {
            OpCode op = ReadOpCode(stream);

            if (op == OpCodes.Call || op == OpCodes.Calli || op == OpCodes.Callvirt) {
               int metadataToken = ReadOperand(stream);
               MemberInfo info = _declaringType.Module.ResolveMethod(metadataToken);
               yield return info;
            }
         }
      }

      public IEnumerable<PropertyInfo> GetAccessedProperties() {
         IEnumerable<MemberInfo> methods = GetInvokedMethods();

         return methods
            .Select(m => TryGetProperty(m))
            .Where(p => p != null);
      }

      private PropertyInfo TryGetProperty(MemberInfo getSetMethod) {
         string name = getSetMethod.Name;

         if (name.StartsWith("get_") || name.StartsWith("set_")) {
            return getSetMethod
               .DeclaringType
               .GetProperty(name.Substring(startIndex: 4));
         }

         return null;
      }

      private int ReadOperand(List<byte> stream) {
         byte[] bytes = stream
            .Take(4)
            .ToArray();

         stream.RemoveRange(0, count: 4);

         return BitConverter.ToInt32(bytes, 0);
      }

      private OpCode ReadOpCode(List<byte> stream) {
         OpCode op;

         byte byteCode = stream.First();
         stream.RemoveAt(0);

         if (_opCodes.TryGetValue(byteCode, out op)) {
            return op;
         }

         short shortCode = (short)(byteCode << 8 + stream.First());
         stream.RemoveAt(0);

         if (_opCodes.TryGetValue(shortCode, out op)) {
            return op;
         }

         throw new ArgumentException();
      }

      private void SkipOpCode(List<byte> stream, OpCode code) {
         stream.RemoveRange(0, count: code.Size);
      }

      private static Dictionary<short, OpCode> CreateOpCodeLookup() {
         return typeof(OpCodes)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(fi => typeof(OpCode).IsAssignableFrom(fi.FieldType))
            .Select(fi => (OpCode)fi.GetValue(null))
            .Where(op => op.OpCodeType != OpCodeType.Nternal)
            .ToDictionary(op => op.Value);
      }
   }
}
