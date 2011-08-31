namespace Inspiring.Mvvm.Common.Core {
   using System;
   using System.Reflection;

   internal static class UniqueAppKey {
      public static string Get() {
         var entryAssembly = Assembly.GetEntryAssembly();

         if (entryAssembly == null || entryAssembly.EntryPoint == null) {
            throw new InvalidOperationException();
         }

         string entryTypeNamespace = entryAssembly
            .EntryPoint
            .DeclaringType
            .FullName;

         return entryTypeNamespace;
      }

      public static string GetWithPrefix(string prefix) {
         return prefix + Get();
      }
   }
}
