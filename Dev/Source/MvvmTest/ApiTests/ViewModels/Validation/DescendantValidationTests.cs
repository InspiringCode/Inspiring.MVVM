namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class DescendantValidationTests : TestBase {
      [TestMethod]
      public void FirstAccess_OfWrappingCollection_ValidatesItems() {
         var vm = new EmployeeVM();
         vm.WrappedProjectsSource.Add(new Project("Project 1"));

         var collectionItem = vm.ProjectsWrapped.First();

         Assert.IsTrue(collectionItem.WasValidated);
      }

      [TestMethod]
      public void FirstAccess_OfPopulatedCollection_ValidatesItems() {
         var vm = new EmployeeVM();
         vm.PopulatedProjectsSource.Add(new ProjectVM());

         var collectionItem = vm.ProjectsPopulated.First();

         Assert.IsTrue(collectionItem.WasValidated);
      }

      [TestMethod]
      public void FirstAccess_OfWrappingVM_ValidatesChildVM() {
         var vm = new EmployeeVM();
         vm.WrappedCurrentProjectSource = new Project("Project 1");

         Assert.IsTrue(vm.CurrentProjectWrapped.WasValidated);
      }

      [TestMethod]
      public void FirstAccess_OfDelegatedVM_ValidatesChildVM() {
         var vm = new EmployeeVM();
         vm.DelegatedCurrentProjectSource = new ProjectVM();

         Assert.IsTrue(vm.CurrentProjectDelegated.WasValidated);
      }

      [TestMethod]
      public void GetValidationState_Children_AfterInvalidChildWasAdded_ReturnsError() {
         ProjectVM childItem = new ProjectVM();
         childItem.Source = new Project("Project 1");
         childItem.Revalidate();

         var itemBeforeAdditionState = childItem.GetValidationState(ValidationResultScope.All);
         throw new NotImplementedException("Fix assertion");
         Contract.Assert(!itemBeforeAdditionState.IsValid);

         EmployeeVM vm = new EmployeeVM();
         var parentBeforeAdditionState = vm.GetValidationState(ValidationResultScope.All);
         Contract.Assert(parentBeforeAdditionState.IsValid);

         vm.ProjectsWrapped.Add(childItem);

         Assert.IsFalse(vm.GetValidationState(ValidationResultScope.Descendants).IsValid);
      }

      [TestMethod]
      public void GetValidationState_Children_AfterInvalidChildWasAssigned_ReturnsError() {
         ProjectVM childItem = new ProjectVM();
         childItem.Source = new Project("Project 1");
         childItem.Revalidate();

         var itemBeforeAdditionState = childItem.GetValidationState(ValidationResultScope.All);
         throw new NotImplementedException("Fix assertion");
         Contract.Assert(!itemBeforeAdditionState.IsValid);

         EmployeeVM vm = new EmployeeVM();
         var parentBeforeAdditionState = vm.GetValidationState(ValidationResultScope.All);
         Contract.Assert(parentBeforeAdditionState.IsValid);

         vm.CurrentProjectWrapped = childItem;

         Assert.IsFalse(vm.GetValidationState(ValidationResultScope.Descendants).IsValid);
      }

      private sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, c) => {
               var vm = c.GetPropertyBuilder();

               d.CurrentProjectWrapped = vm
                  .VM
                  .Wraps(x => x.WrappedCurrentProjectSource)
                  .With<ProjectVM>();

               d.CurrentProjectDelegated = vm
                  .VM
                  .DelegatesTo(x => x.DelegatedCurrentProjectSource);

               d.ProjectsWrapped = vm
                  .Collection
                  .Wraps(x => x.WrappedProjectsSource)
                  .With<ProjectVM>(ProjectVM.ClassDescriptor);

               d.ProjectsPopulated = vm
                  .Collection
                  .PopulatedWith(x => x.PopulatedProjectsSource)
                  .With(ProjectVM.ClassDescriptor);
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
            WrappedProjectsSource = new List<Project>();
            PopulatedProjectsSource = new List<ProjectVM>();
         }

         public Project WrappedCurrentProjectSource { get; set; }

         public ProjectVM DelegatedCurrentProjectSource { get; set; }

         public List<Project> WrappedProjectsSource { get; set; }

         public List<ProjectVM> PopulatedProjectsSource { get; set; }

         public ProjectVM CurrentProjectWrapped {
            get { return GetValue(Descriptor.CurrentProjectWrapped); }
            set { SetValue(Descriptor.CurrentProjectWrapped, value); }
         }

         public ProjectVM CurrentProjectDelegated {
            get { return GetValue(Descriptor.CurrentProjectDelegated); }
            set { SetValue(Descriptor.CurrentProjectDelegated, value); }
         }

         public IVMCollection<ProjectVM> ProjectsWrapped {
            get { return GetValue(Descriptor.ProjectsWrapped); }
         }

         public IVMCollection<ProjectVM> ProjectsPopulated {
            get { return GetValue(Descriptor.ProjectsPopulated); }
         }
      }

      private sealed class ProjectVM : ViewModel<ProjectVMDescriptor>, IHasSourceObject<Project> {
         public static readonly ProjectVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ProjectVMDescriptor>()
            .For<ProjectVM>()
            .WithProperties((d, c) => {
               var p = c.GetPropertyBuilder(x => x.Source);
               d.Title = p.Property.MapsTo(x => x.Title);
            })
            .WithValidators(b => {
               b.Check(x => x.Title).Custom(args => {
                  args.AddError("Error");
                  args.Owner.WasValidated = true;
               });
            })
            .Build();

         public ProjectVM()
            : base(ClassDescriptor) {
         }

         public Project Source { get; set; }

         public bool WasValidated { get; set; }

         public void Revalidate() {
            base.Revalidate(ValidationScope.SelfAndAllDescendants, ValidationMode.DiscardInvalidValues);
         }
      }

      private sealed class ProjectVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Title { get; set; }
      }

      private sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ProjectVM> CurrentProjectWrapped { get; set; }
         public IVMPropertyDescriptor<ProjectVM> CurrentProjectDelegated { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> ProjectsWrapped { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> ProjectsPopulated { get; set; }
      }
   }
}