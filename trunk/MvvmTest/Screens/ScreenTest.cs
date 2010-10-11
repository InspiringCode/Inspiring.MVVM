namespace Inspiring.MvvmTest.Screens {
   using Inspiring.Mvvm.Screens;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ScreenTest {
      [TestMethod]
      public void TestChildren() {
         //var childScreen = new Mock<ILifecycleHandler>(MockBehavior.Strict);
         //var childScreenFactory = MockObjectFactory.MockScreenFactory(childScreen.Object);
         //var parent = new Mock<ParentScreen>(MockBehavior.Loose, childScreenFactory.Object);

         //ILifecycleHandler parentAsScreen = (ILifecycleHandler)parent.Object;

         //childScreen.SetupSet(x => x.Parent = parent.Object);
         //childScreen.Setup(x => x.Initialize());
         //parent.Object.CreateChild();
         //Assert.AreEqual(childScreen.Object, parent.Object.Child);
         //childScreen.Verify(x => x.Initialize(), Times.Once());

         //parentAsScreen.Initialize();
         //childScreen.Verify(x => x.Initialize(), Times.Once());

         //childScreen.Setup(x => x.Activate());
         //parentAsScreen.Activate();
         //childScreen.Verify(x => x.Activate(), Times.Once());
         //parent.Protected().Verify("OnActivate", Times.Once());

         //childScreen.Setup(x => x.Deactivate());
         //parentAsScreen.Deactivate();
         //childScreen.Verify(x => x.Deactivate(), Times.Once());
         //parent.Protected().Verify("OnDeactivate", Times.Once());

         //childScreen.Setup(x => x.RequestClose()).Returns(false);
         //Assert.IsFalse(parentAsScreen.RequestClose());
         //childScreen.Verify(x => x.RequestClose(), Times.Once());
         //parent.Protected().Verify("OnRequestClose", Times.Never());

         //childScreen.Setup(x => x.RequestClose()).Returns(true);
         //parent.Protected().Setup<bool>("OnRequestClose").Returns(true);
         //Assert.IsTrue(parentAsScreen.RequestClose());
         //childScreen.Verify(x => x.RequestClose(), Times.Exactly(2));
         //parent.Protected().Verify("OnRequestClose", Times.Once());

         //childScreen.Setup(x => x.Close());
         //parentAsScreen.Close();
         //childScreen.Verify(x => x.Close(), Times.Once());
         //parent.Protected().Verify("OnClose", Times.Once());
      }


      public class ParentScreen : ScreenBase {
         //private IScreenFactory<ILifecycleHandler> _mockFactory;

         //public ParentScreen(IScreenFactory<ILifecycleHandler> mockFactory) {
         //   _mockFactory = mockFactory;
         //}

         //public ILifecycleHandler Child { get; set; }

         //public void CreateChild() {
         //   Child = Children.AddNew<IScreen>(_mockFactory);
         //}
      }


      //[TestMethod]
      //public void MultipleInstances() {
      //   PersonScreen s = new PersonScreen();

      //   bool propertyChanged = false;
      //   s.PropertyChanged += (sender, e) => {
      //      if (e.PropertyName == "Title") {
      //         propertyChanged = true;
      //      }
      //   };

      //   s.ChangeTitle("New title");
      //   Assert.IsTrue(propertyChanged);
      //}

      //private class PersonScreen : Screen {
      //   public void ChangeTitle(string title) {
      //      Title = title;
      //   }
      //}
   }
}
