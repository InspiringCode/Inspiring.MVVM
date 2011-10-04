namespace Inspiring.MvvmTest.ViewModels.Core.Common.Paths {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class OptionalStepTests : StepFixture {

      [TestMethod]
      public void Matches_InnerStepSucceeds_Succeeds() {
         var optionalStep = new OptionalStep(PathAssert.SucceedingNextStep);

         var path = Path
            .Empty
            .Append(VM);

         var assert = new PathAssert(optionalStep, path);

         assert.AssertMatchWith(PathAssert.SucceedingNextStep);
      }

      [TestMethod]
      public void Matches_InnerStepFailsNextStepSucceeds_Succeeds() {
         var optionalStep = new OptionalStep(PathAssert.FailingNextStep);

         var path = Path
            .Empty
            .Append(VM);

         var assert = new PathAssert(optionalStep, path);

         assert.AssertMatchWith(PathAssert.SucceedingNextStep);
      }

      [TestMethod]
      public void Matches_InnerStepFailsNextStepFails_Fails() {
         var optionalStep = new OptionalStep(PathAssert.FailingNextStep);

         var path = Path
            .Empty
            .Append(VM);

         var assert = new PathAssert(optionalStep, path);

         assert.AssertNoMatchWith(PathAssert.FailingNextStep);
      }

      [TestMethod]
      public void ToString_ReturnsOptionalStepPlusInnerStep() {
         var innerStep = CreateStep(x => x.Projects);
         var optionalStep = new OptionalStep(innerStep);
         Assert.AreEqual("Projects?", optionalStep.ToString());
      }

      private PathDefinitionStep CreateStep<TValue>(Func<EmployeeVMDescriptor, IVMPropertyDescriptor<TValue>> propertySelector) {
         return CreateStep<EmployeeVMDescriptor, TValue>(propertySelector);
      }

      private PathDefinitionStep CreateStep<TDescriptor, TValue>(
         Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector
      ) where TDescriptor : IVMDescriptor {
         return new PropertyStep<TDescriptor>(propertySelector);
      }
   }
}
