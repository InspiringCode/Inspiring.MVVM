namespace Inspiring.MvvmContribTest.Screens {
   using System;
   using System.Windows.Input;
   using Inspiring.Mvvm.Screens;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ApplicationCommandManagerTest {
      [TestMethod]
      public void TestCommandManager() {
         ApplicationCommandManager cm = new ApplicationCommandManager();

         ICommand newCommand = cm[TestCommands.New];
         Assert.AreEqual(newCommand, cm[TestCommands.New]);

         ICommand saveCommand = cm[TestCommands.Save];
         Assert.AreNotEqual(newCommand, saveCommand);

         var actualNewCommand = new Mock<ICommand>();
         actualNewCommand.Setup(x => x.CanExecute(It.IsAny<Object>())).Returns(true);

         Assert.IsFalse(newCommand.CanExecute(null));
         AssertHelper.Throws<InvalidOperationException>(() => {
            newCommand.Execute(null);
         });

         cm.RegisterCommand(TestCommands.New, actualNewCommand.Object);

         Assert.IsTrue(newCommand.CanExecute(null));
         actualNewCommand.Verify(x => x.CanExecute(null), Times.Once());

         newCommand.Execute(null);
         actualNewCommand.Verify(x => x.Execute(null), Times.Once());

         cm.UnregisterCommand(TestCommands.New);

         Assert.IsFalse(newCommand.CanExecute(null));
         AssertHelper.Throws<InvalidOperationException>(() => {
            newCommand.Execute(null);
         });

         // Verify that these method were not called
         actualNewCommand.Verify(x => x.CanExecute(null), Times.Once());
         actualNewCommand.Verify(x => x.Execute(null), Times.Once());
      }

      [TestMethod]
      public void CheckCanExecuteChanged() {
         ApplicationCommandManager cm = new ApplicationCommandManager();

         ICommand proxy = cm[TestCommands.Close];

         TestCommand cmd = new TestCommand();
         cm.RegisterCommand(TestCommands.Close, cmd);

         bool raised = false;
         proxy.CanExecuteChanged += delegate {
            raised = true;
         };

         cmd.RaiseCanExecuteChanged();

         Assert.IsTrue(raised);
      }

      [TestMethod]
      public void GetProxyCommand_WithNoRegisteredRealCommand_HasCommandIsFalse() {
         ApplicationCommandManager cm = new ApplicationCommandManager();

         var proxyCmd = (CommandProxy)cm[TestCommands.New];

         Assert.IsFalse(proxyCmd.HasCommand);
      }

      [TestMethod]
      public void GetProxyCommand_WithRegisteredRealCommand_HasCommandIsTrue() {
         ApplicationCommandManager cm = new ApplicationCommandManager();

         TestCommand cmd = new TestCommand();
         cm.RegisterCommand(TestCommands.New, cmd);

         var proxyCmd = (CommandProxy)cm[TestCommands.New];

         Assert.IsTrue(proxyCmd.HasCommand);
      }

      private enum TestCommands {
         New,
         Save,
         Close
      }

      private class TestCommand : ICommand {
         public event EventHandler CanExecuteChanged;

         public void RaiseCanExecuteChanged() {
            if (CanExecuteChanged != null) {
               CanExecuteChanged(this, EventArgs.Empty);
            }
         }

         public bool CanExecute(object parameter) {
            return true;
         }

         public void Execute(object parameter) {
         }
      }
   }
}