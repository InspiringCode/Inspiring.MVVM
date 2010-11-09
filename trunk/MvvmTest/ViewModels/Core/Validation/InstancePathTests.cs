namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class InstancePathTests {
      [TestMethod]
      public void Steps_OfDefaultInstance_ReturnsEmptyCollection() {
         var path = new InstancePath();
         Assert.AreEqual(0, path.Steps.Length);
      }

      [TestMethod]
      public void Steps_ViewModelConstructor_ReturnsSingleStep() {
         IViewModel vm = new Mock<IViewModel>().Object;

         var path = new InstancePath(vm);

         AssertHelper.AreEquivalent(
            new InstancePathStep[] { new InstancePathStep(vm) },
            path.Steps,
            StepsAreEqual
         );
      }

      [TestMethod]
      public void PrependVM_CalledTwoTimes_AddsTwoSteps() {

      }

      [TestMethod]
      public void PrependCollection_OnDefaultInstance_ThrowsInvalidOperationException() {

      }

      [TestMethod]
      public void PrependCollection_WithTwoStepPath_ModifiesFirstStep() {

      }

      [TestMethod]
      public void MatchStart_WithOnePropertyOnDefaultInstance_DoesNotMatch() {

      }

      [TestMethod]
      public void MatchStart_WithOnePropertyOnOneStepPath_DoesNotMatch() {

      }

      [TestMethod]
      public void MatchStart_WithTwoMatcingPropertiesOnThreeStepPath_DoesMatch() {

      }

      [TestMethod]
      public void MatchStart_WithTwoNonMatchingPropertiesOnThreeStepPath_DoesNotMatch() {

      }

      private static bool StepsAreEqual(InstancePathStep x, InstancePathStep y) {
         if (Object.ReferenceEquals(x, y)) {
            return true;
         }

         return
            x != null &&
            y != null &&
            Object.ReferenceEquals(x.VM, y.VM) &&
            Object.ReferenceEquals(x.ParentCollection, y.ParentCollection);
      }
   }
}