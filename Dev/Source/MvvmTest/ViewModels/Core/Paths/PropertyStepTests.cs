namespace Inspiring.MvvmTest.ViewModels.Core.Paths {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class PropertyStepTests : PathFixture {
      private EmployeeVM VM { get; set; }

      private EmployeeVMDescriptor Descriptor {
         get { return EmployeeVM.ClassDescriptor; }
      }

      [TestInitialize]
      public void Setup() {
         VM = new EmployeeVM();
         VM.GetValue(x => x.Projects).Add(new ProjectVM());
      }

      [TestMethod]
      public void Matches_WithMatchingViewModelPlusProperty_Succeeds() {
         AssertMatchIfNextSucceeds(
            CreateStep(x => x.SelectedProject),
            Path.Empty.Prepend(Descriptor.SelectedProject).Prepend(VM)
         );
      }

      [TestMethod]
      public void Matches_WithMatchingViewModelPlusChildViewModel_Succeeds() {
         AssertMatchIfNextSucceeds(
            CreateStep(x => x.SelectedProject),
            Path.Empty.Prepend(VM.GetValue(x => x.SelectedProject)).Prepend(VM)
         );
      }

      [TestMethod]
      public void Matches_WithMatchingViewModelPlusCollection_Succeeds() {
         AssertMatchIfNextSucceeds(
            CreateStep(x => x.Projects),
            Path.Empty.Prepend(VM.GetValue(x => x.Projects)).Prepend(VM)
         );
      }

      [TestMethod]
      public void Matches_WithMatchingViewModelPlusCollectionPlusItemViewModel_SucceedsAndMatchesTwoPathSteps() {
         IVMCollection<ProjectVM> collection = VM.GetValue(x => x.Projects);
         ProjectVM item = collection.First();

         AssertMatchIfNextSucceeds(
            CreateStep(x => x.Projects),
            Path.Empty.Prepend(item).Prepend(collection).Prepend(VM),
            expectedMatchLength: 2
         );
      }

      [TestMethod]
      public void Matches_WithEmptyPath_Fails() {
         AssertNoMatch(
            CreateStep(x => x.SelectedProject),
            Path.Empty
         );
      }

      [TestMethod]
      public void Matches_WithSingleMatchingViewModelOnly_Fails() {
         AssertNoMatch(
            CreateStep(x => x.SelectedProject),
            Path.Empty.Prepend(VM)
         );
      }

      [TestMethod]
      public void Matches_WithSingleMatchingPropertyOnly_ThrowsException() {
         AssertException(
            CreateStep(x => x.SelectedProject),
            Path.Empty.Prepend(Descriptor.SelectedProject)
         );
      }

      [TestMethod]
      public void Matches_WithNotMatchingViewModelPlusMatchingProperty_Fails() {
         var wrongVM = new ProjectVM();

         AssertNoMatch(
            CreateStep(x => x.SelectedProject),
            Path.Empty.Prepend(Descriptor.SelectedProject).Prepend(wrongVM)
         );
      }

      [TestMethod]
      public void Matches_WithMatchingViewModelPlusNotMatchingProperty_Fails() {
         AssertNoMatch(
            CreateStep(x => x.SelectedProject),
            Path.Empty.Prepend(Descriptor.Name).Prepend(VM)
         );
      }

      [TestMethod]
      public void Matches_WithMatchingCollectionAndProperty_Fails() {
         // I couldn't think of a concrete example where this feature is useful.

         var projectCollection = VM.GetValue(x => x.Projects);
         var projectNameProperty = ProjectVM.ClassDescriptor.Name;

         AssertException(
            CreateStep<ProjectVMDescriptor>(x => x.Name),
            Path.Empty.Prepend(projectNameProperty).Prepend(projectCollection)
         );
      }

      private PathDefinitionStep CreateStep(Func<EmployeeVMDescriptor, IVMPropertyDescriptor> propertySelector) {
         return CreateStep<EmployeeVMDescriptor>(propertySelector);
      }

      private PathDefinitionStep CreateStep<TDescriptor>(
         Func<TDescriptor, IVMPropertyDescriptor> propertySelector
      ) where TDescriptor : VMDescriptorBase {
         return new PropertyStep<TDescriptor>(propertySelector);
      }
   }
}