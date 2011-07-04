namespace BehaviorSpike {
   using System;
   using System.Diagnostics;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class SpikeTest {
      [TestMethod]
      public void PerformanceTest() {
         var property = new VMPropertyDescriptor(new Behavior[] {
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>()
         });

         const int runs = 20 * 1000 * 1000;

         Stopwatch sw = Stopwatch.StartNew();

         for (int i = 0; i < runs; i++) {
            GetContext(property).SetValueNew("Test");
         }

         sw.Stop();

         Console.WriteLine(SetValueBehavior<string>.InvocationCount);
         Console.WriteLine(SetValueBehavior<int>.InvocationCount);

         Assert.Fail("Runs per second: {0}", (long)runs * 1000 / sw.ElapsedMilliseconds);
      }

      [TestMethod]
      public void Performance_SetValueNextOld() {
         var property = new VMPropertyDescriptor(new Behavior[] {
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>(),
            new SetValueBehavior<string>()
         });

         var rootBehavior = new Behavior { Successor = property.Behaviors[0] };
         var context = GetContext(property);

         const int runs = 20 * 1000 * 1000;

         Stopwatch sw = Stopwatch.StartNew();

         for (int i = 0; i < runs; i++) {
            rootBehavior.SetValueNextOld(context, "Test");
         }

         sw.Stop();

         Console.WriteLine(SetValueBehavior<string>.InvocationCount);
         Console.WriteLine(SetValueBehavior<int>.InvocationCount);

         Assert.Fail("Runs per second: {0}", (long)runs * 1000 / sw.ElapsedMilliseconds);
      }

      private PropertyBehaviorContext GetContext(VMPropertyDescriptor property) {
         return new PropertyBehaviorContext(property.Behaviors, 0);
      }
   }

   internal class PropertyBehaviorContext : BehaviorContext {
      private IBehavior[] _behaviors;
      private int _index;

      public PropertyBehaviorContext(IBehavior[] behaviors, int index) {
         _behaviors = behaviors;
         _index = index;
      }

      public bool TryGetNext<T>(out T foundBehavior, out PropertyBehaviorContext context) where T : IBehavior {
         for (int i = _index; i < _behaviors.Length; i++) {
            IBehavior b = _behaviors[i];
            if (b is T) {
               foundBehavior = (T)b;
               context = new PropertyBehaviorContext(_behaviors, i + 1);
               return true;
            }
         }

         foundBehavior = default(T);
         context = null;
         return false;
      }

      public PropertyBehaviorContext MoveNext() {
         return new PropertyBehaviorContext(_behaviors, _index + 1);
      }
   }

   internal class SetValueBehavior<T> : Behavior {
      public static long InvocationCount = 0;

      public void SetValueNew(PropertyBehaviorContext context, T value) {
         InvocationCount++;
         context.SetValueNew(value);
      }

      public void SetValueOld(PropertyBehaviorContext context, T value) {
         InvocationCount++;
         this.SetValueNextOld(context, value);
      }
   }

   internal class VMPropertyDescriptor {
      public VMPropertyDescriptor(Behavior[] behaviors) {
         Behaviors = behaviors;

         for (int i = 0; i < behaviors.Length - 1; i++) {
            behaviors[i].Successor = behaviors[i + 1];
         }
      }

      public Behavior[] Behaviors { get; private set; }
   }

   internal static class Extensions {
      public static void SetValueNew<T>(this PropertyBehaviorContext context, T value) {
         SetValueBehavior<T> behavior;
         PropertyBehaviorContext next;
         if (context.TryGetNext(out behavior, out next)) {
            behavior.SetValueNew(next, value);
         }
      }

      public static void SetValueNextOld<T>(this Behavior behavior, PropertyBehaviorContext context, T value) {
         SetValueBehavior<T> next;
         if (behavior.TryGetBehavior(out next)) {
            next.SetValueOld(context, value);
         }
      }
   }
}