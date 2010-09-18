using System.Collections.Generic;
using Inspiring.Mvvm.ViewModels.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.ViewModels.Core.Behaviors {
   [TestClass]
   public class BehaviorConfiguration2Test {
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

         InsertAndEnable(conf, VMBehaviorKey.CustomOne, _factory1, RelativePosition.After, VMBehaviorKey.Last);
         conf.OverrideFactory(VMBehaviorKey.CustomOne, _factory1);
         conf.Enable(VMBehaviorKey.CustomOne);
         AssertChain(conf, _behavior1);

         InsertAndEnable(conf, VMBehaviorKey.CustomTwo, _factory2, RelativePosition.After, VMBehaviorKey.Last);
         AssertChain(conf, _behavior1, _behavior2);

         InsertAndEnable(conf, VMBehaviorKey.CustomThree, _factory3, RelativePosition.After, VMBehaviorKey.CustomTwo);
         AssertChain(conf, _behavior1, _behavior2, _behavior3);

         InsertAndEnable(conf, VMBehaviorKey.CustomFour, _factory4, RelativePosition.After, VMBehaviorKey.CustomOne);
         AssertChain(conf, _behavior1, _behavior4, _behavior2, _behavior3);
      }

      [TestMethod]
      public void InsertBefore() {
         BehaviorConfiguration conf = new BehaviorConfiguration();

         InsertAndEnable(conf, VMBehaviorKey.CustomOne, _factory1, RelativePosition.Before, VMBehaviorKey.First);
         AssertChain(conf, _behavior1);

         InsertAndEnable(conf, VMBehaviorKey.CustomTwo, _factory2, RelativePosition.Before, VMBehaviorKey.First);
         AssertChain(conf, _behavior2, _behavior1);

         InsertAndEnable(conf, VMBehaviorKey.CustomThree, _factory3, RelativePosition.Before, VMBehaviorKey.CustomTwo);
         AssertChain(conf, _behavior3, _behavior2, _behavior1);

         InsertAndEnable(conf, VMBehaviorKey.CustomFour, _factory4, RelativePosition.Before, VMBehaviorKey.CustomOne);
         AssertChain(conf, _behavior3, _behavior2, _behavior4, _behavior1);
      }

      [TestMethod]
      public void ReplaceBehaviors() {
         BehaviorConfiguration template = new BehaviorConfiguration();
         InsertAndEnable(template, VMBehaviorKey.CustomOne, _factory1, RelativePosition.Before, VMBehaviorKey.First);

         template.Insert(VMBehaviorKey.CustomTwo, RelativePosition.After, VMBehaviorKey.Last);
         template.OverrideFactory(VMBehaviorKey.CustomTwo, _factory2);

         InsertAndEnable(template, VMBehaviorKey.CustomThree, _factory3, RelativePosition.After, VMBehaviorKey.Last);

         var conf = template.Clone();
         AssertChain(conf, _behavior1, _behavior3);

         conf.OverrideFactory(VMBehaviorKey.CustomOne, _factory4);
         conf.Enable(VMBehaviorKey.CustomTwo);
         conf.OverrideFactory(VMBehaviorKey.CustomThree, _factory5);
         AssertChain(conf, _behavior4, _behavior2, _behavior5);

         var mergeFrom = new BehaviorConfiguration();
         InsertAndEnable(mergeFrom, VMBehaviorKey.CustomFour, _factory7, RelativePosition.Before, VMBehaviorKey.CustomTwo);
         mergeFrom.OverrideFactory(VMBehaviorKey.CustomTwo, _factory6);

         conf.MergeFrom(mergeFrom);
         AssertChain(conf, _behavior4, _behavior7, _behavior6, _behavior5);
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

      private static void InsertAndEnable(
         BehaviorConfiguration conf,
         VMBehaviorKey behaviorKey,
         IBehaviorFactory factory,
         RelativePosition relativeTo,
         VMBehaviorKey position
      ) {
         conf.Insert(behaviorKey, relativeTo, position);
         conf.OverrideFactory(behaviorKey, factory);
         conf.Enable(behaviorKey);
      }

   }
}
