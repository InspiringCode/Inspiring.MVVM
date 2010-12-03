using Inspiring.Mvvm.ViewModels;
using Inspiring.Mvvm.ViewModels.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.ViewModels.Behaviors {
   [TestClass]
   public class VMPropertyTest {
      [TestMethod]
      public void TestMethod1() {
         EmployeeVM vm = new EmployeeVM();
         AccessPropertyBehaviorFake<string> source = new AccessPropertyBehaviorFake<string>();
         //VMProperty<string> p = new VMProperty<string>(source);

         //Assert.AreEqual(null, p.GetValue(vm));
         //source.Value = "Value 1";
         //Assert.AreEqual("Value 1", p.GetValue(vm));
         //p.SetValue(vm, "Value 2");
         //Assert.AreEqual("Value 2", source.Value);
      }

      private class AccessPropertyBehaviorFake<T> : Behavior, IValueAccessorBehavior<T> {
         public T Value { get; set; }

         public T GetValue(IBehaviorContext vm, ValueStage stage) {
            return Value;
         }

         public void SetValue(IBehaviorContext vm, T value) {
            Value = value;
         }
      }
   }
}
