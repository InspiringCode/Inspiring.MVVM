namespace Inspiring.MvvmTest.ViewModels.Core.PropertyBehaviors {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ViewModelFactoryBehaviorTest {
      // [TestMethod] // TODO
      public void CreateInstance() {
         var context = new Mock<IBehaviorContext>(MockBehavior.Strict).Object;

         IViewModelFactoryBehavior<PersonVM> fac = new ViewModelFactoryBehavior<PersonVM>();

         PersonVM first = fac.CreateInstance(context);
         Assert.IsNotNull(first);

         PersonVM second = fac.CreateInstance(context);
         Assert.IsNotNull(second);
         Assert.AreNotSame(first, second);
      }
   }
}