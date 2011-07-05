namespace Inspiring.MvvmContribTest.ViewModels.MultiSelection {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class LookupPopulatorCollectionBehaviorTests : TestBase {
      [TestMethod]
      public void Repopulate_VMIsPassedToDelegate() {
         Assert.Inconclusive("Refactor");
         var employeeVM = new EmployeeVM();
         EmployeeVM passedVM = null;

         var behavior = CreateBehavior(vm => {
            passedVM = vm;
            return Enumerable.Empty<ProjectVM>();
         });

         throw new NotImplementedException();
         //behavior.Repopulate(new ContextTestHelper(employeeVM).Context, new VMCollection<ProjectVM>(new BehaviorChain(), employeeVM));

         Assert.AreSame(employeeVM, passedVM);
      }

      [TestMethod]
      public void Repopulate_SourceItemFound_AddsVM() {
         Assert.Inconclusive("Refactor");
         var context = new ContextTestHelper(new EmployeeVM());
         var projectSource = new Project();
         var projectVM = new ProjectVM(projectSource);
         var additionalProjectVM = new ProjectVM(new Project());

         var lookupCollection = new[] { projectVM, additionalProjectVM };
         var sourceCollection = new[] { projectSource };

         var behavior = CreateBehavior(lookupCollection, sourceCollection);

         var collection = VMCollectionStub
            .Of<ProjectVM>()
            .WithOwner(context.VM)
            .Build();

         throw new NotImplementedException();
         //behavior.GetValue(
         //behavior.Repopulate(context.Context, collection);

         CollectionAssert.AreEquivalent(
            new[] { projectVM },
            collection
         );
      }

      [TestMethod]
      public void Repopulate_SourceItemNotFound_ThrowsInvalidOperationException() {
         Assert.Inconclusive();
      }

      private LookupPopulatorCollectionBehavior<EmployeeVM, ProjectVM, Project> CreateBehavior(
         IEnumerable<ProjectVM> lookupCollection,
         IEnumerable<Project> sourceObjectCollection = null
      ) {
         return CreateBehavior(vm => lookupCollection, sourceObjectCollection);
      }

      private LookupPopulatorCollectionBehavior<EmployeeVM, ProjectVM, Project> CreateBehavior(
         Func<EmployeeVM, IEnumerable<ProjectVM>> lookupSourceProvider,
         IEnumerable<Project> sourceObjectCollection = null
      ) {
         var behavior = new LookupPopulatorCollectionBehavior<EmployeeVM, ProjectVM, Project>(lookupSourceProvider);

         var sourceAccessorStub = new Mock<IValueAccessorBehavior<IEnumerable<Project>>>();
         sourceAccessorStub
            .Setup(x => x.GetValue(It.IsAny<IBehaviorContext>()))
            .Returns(sourceObjectCollection ?? Enumerable.Empty<Project>());

         behavior.Successor = sourceAccessorStub.Object;

         return behavior;
      }

      public class Project {

      }

      public class EmployeeVM : ViewModel<EmployeeVMDescriptor> {

      }

      public class EmployeeVMDescriptor : VMDescriptor {
      }

      public class ProjectVM : ViewModel<ProjectVMDescriptor>, IHasSourceObject<Project> {
         public ProjectVM(Project source) {
            Source = source;
         }

         public Project Source {
            get;
            set;
         }

         public void InitializeFrom(Project source) {
            Source = source;
         }
      }

      public class ProjectVMDescriptor : VMDescriptor {
      }
   }
}