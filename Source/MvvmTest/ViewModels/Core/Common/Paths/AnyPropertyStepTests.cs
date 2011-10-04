namespace Inspiring.MvvmTest.ViewModels.Core.Common.Paths {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class AnyPropertyStepTests : StepFixture {

      [TestMethod]
      public void Matches_WithEmptyPath_Fails() {
         AssertNoMatch(
            new AnyPropertyStep<EmployeeVMDescriptor>(),
            Path.Empty
         );
      }

      [TestMethod]
      public void Matches_PathStartsWithProperty_Fails() {
         var pathDefinition = new AnyPropertyStep<EmployeeVMDescriptor>();
         var wrongPath = Path.Empty.Append(ProjectVM.ClassDescriptor.EndDate);

         AssertNoMatch(pathDefinition, wrongPath);
      }

      [TestMethod]
      public void Matches_ViewModelPlusProperty_Succeeds() {

         var path = Path
            .Empty
            .Prepend(Descriptor.SelectedProject)
            .Prepend(VM);

         AssertMatchIfNextSucceeds(
            new AnyPropertyStep<EmployeeVMDescriptor>(),
            path);
      }

      [TestMethod]
      public void Matches_ViewModelPlusCollection_Succeeds() {
         var path = Path
            .Empty
            .Prepend(EmployeeVM.ClassDescriptor.Projects)
            .Prepend(VM);

         AssertMatchIfNextSucceeds(
            new AnyPropertyStep<EmployeeVMDescriptor>(),
            path);
      }

      [TestMethod]
      public void Matches_ViewModelPlusChildViewModel_Succeeds() {
         var path = Path
            .Empty
            .Prepend(EmployeeVM.ClassDescriptor.SelectedProject)
            .Prepend(VM);

         AssertMatchIfNextSucceeds(
            new AnyPropertyStep<EmployeeVMDescriptor>(),
            path);
      }

      [TestMethod]
      public void Matches_OnlyViewModel_Fails() {
         var path = Path
           .Empty
           .Prepend(VM);

         AssertNoMatch(
            new AnyPropertyStep<EmployeeVMDescriptor>(),
            path
         );
      }

      [TestMethod]
      public void Matches_ViewModelPlusPropertyFromOtherDescriptor_ThrowsException() {
         var path = Path
            .Empty
            .Prepend(CustomerVM.ClassDescriptor.Name)
            .Prepend(VM);

         AssertException(
            new AnyPropertyStep<EmployeeVMDescriptor>(),
            path
         );
      }

      [TestMethod]
      public void ToString_ReturnsKeyword() {
         var step = new AnyPropertyStep<EmployeeVMDescriptor>();
         Assert.AreEqual("[any property]", step.ToString());
      }
   }
}
