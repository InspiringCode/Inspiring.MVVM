namespace Inspiring.MvvmTest.ViewModels {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class PropertyStub<T> : VMPropertyDescriptor<T> {
      private ValueAccessorStub<T> _valueAccessor = new ValueAccessorStub<T>();

      /// <summary>
      ///   Use the <see cref="PropertyStubBuilder"/> to create instances.
      /// </summary>
      public PropertyStub(string propertyName, IEnumerable<IBehavior> behaviors) {
         Initialize(propertyName);

         behaviors = behaviors.Concat(new[] { _valueAccessor });

         IBehavior last = Behaviors;

         foreach (IBehavior b in behaviors) {
            last.Successor = b;
            last = b;
         }
      }

      public T Value {
         get { return _valueAccessor.Value; }
         set { _valueAccessor.Value = value; }
      }
   }

   public sealed class PropertyStub {
      public static PropertyStubBuilder Named(string name) {
         return new PropertyStubBuilder().Named(name);
      }

      public static PropertyStubBuilder WithBehaviors(params IBehavior[] behaviors) {
         return new PropertyStubBuilder().WithBehaviors(behaviors);
      }

      public static PropertyStub<T> Of<T>() {
         return new PropertyStubBuilder().Of<T>();
      }
   }

   public class PropertyStubBuilder {
      private string _name = "Test property";
      private List<IBehavior> _behaviors = new List<IBehavior>();

      public PropertyStubBuilder Named(string name) {
         _name = name;
         return this;
      }

      public PropertyStubBuilder WithBehaviors(params IBehavior[] behaviors) {
         _behaviors.AddRange(behaviors);
         return this;
      }

      public PropertyStub<T> Of<T>() {
         return new PropertyStub<T>(_name, _behaviors);
      }
   }
}
