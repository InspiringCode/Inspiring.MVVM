namespace Inspiring.MvvmTest.Views {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.Views;
   using Inspiring.MvvmTest.Screens;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;
   using Moq.Protected;

   [TestClass]
   public class ScreenConductorAdapterTest {
      private IScreen _firstScreen;
      private IScreen _secondScreen;
      private IScreenFactory<IScreen> _firstFactory;
      private IScreenFactory<IScreen> _secondFactory;
      private object _firstView;
      private object _secondView;

      [TestInitialize]
      public void Setup() {
         _firstScreen = MockObjectFactory.MockScreen();
         _secondScreen = MockObjectFactory.MockScreen();
         _firstFactory = MockObjectFactory.MockScreenFactory(_firstScreen).Object;
         _secondFactory = MockObjectFactory.MockScreenFactory(_secondScreen).Object;
         _firstView = MockObjectFactory.MockView();
         _secondView = MockObjectFactory.MockView();
      }

      [TestMethod]
      public void AddTwoScreens() {
         List<object> expectedViews = new List<object>();
         ScreenConductor conductor = new ScreenConductor();
         var adapterMock = new Mock<ScreenConductorAdapter>(conductor) { CallBase = true };
         var adapter = adapterMock.Object;

         Assert.AreEqual(null, adapter.ActiveView);
         Assert.IsFalse(adapter.Views.Cast<object>().Any());

         expectedViews.Add(_firstView);
         SetupServiceLocator(_firstView);
         conductor.OpenScreen(_firstFactory);

         Assert.AreEqual(_firstView, adapter.ActiveView);
         CollectionAssert.AreEquivalent(expectedViews, adapter.Views.Cast<object>().ToArray());
         adapterMock.Protected().Verify("OnViewAdded", Times.Once(), _firstView);
         adapterMock.Protected().Verify("OnActiveViewChanged", Times.Once(), _firstView);

         expectedViews.Add(_secondView);
         SetupServiceLocator(_secondView);
         conductor.OpenScreen(_secondFactory);

         Assert.AreEqual(_secondView, adapter.ActiveView);
         CollectionAssert.AreEquivalent(expectedViews, adapter.Views.Cast<object>().ToArray());
         adapterMock.Protected().Verify("OnViewAdded", Times.Once(), _secondView);
         adapterMock.Protected().Verify("OnActiveViewChanged", Times.Once(), _secondView);
      }

      [TestMethod]
      public void InitializeWithTwoScreens() {
         List<object> expectedViews = new List<object> { _firstView, _secondView };
         ScreenConductor conductor = new ScreenConductor();
         conductor.OpenScreen(_firstFactory);
         conductor.OpenScreen(_secondFactory);

         SetupServiceLocator(_firstView, _secondView);
         var adapterMock = new Mock<ScreenConductorAdapter>(conductor) { CallBase = true };
         var adapter = adapterMock.Object;

         CollectionAssert.AreEquivalent(expectedViews, adapter.Views.Cast<object>().ToArray());
         Assert.AreEqual(_secondView, adapter.ActiveView);
         adapterMock.Protected().Verify("OnViewAdded", Times.Once(), _firstView);
         adapterMock.Protected().Verify("OnViewAdded", Times.Once(), _secondView);
         adapterMock.Protected().Verify("OnActiveViewChanged", Times.Once(), _secondView);
      }

      [TestMethod]
      public void RemoveTwoScreens() {
         List<object> expectedViews = new List<object> { _firstView, _secondView };
         ScreenConductor conductor = new ScreenConductor();
         conductor.OpenScreen(_firstFactory);
         conductor.OpenScreen(_secondFactory);

         SetupServiceLocator(_firstView, _secondView);
         var adapterMock = new Mock<ScreenConductorAdapter>(conductor) { CallBase = true };
         var adapter = adapterMock.Object;

         expectedViews.Remove(_secondView);
         conductor.CloseScreen(_secondScreen);

         Assert.AreEqual(_firstView, adapter.ActiveView);
         CollectionAssert.AreEquivalent(expectedViews, adapter.Views.Cast<object>().ToArray());
         adapterMock.Protected().Verify("OnViewRemoved", Times.Once(), _secondView);
         adapterMock.Protected().Verify("OnActiveViewChanged", Times.Once(), _firstView);

         expectedViews.Remove(_firstView);
         conductor.CloseScreen(_firstScreen);

         Assert.AreEqual(null, adapter.ActiveView);
         CollectionAssert.AreEquivalent(expectedViews, adapter.Views.Cast<object>().ToArray());
         adapterMock.Protected().Verify("OnViewRemoved", Times.Once(), _firstView);
         adapterMock.Protected().Verify("OnActiveViewChanged", Times.Once(), ItExpr.IsNull<object>());
      }

      [TestMethod]
      public void SetActiveView() {
         List<object> expectedViews = new List<object> { _firstView, _secondView };
         ScreenConductor conductor = new ScreenConductor();
         conductor.OpenScreen(_firstFactory);
         conductor.OpenScreen(_secondFactory);

         SetupServiceLocator(_firstView, _secondView);
         var adapterMock = new Mock<ScreenConductorAdapter>(conductor) { CallBase = true };
         var adapter = adapterMock.Object;

         Assert.AreEqual(_secondView, adapter.ActiveView);
         Assert.AreEqual(_secondScreen, conductor.ActiveScreen);
         adapter.ActiveView = _firstView;
         Assert.AreEqual(_firstView, adapter.ActiveView);
         Assert.AreEqual(_firstScreen, conductor.ActiveScreen);
      }

      [TestMethod]
      public void CloseScreen() {
         List<object> expectedViews = new List<object> { _firstView, _secondView };
         ScreenConductor conductor = new ScreenConductor();
         conductor.OpenScreen(_firstFactory);
         conductor.OpenScreen(_secondFactory);

         SetupServiceLocator(_firstView, _secondView);
         var adapterMock = new Mock<ScreenConductorAdapter>(conductor) { CallBase = true };
         var adapter = adapterMock.Object;

         Assert.AreEqual(2, conductor.Screens.Count());
         adapter.CloseView(_firstView);
         Assert.AreEqual(1, conductor.Screens.Count());
      }

      private void SetupServiceLocator(params object[] viewsToReturnInSeqence) {
         int index = 0;
         var mock = new Mock<IServiceLocator>();
         mock.Setup(x => x.TryGetInstance(It.IsAny<Type>())).Returns<Type>(x => {
            return viewsToReturnInSeqence[index++];
         });

         ServiceLocator.SetServiceLocator(mock.Object);
      }
   }
}
