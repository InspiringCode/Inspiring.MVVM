namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class VMPropertyFactoryTest {
      private BehaviorConfigurationDictionary _configs;
      private VMPropertyFactory<PersonVM, PersonVM> _rootFactory;
      private VMPropertyFactory<PersonVM, Person> _personFactory;

      private static void OverrideDefaultPropertyConfiguration() {
         BehaviorConfigurationFactory.OverrideDefaultConfiguration(
            new BehaviorConfiguration()
               .Append(VMBehaviorKey.PropertyValueAcessor)
               .Append(VMBehaviorKey.SourceValueAccessor, disabled: true)
         );
      }

      private static void OverrideDefaultCollectionConfiguration() {
         BehaviorConfigurationFactory.OverrideDefaultCollectionConfiguration(
            new BehaviorConfiguration()
               .Append(VMBehaviorKey.CollectionPopulator)
               .Append(VMBehaviorKey.SourceValueAccessor)
               .Append(VMBehaviorKey.ViewModelFactory)
               .Append(VMBehaviorKey.CollectionFactory)
         );
      }

      private static void OverrideDefaultViewModelPropertyConfiguration() {
         BehaviorConfigurationFactory.OverrideDefaultViewModelPropertyConfiguration(
            new BehaviorConfiguration()
               .Append(VMBehaviorKey.ViewModelPropertyInitializer)
               .Append(VMBehaviorKey.ViewModelFactory)
               .Append(VMBehaviorKey.SourceValueAccessor)
         );
      }

      [TestInitialize]
      public void Setup() {
         OverrideDefaultPropertyConfiguration();
         OverrideDefaultCollectionConfiguration();
         OverrideDefaultViewModelPropertyConfiguration();

         _configs = new BehaviorConfigurationDictionary();
         _rootFactory = new VMPropertyFactory<PersonVM, PersonVM>(PropertyPath.Empty<PersonVM>(), _configs);
         _personFactory = new VMPropertyFactory<PersonVM, Person>(PropertyPath.Create((PersonVM vm) => vm.Person), _configs);
      }

      [TestCleanup]
      public void Teardown() {
         BehaviorConfigurationFactory.Reset();
      }

      [TestMethod]
      public void MappedProperty() {
         var property = _rootFactory.Mapped(x => x.Person.FirstName);
         AssertBehaviors(property, typeof(MappedPropertyBehavior<PersonVM, string>));

         property = _personFactory.Mapped(x => x.FirstName);
         AssertBehaviors(property, typeof(MappedPropertyBehavior<PersonVM, string>));
      }

      [TestMethod]
      public void CalculatedProperty() {
         var property = _rootFactory.Calculated(x => x.Person.FirstName);
         AssertBehaviors(
            property,
            typeof(CalculatedPropertyBehavior<PersonVM, string>)
         );

         property = _personFactory.Calculated(x => x.FirstName);
         AssertBehaviors(
            property,
            typeof(CalculatedPropertyBehavior<Person, string>),
            typeof(MappedPropertyBehavior<PersonVM, Person>)
         );
      }

      [TestMethod]
      public void CollectionProperty() {
         var property = _rootFactory.MappedCollection(x => x.Projects).Of<ProjectVM>(PersonVM.Descriptor);
         AssertBehaviors(
            property,
            typeof(CollectionPopulatorBehavior<PersonVM, ProjectVM, Project>),
            typeof(MappedPropertyBehavior<PersonVM, IEnumerable<Project>>),
            typeof(ViewModelFactoryBehavior<ProjectVM>),
            typeof(CollectionFactoryBehavior<ProjectVM>)
         );

         property = _personFactory.MappedCollection(x => x.Projects).Of<ProjectVM>(PersonVM.Descriptor);
         AssertBehaviors(
            property,
            typeof(CollectionPopulatorBehavior<PersonVM, ProjectVM, Project>),
            typeof(MappedPropertyBehavior<PersonVM, IEnumerable<Project>>),
            typeof(ViewModelFactoryBehavior<ProjectVM>),
            typeof(CollectionFactoryBehavior<ProjectVM>)
         );
      }

      [TestMethod]
      public void ViewModelProperty() {
         var property = _rootFactory.MappedVM(x => x.Person.CurrentProject).Of<ProjectVM>();
         AssertBehaviors(
            property,
            typeof(ViewModelPropertyInitializerBehavior<ProjectVM, Project>),
            typeof(ViewModelFactoryBehavior<ProjectVM>),
            typeof(MappedPropertyBehavior<PersonVM, Project>)
         );

         property = _personFactory.MappedVM(x => x.CurrentProject).Of<ProjectVM>();
         AssertBehaviors(
            property,
            typeof(ViewModelPropertyInitializerBehavior<ProjectVM, Project>),
            typeof(ViewModelFactoryBehavior<ProjectVM>),
            typeof(MappedPropertyBehavior<PersonVM, Project>)
         );
      }

      [TestMethod]
      public void LocalProperty() {
         var property = _rootFactory.Local<int>();
         AssertBehaviors(property, typeof(InstancePropertyBehavior<int>));
      }



      private void AssertBehaviors(VMProperty property, params Type[] expectedBehaviorTypes) {
         IBehavior head = _configs
            .GetConfiguration(property)
            .CreateBehaviorChain<string>();

         List<Type> actualTypes = new List<Type>();
         for (IBehavior b = head.Successor; b != null; b = b.Successor) {
            actualTypes.Add(b.GetType());
            Assert.IsTrue(actualTypes.Count < 100, "Inifinte behavior loop detected");
         }
         CollectionAssert.AreEqual(expectedBehaviorTypes, actualTypes);
      }
   }

   [TestClass]
   public class ConfiguredVMPropertyFactoryTest {
      private Behavior _additionalBehavior;
      private VMPropertyFactory<TestVM, TestVM> _configuredFactory;
      private BehaviorConfigurationDictionary _configs;

      [TestInitialize]
      public void Setup() {
         BehaviorConfiguration additionalConfig = new BehaviorConfiguration();
         additionalConfig.Insert(VMBehaviorKey.CustomOne, RelativePosition.Before, VMBehaviorKey.First);

         _additionalBehavior = new Behavior();
         additionalConfig.OverrideFactory(
            VMBehaviorKey.CustomOne,
            new ConstantBehaviorFactory(_additionalBehavior)
         ).Enable(VMBehaviorKey.CustomOne);

         _configs = new BehaviorConfigurationDictionary();

         VMPropertyFactory<TestVM, TestVM> factory = new VMPropertyFactory<TestVM, TestVM>(
            PropertyPath.Empty<TestVM>(),
            _configs
         );

         _configuredFactory = factory.WithConfiguration(additionalConfig);
      }

      [TestMethod]
      public void CreateMappedPropertyWithAdditionalBehavior() {
         CheckForAdditionalBehavior(_configuredFactory.Mapped(x => x.Source.MappedMutableValue));
      }

      [TestMethod]
      public void CreateCalculatedPropertyWithAdditionalBehavior() {
         CheckForAdditionalBehavior(_configuredFactory.Calculated(x => x.Source.MappedMutableValue));
      }

      [TestMethod]
      public void CreateLocalPropertyWithAdditionalBehavior() {
         CheckForAdditionalBehavior(_configuredFactory.Local<string>());
      }

      [TestMethod]
      public void CreateCommandPropertyWithAdditionalBehavior() {
         CheckForAdditionalBehavior(_configuredFactory.Command(x => { }));
      }

      [TestMethod]
      public void CreateViewModelPropertyWithAdditionalBehavior() {
         CheckForAdditionalBehavior(_configuredFactory
            .MappedVM(x => x.Source.ChildValue)
            .Of<ChildVM>()
         );
      }

      [TestMethod]
      public void CreateCollectionPropertyWithAdditionalBehavior() {
         CheckForAdditionalBehavior(_configuredFactory
            .MappedCollection(x => x.Source.ChildCollection)
            .Of<ChildVM>(ChildVM.Descriptor)
         );
      }

      private void CheckForAdditionalBehavior(VMProperty property) {
         var chain = _configs.GetConfiguration(property).CreateBehaviorChain<string>();
         Assert.AreSame(_additionalBehavior, chain.Successor);
      }
   }
}
