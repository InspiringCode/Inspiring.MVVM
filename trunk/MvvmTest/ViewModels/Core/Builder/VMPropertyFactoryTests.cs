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
   public class VMPropertyFactoryTests {
      public VMDescriptorConfiguration Configuration { get; private set; }

      [TestInitialize]
      public void Setup() {
         var template = BehaviorChainTemplateRegistry.GetTemplate(
            BehaviorChainTemplateKeys.ViewModel
         );

         var viewModelConfiguration = template.CreateConfiguration(ViewModelBehaviorFactory.CreateInvoker<EmployeeVM>());

         Configuration = new VMDescriptorConfiguration(viewModelConfiguration);
      }

      [TestMethod]
      public void MappedProperty_RootFactory_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.Property.MapsTo(x => x.SourcePerson.Name);

         Assert.IsTrue(ContainsBehavior<MappedPropertyAccessor<EmployeeVM, string>>(p));
         AssertDefaultPropertyBehaviors(p);
      }

      [TestMethod]
      public void CalculatedProperty_RootFactory_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.Property.DelegatesTo(x => x.SourcePerson.Name);

         Assert.IsTrue(ContainsBehavior<CalculatedPropertyAccessor<EmployeeVM, EmployeeVM, string>>(p));
         AssertDefaultPropertyBehaviors(p);
      }

      [TestMethod]
      public void LocalProperty_RootFactory_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.Property.Of<string>();

         Assert.IsTrue(ContainsBehavior<InstancePropertyBehavior<string>>(p));
         AssertDefaultPropertyBehaviors(p);
      }

      [TestMethod]
      public void ViewModelProperty_WrapsExpression_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.VM.Wraps(x => x.SourcePerson.CurrentProject).With<ProjectVM>();

         AssertDefaultViewModelBehaviors(p);
         Assert.IsTrue(ContainsBehavior<MappedPropertyAccessor<EmployeeVM, Project>>(p));
         Assert.IsTrue(ContainsBehavior<ViewModelFactoryBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<ViewModelWithSourceAcessorBehavior<ProjectVM, Project>>(p));
         Assert.IsTrue(ContainsBehavior<ValueCacheBehavior<ProjectVM>>(p));
      }

      [TestMethod]
      public void ViewModelProperty_WrapsDelegates_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.VM.Wraps(x => x.SourcePerson.CurrentProject, setter: null).With<ProjectVM>();

         AssertDefaultViewModelBehaviors(p);
         Assert.IsTrue(ContainsBehavior<CalculatedPropertyAccessor<EmployeeVM, EmployeeVM, Project>>(p));
         Assert.IsTrue(ContainsBehavior<ViewModelFactoryBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<ViewModelWithSourceAcessorBehavior<ProjectVM, Project>>(p));
         Assert.IsTrue(ContainsBehavior<ValueCacheBehavior<ProjectVM>>(p));
      }

      [TestMethod]
      public void ViewModelProperty_InstanceProperty_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.VM.Of<ProjectVM>();

         AssertDefaultViewModelBehaviors(p);
         Assert.IsTrue(ContainsBehavior<InstancePropertyBehavior<ProjectVM>>(p));
         Assert.IsFalse(ContainsBehavior<ViewModelFactoryBehavior<ProjectVM>>(p));
         Assert.IsFalse(ContainsBehavior<ViewModelWithSourceAcessorBehavior<ProjectVM, Project>>(p));
         Assert.IsFalse(ContainsBehavior<ValueCacheBehavior<ProjectVM>>(p));
      }

      [TestMethod]
      public void CollectionProperty_Wraps_InsertsPropertyBehaviors() {
         var f = CreateRootFactory();
         var p = f.Collection.Wraps(x => x.SourcePerson.Projects).With<ProjectVM>(new VMDescriptorStub());

         AssertDefaultCollectionPropertyBehaviors(p);
         Assert.IsTrue(ContainsBehavior<CollectionPopulatorBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<ValueCacheBehavior<IVMCollection<ProjectVM>>>(p));

         Assert.IsTrue(ContainsCollectionBehavior<PopulatorCollectionBehavior<ProjectVM, Project>>(p));
         Assert.IsTrue(ContainsCollectionBehavior<ViewModelFactoryBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsCollectionBehavior<CalculatedPropertyAccessor<EmployeeVM, EmployeeVM, IEnumerable<Project>>>(p));
      }

      [TestMethod]
      public void CollectionProperty_Initialized_InsertsPropertyBehaviors() {
         var f = CreateRootFactory();
         var p = f.Collection.PopulatedWith(x => Enumerable.Empty<ProjectVM>()).With(new VMDescriptorStub());

         AssertDefaultCollectionPropertyBehaviors(p);
         Assert.IsTrue(ContainsBehavior<CollectionPopulatorBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<ValueCacheBehavior<IVMCollection<ProjectVM>>>(p));

         Assert.IsTrue(ContainsCollectionBehavior<DelegatePopulatorCollectionBehavior<ProjectVM, EmployeeVM>>(p));
         Assert.IsTrue(ContainsCollectionBehavior<MappedPropertyAccessor<EmployeeVM, EmployeeVM>>(p));
         Assert.IsFalse(ContainsCollectionBehavior<ViewModelFactoryBehavior<ProjectVM>>(p));
      }

      [TestMethod]
      public void CollectionProperty_Instance_InsertsPropertyBehaviors() {
         var f = CreateRootFactory();
         var p = f.Collection.Of<ProjectVM>(new VMDescriptorStub());

         AssertDefaultCollectionPropertyBehaviors(p);
         Assert.IsFalse(ContainsBehavior<CollectionPopulatorBehavior<ProjectVM>>(p));

         Assert.IsFalse(ContainsCollectionBehavior<PopulatorCollectionBehavior<ProjectVM, Project>>(p));
         Assert.IsFalse(ContainsCollectionBehavior<MappedPropertyAccessor<EmployeeVM, EmployeeVM>>(p));
         Assert.IsFalse(ContainsCollectionBehavior<ViewModelFactoryBehavior<ProjectVM>>(p));
      }

      [TestMethod]
      public void ViewModelProperty_DelegatesTo_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.VM.DelegatesTo(x => new ProjectVM());

         AssertDefaultViewModelBehaviors(p);
         Assert.IsTrue(ContainsBehavior<CalculatedPropertyAccessor<EmployeeVM, EmployeeVM, ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<ValueCacheBehavior<ProjectVM>>(p));
         Assert.IsFalse(ContainsBehavior<ViewModelWithSourceAcessorBehavior<ProjectVM, Project>>(p));
      }

      private void AssertDefaultPropertyBehaviors(IVMProperty p) {
         Assert.IsTrue(ContainsBehavior<PropertyDescriptorBehavior>(p));

         Assert.IsFalse(ContainsBehavior<PropertyValidationBehavior<string>>(p));
         Assert.IsFalse(ContainsBehavior<AllowInvalidDisplayValuesBehavior>(p));
         Assert.IsFalse(ContainsBehavior<RefreshableValueCacheBehavior<string>>(p));
      }

      private void AssertDefaultViewModelBehaviors(IVMProperty p) {
         Assert.IsTrue(ContainsBehavior<DisplayValueAccessorBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<ParentSetterBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<PropertyDescriptorBehavior>(p));
      }

      private void AssertDefaultCollectionPropertyBehaviors(IVMProperty p) {
         Assert.IsTrue(ContainsBehavior<IDisplayValueAccessorBehavior>(p));
         Assert.IsTrue(ContainsBehavior<ValueCacheBehavior<IVMCollection<ProjectVM>>>(p));
         Assert.IsTrue(ContainsBehavior<CollectionFactoryBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsBehavior<PropertyDescriptorBehavior>(p));

         Assert.IsTrue(ContainsCollectionBehavior<DescriptorSetterCollectionBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsCollectionBehavior<ParentSetterCollectionBehavior<ProjectVM>>(p));
         Assert.IsTrue(ContainsCollectionBehavior<ChangeNotifierCollectionBehavior<ProjectVM>>(p));

      }

      private bool ContainsBehavior<T>(IVMProperty property) where T : IBehavior {
         var config = Configuration.PropertyConfigurations[property];
         BehaviorChain chain = config.CreateChain();

         T behavior;
         return chain.TryGetBehavior<T>(out behavior);
      }

      private bool ContainsCollectionBehavior<T>(IVMProperty property) where T : IBehavior {
         var config = Configuration.PropertyConfigurations[property];
         var propertyChain = config.CreateChain();

         var collectionFactory = propertyChain.GetNextBehavior<CollectionFactoryBehavior<ProjectVM>>();
         var collectionChain = collectionFactory.CollectionBehaviorConfiguration.CreateChain();

         T behavior;
         return collectionChain.TryGetBehavior(out behavior);
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

      private class ProjectVM : ViewModelStub, IVMCollectionItem<Project> {
         public Project SourceProject { get; private set; }

         Project IVMCollectionItem<Project>.Source {
            get { return SourceProject; }
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