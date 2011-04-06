namespace Inspiring.MvvmTest.ApiTests.ViewModels.Undo {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class UndoManagerTests {

      [TestMethod]
      public void GetManager_NoBehaviorIsDefined_ThrowsInvalidOperationException() {
         var rootVM = new ViewModelStub();

         AssertHelper.Throws<InvalidOperationException>(() => {
            UndoManager.GetManager(rootVM);
         });
      }

      [TestMethod]
      public void GetManager_RootViewModelContainsBehavior_ReturnsManager() {
         var rootVM = new ViewModelStub(new UndoRootBehavior());

         var foundManager = UndoManager.GetManager(rootVM);

         Assert.IsNotNull(foundManager);
      }

      [TestMethod]
      public void GetManager_ParentVMContainsBehavior_ReturnsManager() {
         var parent1 = new ViewModelStub();
         var parent2 = new ViewModelStub(new UndoRootBehavior());

         var rootVM = new ViewModelStub();
         rootVM.Kernel.Parents.Add(parent1);
         rootVM.Kernel.Parents.Add(parent2);

         var foundManager = UndoManager.GetManager(rootVM);

         Assert.IsNotNull(foundManager);
      }

      [TestMethod]
      public void GetManager_MultipleUndoRootsInSameAncestorDistance_ThrowsNotSupportedException() {
         var parent1 = new ViewModelStub(new UndoRootBehavior());
         var parent2 = new ViewModelStub(new UndoRootBehavior());

         var rootVM = new ViewModelStub();
         rootVM.Kernel.Parents.Add(parent1);
         rootVM.Kernel.Parents.Add(parent2);

         AssertHelper.Throws<NotSupportedException>(() => {
            UndoManager.GetManager(rootVM);
         });
      }

      [TestMethod]
      public void GetManager_BehaviorIsLocatedInComplexAncestorStructure_ResturnsManager() {
         var grandparent1 = new ViewModelStub(new UndoRootBehavior());

         var parent1 = new ViewModelStub();
         parent1.Kernel.Parents.Add(grandparent1);

         var parent2 = new ViewModelStub();

         var rootVM = new ViewModelStub();
         rootVM.Kernel.Parents.Add(parent1);
         rootVM.Kernel.Parents.Add(parent2);

         var foundManager = UndoManager.GetManager(rootVM);

         Assert.IsNotNull(foundManager);
      }

      [TestMethod]
      public void GetManager_BehaviorIsLocatedInDifferentAncestorDisntances_ReturnsNearestManager() {
         var behavior = new UndoRootBehavior();

         var grandparent1 = new ViewModelStub(new UndoRootBehavior());

         var parent1 = new ViewModelStub();
         parent1.Kernel.Parents.Add(grandparent1);

         var parent2 = new ViewModelStub(behavior);
         var expectedManager = behavior.GetUndoManager(parent2.GetContext());

         var rootVM = new ViewModelStub();
         rootVM.Kernel.Parents.Add(parent1);
         rootVM.Kernel.Parents.Add(parent2);

         var foundManager = UndoManager.GetManager(rootVM);

         Assert.IsNotNull(foundManager);
         Assert.AreSame(expectedManager, foundManager);

      }

      [TestMethod]
      public void Undo_RollbackPointDoesntExists_ThrowsArgumentException() {
         AssertHelper.Throws<ArgumentException>(() => {
            new UndoManager().RollbackTo(new Mock<IUndoableAction>().Object);
         });
      }
   }
}