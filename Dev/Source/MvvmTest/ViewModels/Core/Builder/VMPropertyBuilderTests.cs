namespace Inspiring.MvvmTest.ViewModels.Core.Builder {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Core.Behaviors;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   // TODO: Clean me up a bit please.
   [TestClass]
   public class VMPropertyBuilderTests : TestBase {
      public VMDescriptorConfiguration Configuration { get; private set; }

      [TestInitialize]
      public void Setup() {
         var viewModelConfiguration = BehaviorChainConfiguration.GetConfiguration(
            DefaultBehaviorChainTemplateKeys.ViewModel,
            BehaviorFactoryConfigurations.ForViewModel<EmployeeVM>()
         );

         Configuration = new VMDescriptorConfiguration(viewModelConfiguration);
      }

      [TestMethod]
      public void MappedProperty_RootFactory_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.Property.MapsTo(x => x.SourcePerson.Name);

         Assert.IsTrue(ContainsBehavior<MappedValueAccessorBehavior<EmployeeVM, string>>(p));
         Assert.IsTrue(ContainsBehavior<RefreshBehavior.SimpleProperty<string>>(p));
         AssertDefaultPropertyBehaviors(p);
      }

      [TestMethod]
      public void CalculatedProperty_RootFactory_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.Property.DelegatesTo(x => x.SourcePerson.Name);

         Assert.IsTrue(ContainsBehavior<DelegateValueAccessor<EmployeeVM, EmployeeVM, string>>(p));
         Assert.IsTrue(ContainsBehavior<RefreshBehavior.SimpleProperty<string>>(p));
         AssertDefaultPropertyBehaviors(p);
      }

      [TestMethod]
      public void LocalProperty_RootFactory_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.Property.Of<string>();

         Assert.IsTrue(ContainsBehavior<StoredValueAccessorBehavior<string>>(p));
         Assert.IsFalse(ContainsBehavior<RefreshBehavior.SimpleProperty<string>>(p));
         AssertDefaultPropertyBehaviors(p);
      }

      [TestMethod]
      public void ViewModelProperty_WrapsExpression_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.VM.Wraps(x => x.SourcePerson.CurrentProject).With<ProjectVM>();

         AssertDefaultViewModelBehaviors(p);
         Assert.IsTrue(ContainsBehavior<MappedValueAccessorBehavior<EmployeeVM, Project>>(p));
         Assert.IsTrue(ContainsBehavior<ServiceLocatorValueFactoryBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<WrapperViewModelAccessorBehavior<ProjectVM, Project>>(p));
         Assert.IsTrue(ContainsBehavior<ValueCacheBehaviorOld<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<RefreshBehavior.ViewModelProperty<ProjectVM>>(p));
      }

      [TestMethod]
      public void ViewModelProperty_WrapsDelegates_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.VM.Wraps(x => x.SourcePerson.CurrentProject, setter: null).With<ProjectVM>();

         AssertDefaultViewModelBehaviors(p);
         Assert.IsTrue(ContainsBehavior<DelegateValueAccessor<EmployeeVM, EmployeeVM, Project>>(p));
         Assert.IsTrue(ContainsBehavior<ServiceLocatorValueFactoryBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<WrapperViewModelAccessorBehavior<ProjectVM, Project>>(p));
         Assert.IsTrue(ContainsBehavior<ValueCacheBehaviorOld<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<RefreshBehavior.ViewModelProperty<ProjectVM>>(p));
      }

      [TestMethod]
      public void ViewModelProperty_InstanceProperty_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.VM.Of<ProjectVM>();

         AssertDefaultViewModelBehaviors(p);
         Assert.IsTrue(ContainsBehavior<StoredValueAccessorBehavior<ProjectVM>>(p));
         Assert.IsFalse(ContainsBehavior<ServiceLocatorValueFactoryBehavior<ProjectVM>>(p));
         Assert.IsFalse(ContainsBehavior<WrapperViewModelAccessorBehavior<ProjectVM, Project>>(p));
         Assert.IsFalse(ContainsBehavior<ValueCacheBehaviorOld<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<RefreshBehavior.ViewModelProperty<ProjectVM>>(p));
      }

      [TestMethod]
      public void CollectionProperty_Wraps_InsertsPropertyBehaviors() {
         var f = CreateRootFactory();
         var p = f.Collection.Wraps(x => x.SourcePerson.Projects).With<ProjectVM>(new VMDescriptorStub());

         AssertDefaultCollectionPropertyBehaviors(p);
         //Assert.IsTrue(ContainsBehavior<CollectionPopulatorBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<ValueCacheBehaviorOld<IVMCollection<ProjectVM>>>(p));
         Assert.IsTrue(ContainsBehavior<RefreshBehavior.PopulatedCollectionProperty<ProjectVM>>(p));

         //Assert.IsTrue(ContainsCollectionBehavior<PopulatorCollectionBehavior<ProjectVM, Project>>(p));
         Assert.IsTrue(ContainsCollectionBehavior<ServiceLocatorValueFactoryBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsCollectionBehavior<DelegateValueAccessor<EmployeeVM, EmployeeVM, IEnumerable<Project>>>(p));
      }

      [TestMethod]
      public void CollectionProperty_Initialized_InsertsPropertyBehaviors() {
         var f = CreateRootFactory();
         var p = f.Collection.PopulatedWith(x => Enumerable.Empty<ProjectVM>()).With(new VMDescriptorStub());

         AssertDefaultCollectionPropertyBehaviors(p);
         //Assert.IsTrue(ContainsBehavior<CollectionPopulatorBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<ValueCacheBehaviorOld<IVMCollection<ProjectVM>>>(p));
         Assert.IsTrue(ContainsBehavior<RefreshBehavior.PopulatedCollectionProperty<ProjectVM>>(p));

         //Assert.IsTrue(ContainsCollectionBehavior<DelegatePopulatorCollectionBehavior<ProjectVM, EmployeeVM>>(p));
         Assert.IsTrue(ContainsCollectionBehavior<MappedValueAccessorBehavior<EmployeeVM, EmployeeVM>>(p));
         Assert.IsFalse(ContainsCollectionBehavior<ServiceLocatorValueFactoryBehavior<ProjectVM>>(p));
      }

      [TestMethod]
      public void CollectionProperty_Instance_InsertsPropertyBehaviors() {
         var f = CreateRootFactory();
         var p = f.Collection.Of<ProjectVM>(new VMDescriptorStub());

         AssertDefaultCollectionPropertyBehaviors(p);
         //Assert.IsFalse(ContainsBehavior<CollectionPopulatorBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<RefreshBehavior.CollectionInstanceProperty<ProjectVM>>(p));

         //Assert.IsFalse(ContainsCollectionBehavior<PopulatorCollectionBehavior<ProjectVM, Project>>(p));
         Assert.IsFalse(ContainsCollectionBehavior<MappedValueAccessorBehavior<EmployeeVM, EmployeeVM>>(p));
         Assert.IsFalse(ContainsCollectionBehavior<ServiceLocatorValueFactoryBehavior<ProjectVM>>(p));
      }

      [TestMethod]
      public void ViewModelProperty_DelegatesTo_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.VM.DelegatesTo(x => new ProjectVM());

         AssertDefaultViewModelBehaviors(p);
         Assert.IsTrue(ContainsBehavior<DelegateValueAccessor<EmployeeVM, EmployeeVM, ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<ValueCacheBehaviorOld<ProjectVM>>(p));
         Assert.IsFalse(ContainsBehavior<WrapperViewModelAccessorBehavior<ProjectVM, Project>>(p));
      }

      private void AssertDefaultPropertyBehaviors(IVMPropertyDescriptor p) {
         Assert.IsTrue(ContainsBehavior<PropertyDescriptorProviderBehavior>(p));

         //Assert.IsFalse(ContainsBehavior<PropertyValidationBehavior<string>>(p));
         Assert.IsFalse(ContainsBehavior<RefreshableValueCacheBehavior<string>>(p));
      }

      private void AssertDefaultViewModelBehaviors(IVMPropertyDescriptor p) {
         Assert.IsTrue(ContainsBehavior<DisplayValueAccessorBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<ParentSetterBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<PropertyDescriptorProviderBehavior>(p));
      }

      private void AssertDefaultCollectionPropertyBehaviors(IVMPropertyDescriptor p) {
         Assert.IsTrue(ContainsBehavior<IDisplayValueAccessorBehavior>(p));
         Assert.IsTrue(ContainsBehavior<ValueCacheBehaviorOld<IVMCollection<ProjectVM>>>(p));
         //Assert.IsTrue(ContainsBehavior<CollectionFactoryBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<PropertyDescriptorProviderBehavior>(p));

         Assert.IsTrue(ContainsCollectionBehavior<ItemDescriptorProviderBehavior>(p));
         Assert.IsTrue(ContainsCollectionBehavior<ItemInitializerBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsCollectionBehavior<ChangeNotifierCollectionBehavior<ProjectVM>>(p));

      }

      private bool ContainsBehavior<T>(IVMPropertyDescriptor property) where T : IBehavior {
         var config = Configuration.PropertyConfigurations[property];
         BehaviorChain chain = config.CreateChain();

         T behavior;
         return chain.TryGetBehavior<T>(out behavior);
      }

      private bool ContainsCollectionBehavior<T>(IVMPropertyDescriptor property) where T : IBehavior {
         var config = Configuration.PropertyConfigurations[property];
         var propertyChain = config.CreateChain();

         //var collectionFactory = propertyChain.GetNextBehavior<CollectionFactoryBehavior<ProjectVM>>();
         //var collectionChain = collectionFactory.CollectionBehaviorConfiguration.CreateChain();

         //T behavior;
         //return collectionChain.TryGetBehavior(out behavior);

         throw new NotImplementedException();
      }

      private VMPropertyBuilder<EmployeeVM, EmployeeVM> CreateRootFactory() {
         return new VMPropertyBuilder<EmployeeVM, EmployeeVM>(
            PropertyPath.Empty<EmployeeVM>(),
            Configuration
         );
      }

      private VMPropertyBuilder<EmployeeVM, Employee> CreateSourceObjectFactory() {
         return new VMPropertyBuilder<EmployeeVM, Employee>(
            PropertyPath.Create((EmployeeVM x) => x.SourcePerson),
            Configuration
         );
      }

      private class EmployeeVM : ViewModelStub {
         public Employee SourcePerson { get; private set; }
      }

      private class ProjectVM : ViewModelStub, IHasSourceObject<Project> {
         public Project SourceProject { get; private set; }

         Project IHasReadonlySourceObject<Project>.Source {
            get { return SourceProject; }
         }

         Project IHasSourceObject<Project>.Source {
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