namespace Inspiring.MvvmTest.ViewModels.Core.PropertyBehaviors {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionValidationBehaviorTest {
      [TestMethod]
      public void Validate() {
         ValidationEventArgs args = new ValidationEventArgs(
            new VMProperty<string>(),
            42m,
            new TestVM { LocalAccessor = 42m }
         );


         int invocationCount = 0;
         CollectionValidator<TestVM> asserter = (item, allItems, e) => {
            invocationCount++;
            Assert.AreSame(args, e);
         };

         var b = new CollectionValidationBehavior<TestVM>();
         b.Add(asserter);
         b.Add(asserter);

         b.Validate(new TestVM[] { new TestVM(), new TestVM() }, args);

         Assert.AreEqual(2, invocationCount);
      }
   }
}