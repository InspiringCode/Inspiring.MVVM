namespace Inspiring.MvvmTest.Screens {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.Screens;
using Inspiring.Mvvm.Common;
using System.Collections.Generic;

   [TestClass]
   public class ScreenLifecycleOperationsTests {
      private EventSubscriptionManager _sm;

      private TestScreen Screen { get; set; }
      private EventAggregator Aggregator { get; set; }
      private ScreenLifecycleOperations Operations { get; set; }
      private List<IEvent> PublishedEvents { get; set; }

      [TestInitialize]
      public void Setup() {
         Aggregator = new EventAggregator();
         Screen = new TestScreen();
         Operations = ScreenLifecycleOperations.For(Aggregator, Screen);
         PublishedEvents = new List<IEvent>();

         _sm = new EventSubscriptionManager(Aggregator);

         AddEventHandlerFor(ScreenEvents.Initialize());
         AddEventHandlerFor(ScreenEvents.Initialize<BaseSubject>());
         AddEventHandlerFor(ScreenEvents.Initialize<DerivedSubject>());
         AddEventHandlerFor(ScreenEvents.Initialize<ISubject>());
         AddEventHandlerFor(ScreenEvents.Activate);
         AddEventHandlerFor(ScreenEvents.Deactivate);
         AddEventHandlerFor(ScreenEvents.RequestClose);
         AddEventHandlerFor(ScreenEvents.Close);
         AddEventHandlerFor(ScreenEvents.LifecycleExceptionOccured);
      }

      [TestMethod]
      public void InitializeWithSubject_PublishesNonGenericInitializeAndGenericInitializeEventsForAllBaseClassesAndInterfacesOfSubject() {
         ISubject subject = new DerivedSubject();

         Operations.Initialize(subject);

         CollectionAssert.AreEquivalent(
            new IEvent[] { 
               ScreenEvents.Initialize(),
               ScreenEvents.Initialize<DerivedSubject>(),
               ScreenEvents.Initialize<BaseSubject>(),
               ScreenEvents.Initialize<ISubject>()
            },
            PublishedEvents
         );
      }

      [TestMethod]
      public void Activate_WhenHandlerThrowsException_RaisesLifecycleExceptionOccuredEventAndThrowsScreenLifecycleException() {
         InvalidOperationException sourceException = new InvalidOperationException();

         AddEventHandlerFor(ScreenEvents.Initialize(), () => { throw sourceException; });

         var actualException = AssertHelper.Throws<ScreenLifecycleException>(() => 
            Operations.Initialize(new BaseSubject())
         );

         CollectionAssert.AreEquivalent(
            new IEvent[] { 
               ScreenEvents.Initialize(),
               ScreenEvents.LifecycleExceptionOccured
            },
            PublishedEvents
         );

         Assert.AreEqual(sourceException, actualException.Exception.InnerException);
      }

      private void AddEventHandlerFor<TArgs>(
         ScreenEvent<TArgs> @event, 
         Action handlerAction = null
      ) where TArgs : ScreenEventArgs {
         handlerAction = handlerAction ?? (() => PublishedEvents.Add(@event));

         _sm.Subscribe(b => {
            b.On(@event, Screen).Execute(args => handlerAction());
         });
      }

      private class TestScreen : ScreenBase { }

      private interface ISubject {
         object Dummy { get; set; }
      }

      private class BaseSubject : ISubject {
         object ISubject.Dummy { get; set; }
      }

      private class DerivedSubject : BaseSubject {
      }
   }
}