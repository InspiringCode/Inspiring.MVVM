using System;
using System.Windows.Controls;
using System.Windows.Input;
using Inspiring.Mvvm.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.Views {
   [TestClass]
   public class CommonBehaviors_DisableNullCommandTest {
      [TestMethod]
      public void Assumption_DisabledCommand_IsEnabledIsFalse() {
         var cmd = new TestCommand { CanExecute = false };

         Button btn = new Button();
         btn.Command = cmd;

         Assert.IsFalse(btn.IsEnabled);
      }

      [TestMethod]
      public void Assumption_EnabledCommand_IsEnabledIsFalse() {
         var cmd = new TestCommand { CanExecute = true };

         Button btn = new Button();
         btn.Command = cmd;

         Assert.IsTrue(btn.IsEnabled);
      }

      [TestMethod]
      public void Assumption_CommandSetToNull_IsEnabledIsTrue() {
         Button btn = new Button();
         btn.Command = new TestCommand { CanExecute = false };
         btn.Command = null;

         Assert.IsTrue(btn.IsEnabled);

      }

      [TestMethod]
      public void NoCommand_IsEnabledIsFalse() {
         Button btn = new Button();
         CommonBehaviors.SetDisableIfCommandIsNull(btn, true);
         Assert.IsFalse(btn.IsEnabled);
      }

      [TestMethod]
      public void EnabledCommand_IsEnabledIsTrue() {
         Button btn = new Button();
         CommonBehaviors.SetDisableIfCommandIsNull(btn, true);

         btn.Command = new TestCommand { CanExecute = true };

         Assert.IsTrue(btn.IsEnabled);
      }

      [TestMethod]
      public void DisabledCommand_IsEnabledIsFalse() {
         Button btn = new Button();
         CommonBehaviors.SetDisableIfCommandIsNull(btn, true);

         btn.Command = new TestCommand { CanExecute = false };

         Assert.IsFalse(btn.IsEnabled);
      }

      [TestMethod]
      public void CommandSetToNull_IsEnabledIsFalse() {
         Button btn = new Button();
         CommonBehaviors.SetDisableIfCommandIsNull(btn, true);

         btn.Command = new TestCommand { CanExecute = false };
         btn.Command = null;

         Assert.IsFalse(btn.IsEnabled);
      }

      private class TestCommand : ICommand {
         private bool _canExecute = true;

         public bool CanExecute {
            get { return _canExecute; }
            set {
               _canExecute = value;
               if (CanExecuteChanged != null) {
                  CanExecuteChanged(this, EventArgs.Empty);
               }
            }
         }

         public event EventHandler CanExecuteChanged;

         bool ICommand.CanExecute(object parameter) {
            return CanExecute;
         }

         void ICommand.Execute(object parameter) {
         }
      }
   }
}
