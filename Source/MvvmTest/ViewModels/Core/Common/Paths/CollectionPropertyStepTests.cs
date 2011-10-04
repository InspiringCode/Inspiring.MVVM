namespace Inspiring.MvvmTest.ViewModels.Core.Common.Paths {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionPropertyStepTests : PathFixture {
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
      public void Matches_WithMatchingCollectionPlusProperty_Succeeds() {
         var projects = VM.GetValue(x => x.Projects);

         AssertMatchIfNextSucceeds(
            CreateStep<ProjectVMDescriptor, string>(x => x.Name),
            Path.Empty.Append(projects).Append(ProjectVM.ClassDescriptor.Name)
         );
      }

      [TestMethod]
      public void Matches_WithMatchingCollectionPlusNonMatchingProperty_Fails() {
         var projects = VM.GetValue(x => x.Projects);

         AssertNoMatch(
            CreateStep<ProjectVMDescriptor, string>(x => x.Name),
            Path.Empty.Append(projects).Append(ProjectVM.ClassDescriptor.EndDate)
         );
      }

      [TestMethod]
      public void Matches_WithNonMatchingCollectionPlusProperty_Fails() {
         var nonMatchingCollection = VM.GetValue(x => x.Managers);
         var nonMatchingProperty = ProjectVM.ClassDescriptor.EndDate;

         AssertNoMatch(
            CreateStep<ProjectVMDescriptor, string>(x => x.Name),
            Path.Empty.Append(nonMatchingCollection).Append(nonMatchingProperty)
         );
      }

      [TestMethod]
      public void Matches_WithViewModelPlusCollection_Fails() {
         AssertNoMatch(
            CreateStep(x => x.Projects),
            Path.Empty.Append(VM).Append(VM.GetValue(x => x.Projects))
         );
      }

      [TestMethod]
      public void Matches_WithViewModelPlusProperty_Fails() {
         AssertNoMatch(
            CreateStep(x => x.SelectedProject),
            Path.Empty.Prepend(Descriptor.SelectedProject).Prepend(VM)
         );
      }

      [TestMethod]
      public void Matches_WithViewModelPlusViewModel_Fails() {
         AssertNoMatch(
             CreateStep(x => x.SelectedProject),
             Path.Empty.Append(VM).Append(VM.GetValue(x => x.SelectedProject))
         );
      }

      //[TestMethod]
      //public void Matches_WithSingleViewModel_Fails() {

      //}

      //[TestMethod]
      //public void Matches_WithSingleProperty_Fails() {

      //}

      //[TestMethod]
      //public void Matches_WithSingleCollection_Fails() {

      //}

      //[TestMethod]
      //public void Matches_WithEmptyPath_Fails() {

      //}

      [TestMethod]
      public void ToString_ReturnsPropertyName() {
         var step = CreateStep(x => x.Projects);
         Assert.AreEqual("EmployeeVMDescriptor.Projects", step.ToString(isFirst: true));
      }

      private PathDefinitionStep CreateStep<TValue>(Func<EmployeeVMDescriptor, IVMPropertyDescriptor<TValue>> propertySelector) {
         return CreateStep<EmployeeVMDescriptor, TValue>(propertySelector);
      }

      private PathDefinitionStep CreateStep<TDescriptor, TValue>(
         Func<TDescriptor, IVMPropertyDescriptor<TValue>> propertySelector
      ) where TDescriptor : IVMDescriptor {
         return new CollectionPropertyStep<TDescriptor, TValue>(propertySelector);
      }
   }
}