namespace Inspiring.MvvmTest.Views {
   using System;
   using System.Windows;
   using System.Windows.Controls;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.Views;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ViewTest : TestBase {
      [TestMethod]
      public void SetModelOnSingleViewInterface() {
         SimpleScreen screen = new SimpleScreen();
         SimpleScreenView view = new SimpleScreenView();
         ContentControl cc = new ContentControl { Content = view };
         View.SetModel(cc, screen);
         Assert.AreEqual(screen, view.Model);
      }

      [TestMethod]
      public void SetModelOnBaseInterfaceView() {
         DerivedScreen screen = new DerivedScreen();
         BaseScreenView view = new BaseScreenView();
         ContentControl cc = new ContentControl { Content = view };
         View.SetModel(cc, screen);
         Assert.AreEqual(screen, view.Model);
      }

      [TestMethod]
      public void SetModelOnMultipleViewInterfaces() {
         DerivedScreen screen = new DerivedScreen();
         MultiScreenView view = new MultiScreenView();
         ContentControl cc = new ContentControl { Content = view };
         View.SetModel(cc, screen);
         Assert.AreEqual(screen, view.DerivedScreenModel);
         Assert.IsNull(view.SimpleScreenModel);
         Assert.IsNull(view.BaseScreenModel);
         Assert.IsNull(view.ObjectModel);
      }

      [TestMethod]
      public void SetModelOnContentControl() {
         SimpleScreenView view = new SimpleScreenView();

         var locator = new Mock<IServiceLocator>(MockBehavior.Strict);
         locator.Setup(x => x.TryGetInstance(typeof(IView<SimpleScreen>))).Returns(view);
         ServiceLocator.SetServiceLocator(locator.Object);

         SimpleScreen screen = new SimpleScreen();
         ContentControl cc = new ContentControl();

         View.SetModel(cc, screen);

         Assert.AreEqual(view, cc.Content);
         Assert.AreEqual(screen, view.Model);
         locator.Verify(x => x.TryGetInstance(typeof(IView<SimpleScreen>)), Times.Once());

      }

      [TestMethod]
      public void SetModelOnContentPresenter() {
         BaseScreenView view = new BaseScreenView();

         var locator = new Mock<IServiceLocator>(MockBehavior.Strict);
         locator.Setup(x => x.TryGetInstance(typeof(IView<DerivedScreen>))).Returns(null);
         locator.Setup(x => x.TryGetInstance(typeof(IView<BaseScreen>))).Returns(view);
         ServiceLocator.SetServiceLocator(locator.Object);

         DerivedScreen screen = new DerivedScreen();
         ContentPresenter cp = new ContentPresenter();

         View.SetModel(cp, screen);

         Assert.AreEqual(view, cp.Content);
         Assert.AreEqual(screen, view.Model);
         locator.Verify(x => x.TryGetInstance(typeof(IView<DerivedScreen>)), Times.Once());
         locator.Verify(x => x.TryGetInstance(typeof(IView<BaseScreen>)), Times.Once());
      }

      [TestMethod]
      public void SetModelOnPlainDependencyObject() {
         DependencyObject obj = new DependencyObject();

         AssertHelper.Throws<ArgumentException>(() =>
            View.SetModel(obj, new SimpleScreen())
         ).Containing("only be set");
      }

      [TestMethod]
      public void SetModelToNull() {
         SimpleScreenView view = new SimpleScreenView();
         ContentControl cc = new ContentControl { Content = view };
         View.SetModel(cc, new SimpleScreen());
         Assert.IsNotNull(view.Model);
         View.SetModel(cc, null);
         Assert.IsNull(view.Model);
      }

      [TestMethod]
      public void SetModelToInvalidType() {
         SimpleScreenView view = new SimpleScreenView();
         AssertHelper.Throws<ArgumentException>(() =>
            View.SetModel(view, new Object())
         ).Containing("only be set");
      }

      [TestMethod]
      public void SetModelOnContentControlWithUnregisteredView() {
         SimpleScreenView view = new SimpleScreenView();

         var locator = new Mock<IServiceLocator>(MockBehavior.Strict);
         locator.Setup(x => x.TryGetInstance(It.IsAny<Type>())).Returns(null);
         ServiceLocator.SetServiceLocator(locator.Object);

         SimpleScreen screen = new SimpleScreen();
         ContentControl cc = new ContentControl();

         AssertHelper.Throws<ArgumentException>(() =>
            View.SetModel(cc, screen)
         ).Containing("is registered");
      }

      private class SimpleScreen : ScreenBase {
      }

      private class BaseScreen : ScreenBase {
      }

      private class DerivedScreen : BaseScreen {
      }

      private class SimpleScreenView : DependencyObject, IView<SimpleScreen> {
         public SimpleScreen Model { get; set; }
      }

      private class BaseScreenView : DependencyObject, IView<BaseScreen> {
         private BaseScreen _model;

         public BaseScreen Model {
            get { return _model; }
            set {
               Assert.IsNull(_model);
               _model = value;
            }
         }
      }

      private class MultiScreenView : DependencyObject, IView<SimpleScreen>, IView<DerivedScreen>, IView<BaseScreen>, IView<object> {
         public SimpleScreen SimpleScreenModel { get; set; }
         public DerivedScreen DerivedScreenModel { get; set; }
         public BaseScreen BaseScreenModel { get; set; }
         public object ObjectModel { get; set; }

         SimpleScreen IView<SimpleScreen>.Model {
            set {
               Assert.IsNull(SimpleScreenModel);
               SimpleScreenModel = value;
            }
         }

         DerivedScreen IView<DerivedScreen>.Model {
            set {
               Assert.IsNull(DerivedScreenModel);
               DerivedScreenModel = value;
            }
         }

         BaseScreen IView<BaseScreen>.Model {
            set {
               Assert.IsNull(BaseScreenModel);
               BaseScreenModel = value;
            }
         }

         object IView<object>.Model {
            set {
               Assert.IsNull(ObjectModel);
               ObjectModel = value;
            }
         }
      }

      [TestCleanup]
      public void Cleanup() {
         Bootstrapper.Initialize(null); // HACK
      }
   }
}
