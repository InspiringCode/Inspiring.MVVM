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
      public void Matches_WithMatchingCollectionPlusProperty_Succeeds() {
         IVMCollection<ProjectVM> collection = VM.GetValue(x => x.Projects);

         AssertMatchIfNextSucceeds(
            CreateStep((ProjectVMDescriptor x) => x.Name),
            Path.Empty.Prepend(ProjectVM.ClassDescriptor.Name).Prepend(collection)
         );
      }

      [TestMethod]
      public void Matches_WithNonMatchingCollectionPlusProperty_Fails() {
         IVMCollection<EmployeeVM> nonMatching = VM.GetValue(x => x.Managers);

         AssertNoMatch(
            CreateStep((ProjectVMDescriptor x) => x.Name),
            Path.Empty.Prepend(ProjectVM.ClassDescriptor.Name).Prepend(nonMatching)
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
      public void Matches_WithSingleCollection_Fails() {
         // This would be the case, if the definiton [EmployeeVM -> Projects, ProjectVM -> Name]
         // is matched against the path [employeeVM, projects].
         AssertNoMatch(
            CreateStep<ProjectVMDescriptor, string>(x => x.Name),
            Path.Empty.Prepend(VM.GetValue(x => x.Projects))
         );
      }

      [TestMethod]
      public void Matches_WithSingleViewModel_Fails() {
         // This would be the case, if the definiton [EmployeeVM -> SelectedProject, ProjectVM -> Name]
         // is matched against the path [employeeVM, projectVM].
         AssertNoMatch(
            CreateStep<ProjectVMDescriptor, string>(x => x.Name),
            Path.Empty.Prepend(VM.GetValue(x => x.SelectedProject))
         );
      }

      [TestMethod]
      public void Matches_WithSingleProperty_ThrowsException() {
         // I couldn't think of an valid definition/path constellation where this
         // makes sense.
         AssertException(
            CreateStep<ProjectVMDescriptor, string>(x => x.Name),
            Path.Empty.Prepend(Descriptor.Projects)
         );
      }

      [TestMethod]
      public void ToString_ReturnsDescriptorNameAndPropertyType() {
         var step = CreateStep(x => x.Projects);
         Assert.AreEqual("EmployeeVMDescriptor -> IVMCollection<ProjectVM>", step.ToString());
      }

      private PathDefinitionStep CreateStep<TValue>(Func<EmployeeVMDescriptor, IVMPropertyDescriptor<TValue>> propertySelector) {
         return CreateStep<EmployeeVMDescriptor, TValue>(propertySelector);
      }

      private PathDefinitionStep CreateStep<TDescriptor, TValue>(
         Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector
      ) where TDescriptor : VMDescriptorBase {
         return new PropertyStep<TDescriptor, TValue>(propertySelector);
      }
   }
}