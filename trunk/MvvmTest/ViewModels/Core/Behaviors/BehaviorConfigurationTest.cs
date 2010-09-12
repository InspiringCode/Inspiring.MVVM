using System.Collections.Generic;
using Inspiring.Mvvm.ViewModels.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.ViewModels.Core.Behaviors {
   [TestClass]
   public class BehaviorConfigurationTest {
      private IBehavior _behavior1;
      private IBehavior _behavior2;
      private IBehavior _behavior3;
      private IBehavior _behavior4;
      private IBehavior _behavior5;
      private IBehavior _behavior6;
      private IBehavior _behavior7;
      private IBehavior _behavior8;
      private IBehaviorFactory _factory1;
      private IBehaviorFactory _factory2;
      private IBehaviorFactory _factory3;
      private IBehaviorFactory _factory4;
      private IBehaviorFactory _factory5;
      private IBehaviorFactory _factory6;
      private IBehaviorFactory _factory7;
      private IBehaviorFactory _factory8;

      [TestInitialize]
      public void Setup() {
         _behavior1 = new BehaviorFake();
         _behavior2 = new BehaviorFake();
         _behavior3 = new BehaviorFake();
         _behavior4 = new BehaviorFake();
         _behavior5 = new BehaviorFake();
         _behavior6 = new BehaviorFake();
         _behavior7 = new BehaviorFake();
         _behavior8 = new BehaviorFake();
         _factory1 = new BehaviorFactoryFake(_behavior1);
         _factory2 = new BehaviorFactoryFake(_behavior2);
         _factory3 = new BehaviorFactoryFake(_behavior3);
         _factory4 = new BehaviorFactoryFake(_behavior4);
         _factory5 = new BehaviorFactoryFake(_behavior5);
         _factory6 = new BehaviorFactoryFake(_behavior6);
         _factory7 = new BehaviorFactoryFake(_behavior7);
         _factory8 = new BehaviorFactoryFake(_behavior8);
      }

      [TestMethod]
      public void InsertAfter() {
         BehaviorConfiguration conf = new BehaviorConfiguration();

         conf.Add(VMBehaviorKey.CustomOne, _factory1, BehaviorOrderModifier.After, VMBehaviorKey.Last);
         AssertChain(conf, _behavior1);

         conf.Add(VMBehaviorKey.CustomTwo, _factory2, BehaviorOrderModifier.After, VMBehaviorKey.Last);
         AssertChain(conf, _behavior1, _behavior2);

         conf.Add(VMBehaviorKey.CustomThree, _factory3, BehaviorOrderModifier.After, VMBehaviorKey.CustomTwo);
         AssertChain(conf, _behavior1, _behavior2, _behavior3);

         conf.Add(VMBehaviorKey.CustomFour, _factory4, BehaviorOrderModifier.After, VMBehaviorKey.CustomOne);
         AssertChain(conf, _behavior1, _behavior4, _behavior2, _behavior3);
      }

      [TestMethod]
      public void InsertBefore() {
         BehaviorConfiguration conf = new BehaviorConfiguration();

         conf.Add(VMBehaviorKey.CustomOne, _factory1, BehaviorOrderModifier.Before, VMBehaviorKey.First);
         AssertChain(conf, _behavior1);

         conf.Add(VMBehaviorKey.CustomTwo, _factory2, BehaviorOrderModifier.Before, VMBehaviorKey.First);
         AssertChain(conf, _behavior2, _behavior1);

         conf.Add(VMBehaviorKey.CustomThree, _factory3, BehaviorOrderModifier.Before, VMBehaviorKey.CustomTwo);
         AssertChain(conf, _behavior3, _behavior2, _behavior1);

         conf.Add(VMBehaviorKey.CustomFour, _factory4, BehaviorOrderModifier.Before, VMBehaviorKey.CustomOne);
         AssertChain(conf, _behavior3, _behavior2, _behavior4, _behavior1);
      }

      [TestMethod]
      public void ReplaceBehaviors() {
         BehaviorConfiguration template = new BehaviorConfiguration();
         template.Add(VMBehaviorKey.CustomOne, _factory1, BehaviorOrderModifier.Before, VMBehaviorKey.First);
         template.Add(VMBehaviorKey.CustomTwo, _factory2, BehaviorOrderModifier.After, VMBehaviorKey.Last);
         template.Add(VMBehaviorKey.CustomThree, _factory3, BehaviorOrderModifier.After, VMBehaviorKey.Last);

         var conf = template.Clone();
         AssertChain(conf, _behavior1, _behavior2, _behavior3);

         conf.Override(VMBehaviorKey.CustomOne, _factory4);
         conf.OverridePermanently(VMBehaviorKey.CustomTwo, _factory5);
         conf.OverridePermanently(VMBehaviorKey.CustomThree, _factory6);
         AssertChain(conf, _behavior4, _behavior5, _behavior6);

         var replaceWith = new BehaviorConfiguration();
         replaceWith.Add(VMBehaviorKey.CustomThree, _factory3, BehaviorOrderModifier.After, VMBehaviorKey.Last);
         replaceWith.Add(VMBehaviorKey.CustomOne, _factory7, BehaviorOrderModifier.After, VMBehaviorKey.Last);
         replaceWith.Add(VMBehaviorKey.CustomFour, _factory8, BehaviorOrderModifier.After, VMBehaviorKey.Last);
         AssertChain(replaceWith, _behavior3, _behavior7, _behavior8);

         conf.ReplaceBehaviors(replaceWith);
         AssertChain(conf, _behavior6, _behavior7, _behavior8);
      }

      private void AssertChain(BehaviorConfiguration configuration, params IBehavior[] expected) {
         Behavior chain = configuration.CreateBehaviorChain<string>();
         List<IBehavior> list = new List<IBehavior>();

         for (IBehavior b = chain.Successor; b != null; b = b.Successor) {
            list.Add(b);
            Assert.IsTrue(list.Count < 1000, "Endless behavior loop detected.");
         }

         CollectionAssert.AreEqual(expected, list);
      }

      private class BehaviorFactoryFake : IBehaviorFactory {
         private IBehavior _result;

         public BehaviorFactoryFake(IBehavior result) {
            _result = result;
         }

         public IBehavior Create<TValue>() {
            return _result;
         }
      }

      private class BehaviorFake : IBehavior {
         public IBehavior Successor { get; set; }

         public void Initialize(BehaviorInitializationContext context) {
         }
      }
   }
}
