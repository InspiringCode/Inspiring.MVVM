using Inspiring.MvvmTest.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Sequences;

namespace Inspiring.MvvmTest {
   //public interface IBuilder<out T> {

   //}

   //public static class ExtensionsTest {
   //   public static IBuilder<IViewModel<T>> Test<TDesc, T>(this IBuilder<IViewModel<TDesc>> b, Func<TDesc, IVMPropertyDescriptor<IViewModel<T>>> selector) where TDesc : IVMDescriptor {
   //      return null;
   //   }

   //   public static IBuilder<TC> Test<T, TC>(this IBuilder<T> b, Func<T, TC> selector) {
   //      return null;
   //   }
   //}

   [TestClass]
   public class UnitTest1 : TestBase {
      //[TestMethod]
      //public void MyTestMethod() {
      //   IBuilder<IViewModel<Inspiring.MvvmTest.ApiTests.ViewModels.Undo.ProjectVMDescriptor>> foo = null;
      //   IBuilder<Binding> bar = null;

      //   foo.Test(x => x.Customer);




      //}


      [TestMethod]
      public void ThisShouldWork2() {
         var a = new Mock<IFoo>(MockBehavior.Strict);

         using (Sequence.Create()) {
            a.Setup(x => x.M(100)).InSequence().Returns(101);
            a.Setup(x => x.M(200)).InSequence().Returns(201);

            a.Object.M(100);
            a.Object.M(200);
         }
      }

      [TestMethod]
      [ExpectedException(typeof(SequenceException))]
      public void ThisShouldNotWork2() {
         var a = new Mock<IFoo>(MockBehavior.Strict);

         using (Sequence.Create()) {
            a.Setup(x => x.M(100)).InSequence().Returns(101);
            a.Setup(x => x.N(100)).InSequence().Returns(201);

            a.Object.N(100);
            a.Object.M(100);
         }
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
