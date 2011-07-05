namespace Inspiring.MvvmTest.ViewModels.Core.Common.Paths {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class PathDefinitionTests : PathFixture {
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

      [TestMethod]
      public void GetDescendants_ForSimpleProperty_ThrowsNotSupportedException() {
         var path = PathDefinition
            .Empty
            .Append((EmployeeVMDescriptor x) => x.Name);

         var rootVM = new EmployeeVM();

         AssertHelper.Throws<NotSupportedException>(() => {
            path.GetDescendants(rootVM);
         });
      }

      [TestMethod]
      public void GetDescendants_DescendantViewModel_ReturnsViewModel() {
         var path = PathDefinition
            .Empty
            .Append((EmployeeVMDescriptor x) => x.SelectedProject);

         var expectedVM = new ProjectVM();
         var rootVM = new EmployeeVM();
         rootVM.SetValue(x => x.SelectedProject, expectedVM);

         var descendantVM = path.GetDescendants(rootVM);

         Assert.AreSame(expectedVM, descendantVM.First());
      }

      [TestMethod]
      public void GetDescendants_DescendantViewModelOverSeveralHierarchies_ReturnsLastViewModel() {
         var path = PathDefinition
            .Empty
            .Append((EmployeeVMDescriptor x) => x.SelectedProject)
            .Append((ProjectVMDescriptor x) => x.SelectedCustomer);

         var expectedVM = new CustomerVM();
         var projectVM = new ProjectVM();
         projectVM.SetValue(x => x.SelectedCustomer, expectedVM);
         var rootVM = new EmployeeVM();
         rootVM.SetValue(x => x.SelectedProject, projectVM);

         var descendantVM = path.GetDescendants(rootVM);

         Assert.AreSame(expectedVM, descendantVM.First());
      }

      [TestMethod]
      public void GetDescendants_DesendantCollection_ReturnsAllViewModelsInCollection() {
         var path = PathDefinition
            .Empty
            .Append((EmployeeVMDescriptor x) => x.Projects);

         var project1 = new ProjectVM();
         var project2 = new ProjectVM();
         var project3 = new ProjectVM();

         var expectedViewModels = new ProjectVM[] {
            project1,
            project2,
            project3
         };

         var rootVM = new EmployeeVM();

         var projects = rootVM.GetValue(x => x.Projects);
         projects.Add(project1);
         projects.Add(project2);
         projects.Add(project3);

         var descenantViewModels = path.GetDescendants(rootVM);

         CollectionAssert.AreEqual(expectedViewModels, descenantViewModels);
      }

      [TestMethod]
      public void GetDescendants_DesendantCollectionOverSeveralCollectionHierarchies_ReturnsLastCollection() {
         var path = PathDefinition
            .Empty
            .Append((EmployeeVMDescriptor x) => x.Projects)
            .Append((ProjectVMDescriptor x) => x.Customers);

         var customer11 = new CustomerVM();
         var customer21 = new CustomerVM();
         var customer31 = new CustomerVM();


         var project1 = new ProjectVM();
         project1.GetValue(x => x.Customers).Add(customer11);
         var project2 = new ProjectVM();
         project2.GetValue(x => x.Customers).Add(customer21);
         var project3 = new ProjectVM();
         project3.GetValue(x => x.Customers).Add(customer31);

         var expectedViewModels = new CustomerVM[] {
            customer11,
            customer21,
            customer31
         };

         var rootVM = new EmployeeVM();

         var projects = rootVM.GetValue(x => x.Projects);
         projects.Add(project1);
         projects.Add(project2);
         projects.Add(project3);

         var descenantViewModels = path.GetDescendants(rootVM);

         CollectionAssert.AreEqual(expectedViewModels, descenantViewModels);
      }

      [TestMethod]
      public void GetDescendants_DesendantViewModelOverCollection_ReturnsAllViewModels() {
         var path = PathDefinition
            .Empty
            .Append((EmployeeVMDescriptor x) => x.Projects)
            .Append((ProjectVMDescriptor x) => x.SelectedCustomer);

         var customer1 = new CustomerVM();
         var customer2 = new CustomerVM();
         var customer3 = new CustomerVM();

         var project1 = new ProjectVM();
         project1.SetValue(x => x.SelectedCustomer, customer1);
         var project2 = new ProjectVM();
         project2.SetValue(x => x.SelectedCustomer, customer2);
         var project3 = new ProjectVM();
         project3.SetValue(x => x.SelectedCustomer, customer3);

         var expectedViewModels = new CustomerVM[] {
            customer1,
            customer2,
            customer3
         };

         var rootVM = new EmployeeVM();

         var projects = rootVM.GetValue(x => x.Projects);
         projects.Add(project1);
         projects.Add(project2);
         projects.Add(project3);

         var descenantViewModels = path.GetDescendants(rootVM);

         CollectionAssert.AreEqual(expectedViewModels, descenantViewModels);
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

         public override IViewModel[] GetDescendants(
            PathDefinitionIterator definitionSteps,
            IViewModel rootVM) {
            throw new NotSupportedException();
         }
      }
   }
}