namespace Inspiring.MvvmTest.ViewModels.Core.Common.Paths {
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class PathDefinitionTests {
      [TestMethod]
      public void Matches_CallsMatchesOnFirstDefinitionStep() {
         var definitionStep = new StepDefinitionMock(PathMatch.Succeed(0));
         var definition = PathDefinition.Empty.Append(definitionStep);

         definition.Matches(Path.Empty);

         Assert.AreEqual(
            1,
            definitionStep.InvocationCount,
            "The PathDefinition did not call the first PathDefinitionStep."
         );
      }

      [TestMethod]
      public void Matches_PassesPathIteratorAtFirstElementToFirstDefinitionStep() {
         var firstPathStep = new ViewModelStub();
         var path = Path.Empty.Append(firstPathStep);

         var definitionStep = new StepDefinitionMock(PathMatch.Succeed(0));
         var definition = PathDefinition.Empty.Append(definitionStep);

         definition.Matches(path);

         Assert.AreEqual(
            firstPathStep,
            definitionStep.PathIterator.ViewModel,
            "The PathIterator was not positioned at the first step."
         );
      }

      [TestMethod]
      public void Matches_CallingMatchesNextOnPassedIteratorCallsNext() {
         var firstDefinitionStep = new StepDefinitionMock(PathMatch.Succeed(0));
         var secondDefinitionStep = new StepDefinitionMock(PathMatch.Succeed(0));

         var definition = PathDefinition.Empty
            .Append(firstDefinitionStep)
            .Append(secondDefinitionStep);

         definition.Matches(Path.Empty);
         Assert.AreEqual(0, secondDefinitionStep.InvocationCount);

         firstDefinitionStep.DefinitionIterator.MatchesNext(Path.Empty.GetIterator());
         Assert.AreEqual(1, secondDefinitionStep.InvocationCount);
      }

      [TestMethod]
      public void ToString_WithoutSteps_ReturnsEmptyPath() {
         var path = PathDefinition.Empty;
         Assert.AreEqual("[empty path]", path.ToString());
      }

      [TestMethod]
      public void ToString_WithSteps_ReturnsConcatenatedSteps() {
         var path = PathDefinition.Empty
            .Append(new StepDefinitionMock("Step 1"))
            .Append(new StepDefinitionMock("Step 2"));

         Assert.AreEqual("[Step 1, Step 2]", path.ToString());
      }

      private class StepDefinitionMock : PathDefinitionStep {
         public StepDefinitionMock(PathMatch result) {
            Result = result;
         }

         public StepDefinitionMock(string toStringResult) {
            ToStringResult = toStringResult;
         }

         public int InvocationCount { get; private set; }
         public PathDefinitionIterator DefinitionIterator { get; private set; }
         public PathIterator PathIterator { get; private set; }
         private PathMatch Result { get; set; }
         private string ToStringResult { get; set; }

         public override PathMatch Matches(PathDefinitionIterator definitionSteps, PathIterator step) {
            InvocationCount++;
            DefinitionIterator = definitionSteps;
            PathIterator = step;
            return Result;
         }

         public override string ToString() {
            return ToStringResult ?? base.ToString();
         }
      }
   }
}