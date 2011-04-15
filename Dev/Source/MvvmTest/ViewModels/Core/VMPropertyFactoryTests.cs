namespace Inspiring.MvvmTest.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Core.Behaviors;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class VMPropertyFactoryTests : TestBase {
      private static BehaviorKey TestKey = new BehaviorKey("TestKey");
      private VMDescriptorConfiguration Config { get; set; }
      private VMPropertyFactory<IViewModel, object> Factory { get; set; }


      [TestInitialize]
      public void Setup() {
         Config = new VMDescriptorConfiguration(new BehaviorChainConfiguration());
         Factory = new VMPropertyFactory<IViewModel, object>(Config, null);
      }

      [TestMethod]
      public void CreatePropertyWithoutSource_UsesCorrectTemplateAndInsertsAccessor() {
         var factoryProvider = new FactoryProviderMock();

         BehaviorChainTemplateRegistry.RegisterTemplate(
            DefaultBehaviorChainTemplateKeys.Property,
            new BehaviorChainTemplate(factoryProvider)
               .Append(TestKey)
               .Append(PropertyBehaviorKeys.ValueAccessor, DefaultBehaviorState.DisabledWithoutFactory)
         );

         factoryProvider.BehaviorForProperty = Mock<IBehavior>();
         var expectedValueAccessor = Mock<IValueAccessorBehavior<string>>();

         var property = Factory.Property<string>(expectedValueAccessor);

         AssertBehavior(
            property,
            PropertyBehaviorKeys.ValueAccessor,
            actualAccessor => Assert.AreEqual(expectedValueAccessor, actualAccessor)
         );

         AssertBehavior(
            property,
            TestKey,
            actualBehavior => Assert.AreEqual(factoryProvider.BehaviorForProperty, actualBehavior)
         );
      }

      [TestCleanup]
      public void Cleanup() {
         BehaviorChainTemplateRegistry.ResetToDefaults();
      }


      private void AssertBehavior(IVMPropertyDescriptor property, BehaviorKey key, Action<IBehavior> assertion) {
         AssertBehavior<IBehavior>(property, key, assertion);
      }

      private void AssertBehavior<TBehavior>(IVMPropertyDescriptor property, BehaviorKey key, Action<TBehavior> assertion) where TBehavior : IBehavior {
         Assert.IsNotNull(property);

         Config
            .PropertyConfigurations[property]
            .ConfigureBehavior<TBehavior>(key, assertion);
      }

      private class FactoryProviderMock : SimplePropertyProvider {
         public IBehavior BehaviorForProperty { get; set; }
         public IBehavior BehaviorForPropertyWithSource { get; set; }

         public override BehaviorFactory GetFactory<TOwnerVM, TValue, TSourceObject>() {
            return BehaviorForProperty != null ?
               new BehaviorFactory().RegisterBehavior(TestKey, () => BehaviorForProperty) :
               new BehaviorFactory();
         }

         public override BehaviorFactory GetFactoryForPropertyWithSource<TOwnerVM, TValue, TSourceObject>() {
            return BehaviorForPropertyWithSource != null ?
               new BehaviorFactory().RegisterBehavior(TestKey, () => BehaviorForPropertyWithSource) :
               new BehaviorFactory();
         }
      }

      //private class TestBehavior : Behavior { }
   }
}