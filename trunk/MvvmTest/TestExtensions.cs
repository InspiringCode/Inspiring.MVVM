namespace Inspiring.MvvmTest {
   using System;
   using System.Reflection;

   internal static class TestExtensions {
      /// <summary>
      ///   Calls the given method with the given arguments.
      /// </summary>
      public static TReturnValue Invoke<TReturnValue>(this object obj, string methodName, params object[] args) {
         MethodInfo meth = obj.GetType().GetMethod(
            methodName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
         );

         if (meth != null) {
            return (TReturnValue)meth.Invoke(obj, args);
         }

         throw new InvalidOperationException(String.Format(
             "Method'{0}' was not found on object '{1}'.",
             methodName,
             obj
          ));
      }

      /// <summary>
      ///   Calls the given method with the given arguments.
      /// </summary>
      public static void Invoke(this object obj, string methodName, params object[] args) {
         MethodInfo meth = obj.GetType().GetMethod(
            methodName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
         );

         if (meth == null) {
            throw new InvalidOperationException(String.Format(
               "Method'{0}' was not found on object '{1}'.",
               methodName,
               obj
            ));
         }

         meth.Invoke(obj, args);
      }

      /// <summary>
      ///   Returns the value of the property of field specified by 'memberName'.
      /// </summary>
      public static TMember Reveal<TMember>(this object obj, string memberName) {
         PropertyInfo prop = obj.GetType().GetProperty(
            memberName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
         );

         if (prop != null) {
            return (TMember)prop.GetValue(obj, null);
         }

         FieldInfo field = obj.GetType().GetField(
            memberName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
         );

         if (field != null) {
            return (TMember)field.GetValue(obj);
         }

         throw new InvalidOperationException(String.Format(
            "Property or field '{0}' was not found on object '{1}'.",
            memberName,
            obj
         ));
      }
   }
}
