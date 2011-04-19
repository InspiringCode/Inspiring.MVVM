using Inspiring.Mvvm.Screens;
using Inspiring.Mvvm.ViewModels;
using Inspiring.MvvmTest.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Sequences;

namespace Inspiring.MvvmTest {
   [TestClass]
   public class UnitTest1 : TestBase {
      private class A { }

      private class B : A { }

      private class Base<T> where T : A {

      }

      private class Sub<T> : Base<T> where T : B {

      }

      public interface IScreen {
      }

      public abstract class Screen<TDescriptor> : ViewModelScreenBase<TDescriptor>, IScreen where TDescriptor : ScreenDescriptor {

      }

      public class ScreenDescriptor : VMDescriptor {

      }

      //private class PropBehFac: IBehaviorFactory {

      //   protected virtual IBehFac CreateFactory<T1, T2>() {
      //      return new PropBehImpl<T1, T2>();
      //   }

      //   private class PropBehFacInvoker<T1, T2> : BehaviorFactoryInvoker {
      //      public IBehFac CreateFactory(IBehaviorFactory factory) {
      //         return ((PropBehFac)factory).CreateFactory<T1, T2>();
      //      }

      //      public override IBehavior Invoke(IBehaviorFactory factory, BehaviorKey behaviorToCreate) {
      //         //return new XyzFactoryImpl<T1, T2>().Create(behaviorToCreate);
      //         return null;
      //      }
      //   }
      //}

      //private class PropBehFacImpl<T1, T2> : IBehFac {
      //   public IBehavior Create(BehaviorKey key) { return null; }
      //}

      //private interface IBehFac {
      //   IBehavior Create(BehaviorKey key);
      //}


      //private class DerivedPropBehFac : PropBehFac {
      //   protected override IBehFac CreateFactory<T1, T2>() {
      //      return base.CreateFactory<T1, T2>();
      //   }

      //   private class DerivedPropBehFacImp<T1, T2> : PropBehFacImpl<T1, T2> {
      //      // override create or add registrations...
      //   }
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
