namespace Inspiring.MvvmTest.ViewModels.Core.Paths {
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

      private class StepDefinitionMock : PathDefinitionStep {
         public StepDefinitionMock(PathMatch result) {
            Result = result;
         }

         public int InvocationCount { get; private set; }
         public PathDefinitionIterator DefinitionIterator { get; private set; }
         public PathIterator PathIterator { get; private set; }
         private PathMatch Result { get; set; }

         public override PathMatch Matches(PathDefinitionIterator definitionSteps, PathIterator step) {
            InvocationCount++;
            DefinitionIterator = definitionSteps;
            PathIterator = step;
            return Result;
         }
      }
   }
}