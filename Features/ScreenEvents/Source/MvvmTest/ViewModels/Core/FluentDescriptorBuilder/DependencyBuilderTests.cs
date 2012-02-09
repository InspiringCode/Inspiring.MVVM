namespace Inspiring.MvvmTest.ViewModels.Core.FluentDescriptorBuilder {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class DependencyBuilderTests {

      [TestMethod]
      public void DependencyWithRefreshAction_ForPropertiesOnRootVM_SetupDependencyWithEmptyTargetPath() {
         var descriptor = BuildDescriptor(b => b
            .OnChangeOf
            .Properties(x => x.Name)
            .Refresh
            .Properties(x => x.SelectedProject)
         );

         var behavior = descriptor.Behaviors.GetNextBehavior<DeclarativeDependencyBehavior>();

         var action = behavior.Dependencies.First().Action as RefreshAction;

         Assert.IsTrue(action.Target.Path.IsEmpty);
      }

      [TestMethod]
      public void Dependency_WithIncompleteConfiguration_ThrowsException() {
         AssertHelper.Throws<ArgumentException>(() => {
            var d = BuildDescriptor(b => b
               .OnChangeOf
               .Descendant(x => x.SelectedProject)
            );
         });
      }

      [TestMethod]
      public void Dependency_ForSelfWithExecuteAction_SetupsBehaviorProperly() {
         var descriptor = BuildDescriptor(b => b
            .OnChangeOf
            .Self
            .Execute((vm, args) => { })
         );

         var expectedSourcePath = PathDefinition
            .Empty
            .Append(new OptionalStep(new AnyPropertyStep<EmployeeVMDescriptor>()));

         var expectedChangeTypes = new ChangeType[] { 
            ChangeType.PropertyChanged, 
            ChangeType.ValidationResultChanged 
         };

         AssertDependencySetup<ExecuteAction<EmployeeVM>>(
            descriptor,
            expectedSourcePath,
            expectedChangeTypes
         );
      }

      [TestMethod]
      public void Dependency_ForCollectionWithRefreshAction_SetupsBehaviorProperly() {
         var descriptor = BuildDescriptor(b => b
            .OnChangeOf
            .Collection(x => x.Projects)
            .Refresh
            .Descendant(x => x.SelectedProject)
         );

         var expectedSourcePath = PathDefinition
            .Empty
            .AppendCollection<EmployeeVMDescriptor, IViewModelExpression<ProjectVMDescriptor>>(
               x => x.Projects
            );

         var expectedChangeTypes = new ChangeType[] { 
            ChangeType.RemovedFromCollection, 
            ChangeType.AddedToCollection 
         };

         var expectedTargetPath = PathDefinition
            .Empty
            .Append<EmployeeVMDescriptor, IViewModel<ProjectVMDescriptor>>(
               x => x.SelectedProject
            );

         AssertDependencySetup<RefreshAction>(
            descriptor,
            expectedSourcePath,
            expectedChangeTypes,
            expectedTargetPath
         );
      }

      [TestMethod]
      public void Dependency_ForCollectionIncludesPopilatedWithRefreshAction_SetupsBehaviorProperly() {
         var descriptor = BuildDescriptor(b => b
            .OnChangeOf
            .Collection(x => x.Projects, true)
            .Refresh
            .Descendant(x => x.SelectedProject)
         );

         var expectedSourcePath = PathDefinition
            .Empty
            .AppendCollection<EmployeeVMDescriptor, IViewModelExpression<ProjectVMDescriptor>>(
               x => x.Projects
            );

         var expectedChangeTypes = new ChangeType[] { 
            ChangeType.RemovedFromCollection, 
            ChangeType.AddedToCollection,
            ChangeType.CollectionPopulated
         };

         var expectedTargetPath = PathDefinition
            .Empty
            .Append<EmployeeVMDescriptor, IViewModel<ProjectVMDescriptor>>(
               x => x.SelectedProject
            );

         AssertDependencySetup<RefreshAction>(
            descriptor,
            expectedSourcePath,
            expectedChangeTypes,
            expectedTargetPath
         );
      }

      [TestMethod]
      public void Dependency_ForSelfOrAnyDescendantWithValidationActionForProperties_SetupsBehaviorProperly() {
         var descriptor = BuildDescriptor(b => b
            .OnChangeOf
            .Self
            .OrAnyDescendant
            .Revalidate
            .Descendant(x => x.Projects)
            .Properties(x => x.Customer, x => x.EndDate)
         );

         var expectedSourcePath = PathDefinition
            .Empty
            .Append(new OptionalStep(new AnyStepsStep<EmployeeVMDescriptor>()));

         var expectedChangeTypes = new ChangeType[] { 
            ChangeType.PropertyChanged, 
            ChangeType.ValidationResultChanged,
            ChangeType.AddedToCollection,
            ChangeType.RemovedFromCollection
         };

         var expectedTargetPath = PathDefinition
            .Empty
            .Append<EmployeeVMDescriptor, IVMCollectionExpression<IViewModelExpression<ProjectVMDescriptor>>>(
               x => x.Projects
            );

         AssertDependencySetup<ValidationAction>(
            descriptor,
            expectedSourcePath,
            expectedChangeTypes,
            expectedTargetPath
         );
      }

      [TestMethod]
      public void Dependency_ForPropertiesWithExecuteAction_SetupsBehaviorProperly() {
         var descriptor = BuildDescriptor(b => b
           .OnChangeOf
           .Properties(x => x.LastName, x => x.Name)
           .Execute((vm, args) => { })
         );

         var lastNameStep = CreatePropertyStep((EmployeeVMDescriptor x) => x.LastName);
         var nameStep = CreatePropertyStep((EmployeeVMDescriptor x) => x.Name);

         var expectedSourcePath = PathDefinition
            .Empty
            .Append(new OrStep(lastNameStep, nameStep));

         var expectedChangeTypes = new ChangeType[] { 
            ChangeType.PropertyChanged, 
            ChangeType.ValidationResultChanged 
         };

         AssertDependencySetup<ExecuteAction<EmployeeVM>>(
            descriptor,
            expectedSourcePath,
            expectedChangeTypes
         );
      }

      [TestMethod]
      public void Dependency_ForDescendantOfDescendantWithExecuteAction_SetupsBehaviorProperly() {
         var descriptor = BuildDescriptor(b => b
           .OnChangeOf
           .Descendant(x => x.Projects)
           .Descendant(x => x.Customer)
           .Execute((vm, args) => { })
         );

         var lastNameStep = CreatePropertyStep((EmployeeVMDescriptor x) => x.LastName);
         var nameStep = CreatePropertyStep((EmployeeVMDescriptor x) => x.Name);

         var expectedSourcePath = PathDefinition
            .Empty
            .Append<EmployeeVMDescriptor, IVMCollectionExpression<IViewModelExpression<ProjectVMDescriptor>>>(
               x => x.Projects
            )
            .Append<ProjectVMDescriptor, IViewModel<CustomerVMDescriptor>>(
               x => x.Customer
            )
            .Append(new OptionalStep(new AnyPropertyStep<CustomerVMDescriptor>()));

         var expectedChangeTypes = new ChangeType[] { 
            ChangeType.PropertyChanged, 
            ChangeType.ValidationResultChanged 
         };

         AssertDependencySetup<ExecuteAction<EmployeeVM>>(
            descriptor,
            expectedSourcePath,
            expectedChangeTypes
         );
      }

      private ChangeType[] AllChangeTypes() {
         List<ChangeType> types = new List<ChangeType>();
         foreach (ChangeType type in Enum.GetValues(typeof(ChangeType))) {
            types.Add(type);
         }
         return types.ToArray();
      }

      private PropertySelector<TDescriptor> CreatePropertySelector<TDescriptor>(
         Func<TDescriptor, IVMPropertyDescriptor> propertySelector
      ) where TDescriptor : IVMDescriptor {
         return new PropertySelector<TDescriptor>(propertySelector);
      }

      private PropertyStep<TDescriptor> CreatePropertyStep<TDescriptor>(
         Func<TDescriptor, IVMPropertyDescriptor> propertySelector
      ) where TDescriptor : IVMDescriptor {
         return new PropertyStep<TDescriptor>(propertySelector);
      }

      private void AssertDependencySetup<TAction>(
         IVMDescriptor descriptor,
         PathDefinition sourcePath,
         ChangeType[] changeTypes,
         PathDefinition targetPath = null
      ) where TAction : DependencyAction {
         var behavior = descriptor.Behaviors.GetNextBehavior<DeclarativeDependencyBehavior>();

         Assert.AreEqual(
            sourcePath.ToString(),
            behavior.Dependencies.First().SourcePath.ToString()
         );

         Assert.IsInstanceOfType(
            behavior.Dependencies.First().Action,
            typeof(TAction)
         );

         CollectionAssert.AreEquivalent(
            changeTypes,
            behavior.Dependencies.First().ChangeTypes
         );

         if (targetPath != null) {
            DependencyAction action = behavior.Dependencies.First().Action;
            if (action is ValidationAction) {
               Assert.AreEqual(
                  targetPath.ToString(),
                  ((ValidationAction)action).Target.Path.ToString()
               );
            } else if (action is RefreshAction) {
               Assert.AreEqual(
                  targetPath.ToString(),
                  ((RefreshAction)action).Target.Path.ToString()
               );
            } else {
               Assert.Fail("When asserting a target path the action have to be a RefreshAction or ValidationAction");
            }
         }
      }

      private EmployeeVMDescriptor BuildDescriptor(
         Action<IVMDependencyBuilder<EmployeeVM, EmployeeVMDescriptor>> configurationAction
      ) {
         return VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Name = v.Property.Of<string>();
               d.LastName = v.Property.Of<string>();
               d.SelectedProject = v.VM.Of<ProjectVM>();
               d.Projects = v.Collection.Of<ProjectVM>(new ProjectVMDescriptor());
            })
            .WithDependencies(configurationAction)
            .Build();
      }

      private sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> { }

      private sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<string> LastName { get; set; }
         public IVMPropertyDescriptor<ProjectVM> SelectedProject { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
      }

      private sealed class ProjectVM : ViewModel<ProjectVMDescriptor> { }

      private sealed class ProjectVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<DateTime> EndDate { get; set; }
         public IVMPropertyDescriptor<CustomerVM> Customer { get; set; }
      }

      private sealed class CustomerVM : ViewModel<CustomerVMDescriptor> { }

      private sealed class CustomerVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<double> Rating { get; set; }
      }
   }
}