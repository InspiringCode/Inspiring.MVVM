namespace Inspiring.MvvmTest.ViewModels.Core.Builder {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   // TODO: Clean me up a bit please.
   [TestClass]
   public class VMPropertyBuilderTests : TestBase {
      private VMDescriptorConfiguration Configuration { get; set; }
      private VMPropertyBuilder<EmployeeVM, Employee> SourceObjectFactory { get; set; }
      private VMPropertyBuilder<EmployeeVM, EmployeeVM> RootFactory { get; set; }

      [TestInitialize]
      public void Setup() {
         Configuration = new VMDescriptorConfiguration(new BehaviorChainConfiguration());

         SourceObjectFactory = new VMPropertyBuilder<EmployeeVM, Employee>(
            PropertyPath.Create((EmployeeVM x) => x.SourcePerson),
            Configuration
         );

         RootFactory = new VMPropertyBuilder<EmployeeVM, EmployeeVM>(
            PropertyPath.Empty<EmployeeVM>(),
            Configuration
         );
      }

      [TestMethod]
      public void MappedProperty_RootFactory_InsertsProperBehaviors() {
         var p = RootFactory.Property.MapsTo(x => x.SourcePerson.Name);

         AssertBasicPropertyBehaviors(p);
         Assert.IsTrue(ContainsBehavior<MappedValueAccessorBehavior<EmployeeVM, string>>(p));
      }

      [TestMethod]
      public void CalculatedProperty_RootFactory_InsertsProperBehaviors() {
         var p = RootFactory.Property.DelegatesTo(x => x.SourcePerson.Name);

         AssertBasicPropertyBehaviors(p);
         Assert.IsTrue(ContainsBehavior<DelegateValueAccessor<EmployeeVM, EmployeeVM, string>>(p));
      }

      [TestMethod]
      public void LocalProperty_RootFactory_InsertsProperBehaviors() {
         var p = RootFactory.Property.Of<string>();

         AssertBasicPropertyBehaviors(p);
         Assert.IsTrue(ContainsBehavior<StoredValueAccessorBehavior<string>>(p));
      }

      [TestMethod]
      public void ViewModelProperty_WrapsExpression_InsertsProperBehaviors() {
         var p = RootFactory.VM.Wraps(x => x.SourcePerson.CurrentProject).With<ProjectVM>();

         AssertDefaultViewModelBehaviors(p);
         Assert.IsTrue(ContainsBehavior<WrapperViewModelAccessorBehavior<ProjectVM, Project>>(p));
         Assert.IsTrue(ContainsBehavior<MappedValueAccessorBehavior<EmployeeVM, Project>>(p));
         Assert.IsTrue(ContainsBehavior<ServiceLocatorValueFactoryBehavior<ProjectVM>>(p));
      }

      [TestMethod]
      public void ViewModelProperty_WrapsDelegates_InsertsProperBehaviors() {
         var p = RootFactory.VM.Wraps(x => x.SourcePerson.CurrentProject, setter: null).With<ProjectVM>();

         AssertDefaultViewModelBehaviors(p);
         Assert.IsTrue(ContainsBehavior<WrapperViewModelAccessorBehavior<ProjectVM, Project>>(p));
         Assert.IsTrue(ContainsBehavior<DelegateValueAccessor<EmployeeVM, EmployeeVM, Project>>(p));
         Assert.IsTrue(ContainsBehavior<ServiceLocatorValueFactoryBehavior<ProjectVM>>(p));
      }

      [TestMethod]
      public void ViewModelProperty_DelegatesTo_InsertsProperBehaviors() {
         var p = RootFactory.VM.DelegatesTo(x => new ProjectVM());

         AssertDefaultViewModelBehaviors(p);
         Assert.IsTrue(ContainsBehavior<DelegateViewModelAccessorBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<DelegateValueAccessor<EmployeeVM, EmployeeVM, ProjectVM>>(p));
      }

      [TestMethod]
      public void ViewModelProperty_InstanceProperty_InsertsProperBehaviors() {
         var p = RootFactory.VM.Of<ProjectVM>();

         AssertDefaultViewModelBehaviors(p);
         Assert.IsTrue(ContainsBehavior<StoredValueAccessorBehavior<ProjectVM>>(p));
      }

      [TestMethod]
      public void CollectionProperty_Wraps_InsertsPropertyBehaviors() {
         var p = RootFactory.Collection.Wraps(x => x.SourcePerson.Projects).With<ProjectVM>(new VMDescriptorStub());

         AssertDefaultCollectionPropertyBehaviors(p);
         Assert.IsTrue(ContainsBehavior<WrapperCollectionAccessorBehavior<ProjectVM, Project>>(p));
         Assert.IsTrue(ContainsBehavior<DelegateValueAccessor<EmployeeVM, EmployeeVM, IEnumerable<Project>>>(p));
         Assert.IsTrue(ContainsBehavior<ServiceLocatorValueFactoryBehavior<ProjectVM>>(p));
      }

      [TestMethod]
      public void CollectionProperty_PopulatedWith_InsertsPropertyBehaviors() {
         var p = RootFactory.Collection.PopulatedWith(x => Enumerable.Empty<ProjectVM>()).With(new VMDescriptorStub());

         AssertDefaultCollectionPropertyBehaviors(p);
         Assert.IsTrue(ContainsBehavior<PopulatedCollectionAccessorBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<DelegateValueAccessor<EmployeeVM, EmployeeVM, IEnumerable<ProjectVM>>>(p));
      }

      [TestMethod]
      public void CollectionProperty_Instance_InsertsPropertyBehaviors() {
         var p = RootFactory.Collection.Of<ProjectVM>(new VMDescriptorStub());

         AssertDefaultCollectionPropertyBehaviors(p);
         Assert.IsTrue(ContainsBehavior<StoredCollectionAccessorBehavior<ProjectVM>>(p));
      }

      [TestMethod]
      public void CommandProperty_InsertsProperBehaviors() {
         var p = RootFactory.Command(x => { });

         AssertBasicPropertyBehaviors(p);
         Assert.IsTrue(ContainsBehavior<DelegateCommandExecutorBehavior<EmployeeVM>>(p));
         Assert.IsTrue(ContainsBehavior<CommandAccessorBehavior>(p));
         Assert.IsTrue(ContainsBehavior<MappedValueAccessorBehavior<EmployeeVM, EmployeeVM>>(p));
      }

      private void AssertBasicPropertyBehaviors<T>(IVMPropertyDescriptor<T> p) {
         Assert.IsTrue(ContainsBehavior<PropertyDescriptorProviderBehavior>(p));
         Assert.IsTrue(ContainsBehavior<DisplayValueAccessorBehavior<T>>(p));
         Assert.IsTrue(ContainsBehavior<UntypedPropertyAccessorBehavior<T>>(p));
      }

      private void AssertDefaultViewModelBehaviors<T>(IVMPropertyDescriptor<T> p) {
         AssertBasicPropertyBehaviors(p);
      }

      private void AssertDefaultCollectionPropertyBehaviors<T>(IVMPropertyDescriptor<IVMCollection<T>> p) where T : IViewModel {
         AssertBasicPropertyBehaviors<IVMCollection<T>>(p);
         Assert.IsTrue(ContainsBehavior<CollectionFactoryBehavior<T>>(p));
         Assert.IsTrue(ContainsBehavior<ItemDescriptorProviderBehavior>(p));
      }

      private bool ContainsBehavior<T>(IVMPropertyDescriptor property) where T : IBehavior {
         var config = Configuration.PropertyConfigurations[property];
         BehaviorChain chain = config.CreateChain();

         T behavior;
         return chain.TryGetBehavior<T>(out behavior);
      }

      private class EmployeeVM : ViewModelStub {
         public Employee SourcePerson { get; private set; }
      }

      private class ProjectVM : ViewModelStub, IHasSourceObject<Project> {
         public Project SourceProject { get; private set; }

         public Project Source {
            get { return SourceProject; }
            set { SourceProject = value; }
         }

         public void InitializeFrom(Project source) {
            SourceProject = source;
         }
      }

      private class Employee {
         public string Name { get; private set; }
         public Project CurrentProject { get; set; }
         public List<Project> Projects { get; set; }
      }

      private class Project {
         public string Title { get; set; }
      }
   }
}