using Inspiring.Mvvm.ViewModels;
using Inspiring.Mvvm.ViewModels.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Inspiring.MvvmTest.ViewModels.Behaviors {
   [TestClass]
   public class CollectionFactoryTest {
      private IBehaviorContext _context;

      [TestInitialize]
      public void Setup() {
         SetupContext();
      }

      [TestMethod]
      public void GetValue() {
         var fac = new CollectionFactoryBehavior<PersonVM>(PersonVM.Descriptor);
         VMCollection<PersonVM> first = fac.GetValue(_context, ValueStage.PostValidation);
         VMCollection<PersonVM> second = fac.GetValue(_context, ValueStage.PostValidation);
         Assert.IsNotNull(first);
         Assert.IsNotNull(second);
         Assert.AreNotEqual(first, second);
      }

      private void SetupContext() {
         var mock = new Mock<IBehaviorContext>();
         _context = mock.Object;
      }
   }
}
