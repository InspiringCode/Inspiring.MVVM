namespace Inspiring.MvvmTest.ApiTests.ViewModels.Undo {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class UndoRootSearchTests {

      [TestMethod]
      public void GetUndoRootBehavior_NoBehaviorIsDefined_ReturnsNull() {
         var rootVM = new ViewModelStub();

         var foundBehavior = UndoManager.GetUndoRootBehavior(rootVM);

         Assert.IsNull(foundBehavior);
      }

      [TestMethod]
      public void GetUndoRootBehavior_RootViewModelContainsBehavior_ReturnsBehavior() {
         var expectedBehavior = new UndoRootBehavior();

         var rootVM = new ViewModelStub(expectedBehavior);

         var foundBehavior = UndoManager.GetUndoRootBehavior(rootVM);

         Assert.AreSame(expectedBehavior, foundBehavior);
      }

      [TestMethod]
      public void GetUndoRootBehavior_ParentVMContainsBehavior_ReturnsBehavior() {
         var expectedBehavior = new UndoRootBehavior();

         var parent1 = new ViewModelStub();
         var parent2 = new ViewModelStub(expectedBehavior);

         var rootVM = new ViewModelStub();
         rootVM.Kernel.Parents.Add(parent1);
         rootVM.Kernel.Parents.Add(parent2);

         var foundBehavior = UndoManager.GetUndoRootBehavior(rootVM);

         Assert.AreSame(expectedBehavior, foundBehavior);
      }

      [TestMethod]
      public void GetUndoRootBehavior_MultipleUndoRootsInSameAncestorDistance_ThrowsNotSupportedException() {
         var parent1 = new ViewModelStub(new UndoRootBehavior());
         var parent2 = new ViewModelStub(new UndoRootBehavior());

         var rootVM = new ViewModelStub();
         rootVM.Kernel.Parents.Add(parent1);
         rootVM.Kernel.Parents.Add(parent2);

         AssertHelper.Throws<NotSupportedException>(() => {
            UndoManager.GetUndoRootBehavior(rootVM);
         });
      }

      [TestMethod]
      public void GetUndoRootBehavior_BehaviorIsLocatedInComplexAncestorStructure_ResturnsBehavior() {
         var expectedBehavior = new UndoRootBehavior();

         var grandparent1 = new ViewModelStub(expectedBehavior);

         var parent1 = new ViewModelStub();
         parent1.Kernel.Parents.Add(grandparent1);

         var parent2 = new ViewModelStub();

         var rootVM = new ViewModelStub();
         rootVM.Kernel.Parents.Add(parent1);
         rootVM.Kernel.Parents.Add(parent2);

         var foundBehavior = UndoManager.GetUndoRootBehavior(rootVM);

         Assert.AreSame(expectedBehavior, foundBehavior);
      }

      [TestMethod]
      public void GetUndoRootBehavior_BehaviorIsLocatedInDifferentAncestorDisntances_ReturnsNearestBehavior() {
         var expectedBehavior = new UndoRootBehavior();

         var grandparent1 = new ViewModelStub(new UndoRootBehavior());

         var parent1 = new ViewModelStub();
         parent1.Kernel.Parents.Add(grandparent1);

         var parent2 = new ViewModelStub(expectedBehavior);

         var rootVM = new ViewModelStub();
         rootVM.Kernel.Parents.Add(parent1);
         rootVM.Kernel.Parents.Add(parent2);

         var foundBehavior = UndoManager.GetUndoRootBehavior(rootVM);

         Assert.AreSame(expectedBehavior, foundBehavior);
      }
   }
}