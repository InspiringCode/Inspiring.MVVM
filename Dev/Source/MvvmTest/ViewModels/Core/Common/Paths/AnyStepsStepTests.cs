namespace Inspiring.MvvmTest.ViewModels.Core.Common.Paths {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class AnyStepsStepTests : StepFixture {

      [TestMethod]
      public void Matches_WithEmptyPath_Fails() {
         AssertNoMatch(
            new AnyStepsStep<EmployeeVMDescriptor>(),
            Path.Empty
         );
      }

      [TestMethod]
      public void Matches_PathStartsWithProperty_Fails() {
         var pathDefinition = new AnyStepsStep<EmployeeVMDescriptor>();
         var wrongPath = Path.Empty.Append(EmployeeVM.ClassDescriptor.SelectedProject);

         AssertNoMatch(pathDefinition, wrongPath);
      }

      [TestMethod]
      public void Matches_OnlyViewModel_Fails() {
         var path = Path
           .Empty
           .Prepend(VM);

         AssertNoMatch(
            new AnyStepsStep<EmployeeVMDescriptor>(),
            path
         );
      }

      [TestMethod]
      public void Matches_ViewModelPlusProperty_Succeeds() {

         var path = Path
            .Empty
            .Append(VM)
            .Append(EmployeeVM.ClassDescriptor.SelectedProject);

         PathAssert assert = new PathAssert(new AnyStepsStep<EmployeeVMDescriptor>(), path);

         assert.AssertMatchWith(PathAssert.SucceedingNextStep);
      }

      [TestMethod]
      public void Matches_ViewModelPlusPropertyFromOtherDescriptor_ThrowsException() {
         var path = Path
            .Empty
            .Prepend(CustomerVM.ClassDescriptor.Name)
            .Prepend(VM);

         AssertException(
            new AnyStepsStep<EmployeeVMDescriptor>(),
            path
         );
      }

      [TestMethod]
      public void ToString_ReturnsDescriptorNameAndAnyPropertyString() {
         var step = new AnyStepsStep<EmployeeVMDescriptor>();
         Assert.AreEqual("EmployeeVMDescriptor -> AnySteps", step.ToString());
      }
   }
}