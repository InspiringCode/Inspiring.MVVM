namespace Inspiring.MvvmTest.Views {
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.Views;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.Common;

   [TestClass]
   public class DialogServiceTests {
      private IDialogService DialogSerivce { get; set; }

      [TestInitialize]
      public void Setup() {
         DialogSerivce = new DialogService(new WindowServiceStub(), new EventAggregator());
      }

      [TestMethod]
      public void ShowDialogWithCertainScreenInsatnce_ScreenLifecycleDoesntContainDialogLifecycleAfterShowDialog() {
         var screen = new ScreenBase();

         DialogSerivce.ShowDialog(ScreenFactory.For(screen));

         Assert.IsFalse(screen.Children.Contains<DialogLifecycle>());
      }


      private class WindowServiceStub : WindowService {
         public override void ShowDialogWindow(IScreenBase screen, IScreenBase parent, string title) {
         }
      }
   }
}
