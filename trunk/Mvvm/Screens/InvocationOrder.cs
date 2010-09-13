namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using System.Reflection;

   public enum InvocationOrder {
      Never = 0,
      First = 0x10,
      BeforeParent = 0x100,
      Parent = 0x1000,
      AfterParent = 0x10000,
      Last = 0x100000
   }

   [AttributeUsage(AttributeTargets.Method)]
   public class InvocationOrderAttribute : Attribute {
      public InvocationOrderAttribute(InvocationOrder order) {
         Order = order;
      }

      public InvocationOrder Order { get; private set; }

      internal static InvocationOrder GetOrder(
         IScreenLifecycle handler,
         string lifecycleMethodName
      ) {
         MethodInfo method = new Type[] { handler.GetType() }
            .Concat(handler.GetType().GetInterfaces())
            .Select(t =>
               t.GetMethod(
                  lifecycleMethodName,
                  BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
               )
            )
            .FirstOrDefault(t => t != null);

         if (method == null) {
            Contract.Assert(lifecycleMethodName == "Initialize");
            return InvocationOrder.Never;
         }

         InvocationOrderAttribute attr = (InvocationOrderAttribute)
            Attribute.GetCustomAttribute(
               method,
               typeof(InvocationOrderAttribute)
            );

         return attr != null ?
            attr.Order :
            InvocationOrder.Parent;
      }
   }
}
