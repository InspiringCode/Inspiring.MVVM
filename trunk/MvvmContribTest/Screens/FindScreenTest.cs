// NOT IMPLEMENTED YET

//namespace Inspiring.MvvmTest.Screens {
//   using System;
//   using System.Linq;
//   using System.Reflection;
//   using Inspiring.Mvvm.Screens;
//   using Microsoft.VisualStudio.TestTools.UnitTesting;

//   [TestClass]
//   public class FindScreenTest {
//      private ShellScreen _shell = new ShellScreen();
//      private ScreenConductor _mainConductor;
//      private ScreenConductor _dockConductor;
//      private PersonScreen _firstPersonScreen;
//      private PersonScreen _secondPersonScreen;
//      private ScreenConductor _firstPersonConductor;
//      private ScreenConductor _secondPersonConductor;
//      private PersonInfoScreen _firstInfoScreen;
//      private PersonInfoScreen _secondInfoScreen;
//      private ToolboxScreen _toolboxScreen;

//      [TestInitialize]
//      public void Setup() {
//         _shell = new ShellScreen();
//         _shell.SetupForTest();

//         _mainConductor = _shell.MainConductor;
//         _dockConductor = _shell.DockConductor;
//         _firstPersonScreen = (PersonScreen)_mainConductor.Screens.ElementAt(0);
//         _secondPersonScreen = (PersonScreen)_mainConductor.Screens.ElementAt(1);
//         _firstPersonConductor = _firstPersonScreen.PersonConductor;
//         _secondPersonConductor = _secondPersonScreen.PersonConductor;
//         _firstInfoScreen = (PersonInfoScreen)_firstPersonConductor.Screens.Single();
//         _secondInfoScreen = (PersonInfoScreen)_secondPersonConductor.Screens.Single();
//         _toolboxScreen = (ToolboxScreen)_dockConductor.Screens.Single();
//      }

//      [TestMethod]
//      public void CheckRootCanFindItself() {
//         Assert.AreEqual(_shell, _shell.FindScreen<ShellScreen>());
//      }

//      [TestMethod]
//      public void CheckLeafCanFindRoot() {
//         Assert.AreEqual(_shell, _firstInfoScreen.FindScreen<ShellScreen>());
//         Assert.AreEqual(_shell, _firstInfoScreen.FindScreen<ShellScreen>("Shell"));
//         Assert.AreEqual(_shell, _firstInfoScreen.FindScreen<ILifecycleHandler>("Shell"));
//      }

//      [TestMethod]
//      public void CheckFindParentConductors() {
//         Assert.AreEqual(_firstPersonConductor, _firstInfoScreen.FindScreen<ILifecycleHandler>("Persons"));
//         Assert.AreEqual(_secondPersonConductor, _secondInfoScreen.FindScreen<ILifecycleHandler>("Persons"));
//      }

//      [TestMethod]
//      public void CheckFindAncestorConductor() {
//         Assert.AreEqual(_mainConductor, _firstInfoScreen.FindScreen<ILifecycleHandler>("Main"));
//         Assert.AreEqual(_mainConductor, _secondInfoScreen.FindScreen<ILifecycleHandler>("Main"));
//      }

//      [TestMethod]
//      public void CheckFindRelatedScreen() {
//         Assert.AreEqual(_toolboxScreen, _secondInfoScreen.FindScreen<ToolboxScreen>());
//         Assert.AreEqual(_toolboxScreen, _secondInfoScreen.FindScreen<ILifecycleHandler>("Toolbox"));
//         Assert.AreEqual(_firstInfoScreen, _firstPersonScreen.FindScreen<PersonInfoScreen>());
//      }

//      [TestMethod]
//      public void OpenScreen() {
//         Assert.AreEqual(1, _firstPersonConductor.Screens.Count());
//         _firstInfoScreen.OpenScreen(ScreenFactory.For<PersonInfoScreen>());
//         Assert.AreEqual(2, _firstPersonConductor.Screens.Count());

//         Assert.AreEqual(1, _dockConductor.Screens.Count());
//         _secondInfoScreen.OpenScreen(ScreenFactory.For<PersonInfoScreen>(), "Dock");
//         Assert.AreEqual(2, _dockConductor.Screens.Count());

//         Assert.AreEqual(1, _secondPersonConductor.Screens.Count());
//         _secondPersonConductor.OpenScreen(ScreenFactory.For<PersonInfoScreen>());
//         Assert.AreEqual(2, _secondPersonConductor.Screens.Count());
//      }

//      private class ShellScreen : Screen, IIdentifiedScreen {
//         public ScreenConductor MainConductor { get; private set; }
//         public ScreenConductor DockConductor { get; private set; }

//         public void SetupForTest() {
//            OnInitialize();
//         }

//         protected override void OnInitialize() {
//            base.OnInitialize();
//            MainConductor = Children.AddNew(ScreenFactory
//               .WithSubject(new ScreenConductorSubject("Main"))
//               .For<ScreenConductor>()
//            );
//            DockConductor = Children.AddNew(ScreenFactory
//               .WithSubject(new ScreenConductorSubject("Dock"))
//               .For<ScreenConductor>()
//            );

//            MainConductor.OpenScreen(ScreenFactory.For<PersonScreen>());
//            MainConductor.OpenScreen(ScreenFactory.For<PersonScreen>());

//            DockConductor.OpenScreen(ScreenFactory.For<ToolboxScreen>());
//         }

//         public object ScreenId {
//            get { return "Shell"; }
//         }
//      }

//      private class PersonScreen : Screen {
//         public ScreenConductor PersonConductor { get; private set; }

//         protected override void OnInitialize() {
//            base.OnInitialize();
//            PersonConductor = Children.AddNew(ScreenFactory
//               .WithSubject(new ScreenConductorSubject("Persons"))
//               .For<ScreenConductor>()
//            );

//            PersonConductor.OpenScreen(ScreenFactory.For<PersonInfoScreen>());
//         }
//      }

//      private class PersonInfoScreen : Screen {

//      }

//      private class ToolboxScreen : Screen, IIdentifiedScreen {
//         public object ScreenId {
//            get { return "Toolbox"; }
//         }
//      }
//   }


//   public static class ScreenTestExtensions {
//      public static TScreen FindScreen<TScreen>(this Screen screen, object screenId = null)
//         where TScreen : ILifecycleHandler {
//         return (TScreen)typeof(Screen).GetMethod(
//            "FindScreen",
//            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
//            null,
//            new Type[] { typeof(Object) },
//            null
//         )
//         .MakeGenericMethod(typeof(TScreen))
//         .Invoke(screen, new object[] { screenId });
//      }
//   }
//}
