using System;
using Inspiring.Mvvm.ViewModels.Behaviors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.Common {
   [TestClass]
   public class BehaviorsTest {
      private VMPropertyBehavior _first;
      private VMPropertyBehavior _second;
      private VMPropertyBehavior _third;
      private VMPropertyBehavior _fourth;

      [TestInitialize]
      public void TestInitialize() {
         _first = new FirstBehavior();
         _second = new SecondBehavior();
         _third = new ThirdBehavior();
         _fourth = new FourthBehavior();
      }

      [TestMethod]
      public void Insert() {
         //BehaviorChain chain = new BehaviorChain();
         //AssertChainTypes(chain);

         //chain.PrependBehavior(new FirstBehavior());
         //AssertChainTypes(chain, typeof(FirstBehavior));

         //chain.PrependBehavior(new SecondBehavior());
         //AssertChainTypes(chain, typeof(SecondBehavior), typeof(FirstBehavior));
      }

      [TestMethod]
      public void InsertAfter() {
         //BehaviorChain chain = new BehaviorChain();
         //Assert.IsNull(chain.Successor);

         //chain.InsertBehaviorAfter<BehaviorChain>(new FirstBehavior());
         //AssertChainTypes(chain, typeof(FirstBehavior));

         //chain.InsertBehaviorAfter<FirstBehavior>(new SecondBehavior());
         //AssertChainTypes(chain, typeof(FirstBehavior), typeof(SecondBehavior));

         //chain.InsertBehaviorAfter<IFirstBehavior>(new ThirdBehavior());
         //AssertChainTypes(chain, typeof(FirstBehavior), typeof(ThirdBehavior), typeof(SecondBehavior));

         //chain.InsertBehaviorAfter<SecondBehavior>(new FourthBehavior());
         //AssertChainTypes(chain, typeof(FirstBehavior), typeof(ThirdBehavior), typeof(SecondBehavior), typeof(FourthBehavior));

      }

      [TestMethod]
      public void InsertBefore() {
         //BehaviorChain chain = new BehaviorChain();
         //Assert.IsNull(chain.Successor);

         //AssertHelper.Throws<ArgumentException>(() =>
         //   chain.InsertBehaviorBefore<BehaviorChain>(new FirstBehavior())
         //);

         //chain.PrependBehavior(new FirstBehavior());

         //chain.InsertBehaviorBefore<IFirstBehavior>(new SecondBehavior());
         //AssertChainTypes(chain, typeof(SecondBehavior), typeof(FirstBehavior));

         //chain.InsertBehaviorBefore<SecondBehavior>(new ThirdBehavior());
         //AssertChainTypes(chain, typeof(ThirdBehavior), typeof(SecondBehavior), typeof(FirstBehavior));

         //chain.InsertBehaviorBefore<FirstBehavior>(new FourthBehavior());
         //AssertChainTypes(chain, typeof(ThirdBehavior), typeof(SecondBehavior), typeof(FourthBehavior), typeof(FirstBehavior));
      }

      [TestMethod]
      public void AppendBehavior() {
         //BehaviorChain chain = new BehaviorChain();
         //Assert.IsNull(chain.Successor);

         //chain.AppendBehavior(_first);
         //AssertChain(chain, _first);

         //chain.AppendBehavior(_second);
         //AssertChain(chain, _first, _second);

         //chain.AppendBehavior(_third);
         //AssertChain(chain, _first, _second, _third);
      }

      [TestMethod]
      public void GetNextBehavior() {
         //BehaviorChain chain = new BehaviorChain();

         //chain.PrependBehavior(_fourth);
         //chain.PrependBehavior(_second);
         //chain.PrependBehavior(_first);

         //Assert.AreEqual(_first, chain.GetNextBehavior<IFirstBehavior>());
         //Assert.AreEqual(_first, chain.GetNextBehavior<FirstBehavior>());
         //Assert.AreEqual(_second, chain.GetNextBehavior<ISecondBehavior>());
         //Assert.AreEqual(_second, chain.GetNextBehavior<SecondBehavior>());
         //Assert.AreEqual(_fourth, chain.GetNextBehavior<FourthBehavior>());

         //AssertHelper.Throws<ArgumentException>(() => chain.GetNextBehavior<ThirdBehavior>());
         //AssertHelper.Throws<ArgumentException>(() => chain.GetNextBehavior<BehaviorChain>());

         //IFirstBehavior firstResult;
         //Assert.IsTrue(chain.TryGetBehavior<IFirstBehavior>(out firstResult));
         //Assert.AreEqual(_first, firstResult);

         //FourthBehavior fourthResult;
         //Assert.IsTrue(chain.TryGetBehavior<FourthBehavior>(out fourthResult));
         //Assert.AreEqual(_fourth, fourthResult);

         //ThirdBehavior thirdResult;
         //Assert.IsFalse(chain.TryGetBehavior<ThirdBehavior>(out thirdResult));
         //Assert.AreEqual(null, thirdResult);

         //BehaviorChain chainResult;
         //Assert.IsFalse(chain.TryGetBehavior<BehaviorChain>(out chainResult));
         //Assert.AreEqual(null, chainResult);
      }

      private void AssertChainTypes(VMPropertyBehaviorChain chain, params Type[] behaviorTypes) {
         IBehavior behavior = chain.Successor;

         foreach (Type type in behaviorTypes) {
            Assert.IsNotNull(behavior, "Behavior {0} was not found in the chain.", type);
            Assert.IsInstanceOfType(behavior, type);
            behavior = behavior.Successor;
         }

         Assert.IsNull(behavior, "Chain contains more behaviors than expected.");
      }

      private void AssertChain(VMPropertyBehaviorChain chain, params VMPropertyBehavior[] behaviors) {
         IBehavior actual = chain.Successor;

         foreach (VMPropertyBehavior expected in behaviors) {
            Assert.IsNotNull(actual, "Behavior {0} was not found in the chain.", expected);
            Assert.AreSame(actual, expected, "Expected behavior {0} but found behavior {1}.", expected, actual);
            actual = actual.Successor;
         }

         Assert.IsNull(actual, "Chain contains more behaviors than expected.");
      }

      private interface IFirstBehavior {

      }

      private interface ISecondBehavior {
         int ReturnFive();
      }

      private class FirstBehavior : VMPropertyBehavior, IFirstBehavior {

      }

      private class SecondBehavior : VMPropertyBehavior, ISecondBehavior {
         public int ReturnFive() {
            return 5;
         }
      }

      private class ThirdBehavior : VMPropertyBehavior {

      }

      private class FourthBehavior : VMPropertyBehavior {

      }
   }
}
