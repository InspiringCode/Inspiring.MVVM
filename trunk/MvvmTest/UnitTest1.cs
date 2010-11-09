using System;
using Inspiring.Mvvm.ViewModels.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Inspiring.MvvmTest {
   [TestClass]
   public class UnitTest1 {

      [TestMethod]
      public void MyTestMethod() {
         DisplayValueAccessorBehavior<int> b = null;
         //b.CallNext(x => x.SetValue(null, null));
         //b.CallNext(x => x.GetValue(null));


         Action<int> action = x => x++;
         Console.WriteLine(action);

         action = Action1;
         Console.WriteLine(action);

         action = Action2;
         Console.WriteLine(action);
      }

      private void Action1(int x) {

      }

      private static void Action2(int x) {

      }

      // [TestMethod]
      public void ThisShouldWork() {
         var a = new Mock<IFoo>(MockBehavior.Strict);

         MockSequence t = new MockSequence();
         a.InSequence(t).Setup(x => x.M(100)).Returns(101);
         a.InSequence(t).Setup(x => x.M(200)).Returns(201);

         a.Object.M(100);
         a.Object.M(200);
      }

      // [TestMethod]
      public void ThisShouldNotWork() {
         var a = new Mock<IFoo>(MockBehavior.Strict);

         MockSequence t = new MockSequence();
         a.InSequence(t).Setup(x => x.M(100)).Returns(101);
         a.InSequence(t).Setup(x => x.N(100)).Returns(201);

         a.Object.N(100);
         a.Object.M(100);
      }

      public interface IFoo {
         int M(int p);
         int N(int p);
      }
   }
}
