namespace Inspiring.MvvmContribTest.Common {
   using System.Windows.Input;
   using Inspiring.Mvvm.Common;
   using Inspiring.MvvmTest;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class SaveDiscardVMTest {
      private Mock<ISaveDiscardHandler> _handlerMock;
      private SaveDiscardVM _vm;

      [TestInitialize]
      public void Setup() {
         _handlerMock = new Mock<ISaveDiscardHandler>(MockBehavior.Strict);
         _vm = new SaveDiscardVM();
         _vm.InitializeFrom(_handlerMock.Object);
      }

      [TestMethod]
      public void State_Valid_NoChanges() {
         _handlerMock.Setup(x => x.IsValid).Returns(true);
         _handlerMock.Setup(x => x.HasChanges).Returns(false);

         Assert.AreEqual(DataState.Unchanged, _vm.State);
      }

      [TestMethod]
      public void State_Valid_HasChanges() {
         _handlerMock.Setup(x => x.IsValid).Returns(true);
         _handlerMock.Setup(x => x.HasChanges).Returns(true);

         Assert.AreEqual(DataState.Changed, _vm.State);
      }

      [TestMethod]
      public void State_Invalid_NoChanges() {
         _handlerMock.Setup(x => x.IsValid).Returns(false);
         _handlerMock.Setup(x => x.HasChanges).Returns(false);

         Assert.AreEqual(DataState.Invalid, _vm.State);
      }

      [TestMethod]
      public void State_Invalid_HasChanges() {
         _handlerMock.Setup(x => x.IsValid).Returns(false);
         _handlerMock.Setup(x => x.HasChanges).Returns(true);

         Assert.AreEqual(DataState.Invalid, _vm.State);
      }

      [TestMethod]
      public void State_RequerySuggested() {
         PropertyChangedCounter counter = new PropertyChangedCounter(_vm, "State");
         // TODO: This does not work without dispatcher!
         CommandManager.InvalidateRequerySuggested();
         counter.AssertOneRaise();
      }
   }
}