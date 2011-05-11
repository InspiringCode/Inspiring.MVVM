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
            .Execute(() => { })
         );

         var expectedSourcePath = PathDefinition
            .Empty
            .Append(new OptionalStep(new AnyPropertyStep<EmployeeVMDescriptor>()));

         var expectedChangeTypes = AllChangeTypes();

         AssertDependencySetup<ExecuteAction>(
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
            .Append<EmployeeVMDescriptor, IVMCollectionExpression<IViewModelExpression<ProjectVMDescriptor>>>(
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

      private ChangeType[] AllChangeTypes() {
         List<ChangeType> types = new List<ChangeType>();
         foreach (ChangeType type in Enum.GetValues(typeof(ChangeType))) {
            types.Add(type);
         }
         return types.ToArray();
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
                  ((ValidationAction)action).TargetPath.ToString()
               );
            } else if (action is RefreshAction) {
               Assert.AreEqual(
                  targetPath.ToString(),
                  ((RefreshAction)action).TargetPath.ToString()
               );
            } else {
               Assert.Fail("When asserting a target path the action have to be a RefreshAction or ValidationAction");
            }
         }
      }

      //public void MyTestMethod() {
      //   var d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Properties(x => x.Name, x => x.LastName)
      //      .Refresh
      //      .Properties(x => x.Projects)
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Descendant(x => x.SelectedProject)
      //      .Refresh
      //      .Properties(x => x.Name)
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Descendant(x => x.Projects)
      //      .Descendant(x => x.Customer)
      //      .Properties(x => x.Rating)
      //      .Refresh
      //      .Properties(x => x.Name)
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Descendant(x => x.Projects)
      //      .Properties(x => x.Customer)
      //      .Revalidate
      //      .Descendant(x => x.SelectedProject)
      //      .Properties(x => x.EndDate)
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Descendant(x => x.Projects)
      //      .Properties(x => x.Customer)
      //      .Revalidate
      //      .Descendant(x => x.Projects)
      //      .Properties(x => x.EndDate)
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Self
      //      .Execute(() => { })
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Self
      //      .OrAnyDescendant
      //      .Execute(() => { })
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Collection(x => x.Projects)
      //      .Execute(() => { })
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Descendant(x => x.SelectedProject)
      //      .OrAnyDescendant
      //      .Execute(() => { })
      //   );
      //}

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