namespace Inspiring.MvvmTest.ViewModels.Core.Common.Paths {
   using System;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionStepTests : PathFixture {
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
      public void Matches_WithMatchingViewModelPlusCollection_Succeeds() {
         var projects = VM.GetValue(x => x.Projects);

         AssertMatchIfNextSucceeds(
            CreateProjectsStep(),
            Path.Empty.Append(VM).Append(projects)
         );
      }

      [TestMethod]
      public void Matches_WithViewModelPlusItem_Fails() {
         var projects = VM.GetValue(x => x.Projects);

         AssertNoMatch(
            CreateProjectsStep(),
            Path.Empty.Append(VM).Append(projects.First())
         );
      }

      //[TestMethod]
      //public void Matches_WithMatchingCollectionPlusProperty_Succeeds() {
      //   var projects = VM.GetValue(x => x.Projects);

      //   AssertMatchIfNextSucceeds(
      //      CreateStep<ProjectVMDescriptor, string>(x => x.Name),
      //      Path.Empty.Append(projects).Append(ProjectVM.ClassDescriptor.Name)
      //   );
      //}

      //[TestMethod]
      //public void Matches_WithMatchingCollectionPlusNonMatchingProperty_Fails() {
      //   var projects = VM.GetValue(x => x.Projects);

      //   AssertNoMatch(
      //      CreateStep<ProjectVMDescriptor, string>(x => x.Name),
      //      Path.Empty.Append(projects).Append(ProjectVM.ClassDescriptor.EndDate)
      //   );
      //}

      //[TestMethod]
      //public void Matches_WithNonMatchingCollectionPlusProperty_Fails() {
      //   var nonMatchingCollection = VM.GetValue(x => x.Managers);
      //   var nonMatchingProperty = ProjectVM.ClassDescriptor.EndDate;

      //   AssertNoMatch(
      //      CreateStep<ProjectVMDescriptor, string>(x => x.Name),
      //      Path.Empty.Append(nonMatchingCollection).Append(nonMatchingProperty)
      //   );
      //}

      //[TestMethod]
      //public void Matches_WithViewModelPlusCollection_Fails() {
      //   AssertNoMatch(
      //      CreateStep(x => x.Projects),
      //      Path.Empty.Append(VM).Append(VM.GetValue(x => x.Projects))
      //   );
      //}

      //[TestMethod]
      //public void Matches_WithViewModelPlusProperty_Fails() {
      //   AssertNoMatch(
      //      CreateStep(x => x.SelectedProject),
      //      Path.Empty.Prepend(Descriptor.SelectedProject).Prepend(VM)
      //   );
      //}

      //[TestMethod]
      //public void Matches_WithViewModelPlusViewModel_Fails() {
      //   AssertNoMatch(
      //       CreateStep(x => x.SelectedProject),
      //       Path.Empty.Append(VM).Append(VM.GetValue(x => x.SelectedProject))
      //   );
      //}

      [TestMethod]
      public void ToString_ReturnsDescriptorNameAndPropertyType() {
         var step = CreateStep<EmployeeVMDescriptor, ProjectVM>(x => x.Projects);
         Assert.AreEqual("EmployeeVMDescriptor -> IVMCollection<ProjectVM>", step.ToString());
      }

      //private PathDefinitionStep CreateStep<TItemVM>(
      //   Func<EmployeeVMDescriptor, IVMPropertyDescriptor<TValue>> propertySelector
      //) where TItemVM : IViewModel {
      //   return CreateStep<EmployeeVMDescriptor, TValue>(propertySelector);
      //}

      private PathDefinitionStep CreateProjectsStep() {
         return CreateStep<EmployeeVMDescriptor, ProjectVM>(x => x.Projects);
      }

      private PathDefinitionStep CreateStep<TDescriptor, TItemVM>(
         Func<TDescriptor, IVMPropertyDescriptor<IVMCollection<TItemVM>>> propertySelector
      )
         where TDescriptor : VMDescriptorBase
         where TItemVM : IViewModel {
         return new CollectionStep<TDescriptor, TItemVM>(propertySelector);
      }
   }
}