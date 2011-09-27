namespace Inspiring.MvvmContribTest.ViewModels.Selection {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inspiring.Mvvm.ViewModels;
using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;

   [TestClass]
   public class ItemProviderBehaviorTests {
      private ItemProviderBehavior<TestSourceItem> Behavior { get; set; }
      private IBehaviorContext Context { get; set; }


      [TestInitialize]
      public void Setup() {
         Behavior = new ItemProviderBehavior<TestSourceItem>();
         
         Context = ViewModelStub
            .WithBehaviors(Behavior)
            .BuildContext();
      }


      private class TestSourceItem {
      }
   }
}