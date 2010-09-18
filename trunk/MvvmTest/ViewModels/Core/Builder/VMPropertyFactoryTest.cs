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
         );
      }

      private static void OverrideDefaultViewModelPropertyConfiguration() {
         BehaviorConfigurationFactory.OverrideDefaultViewModelPropertyConfiguration(
            new BehaviorConfiguration()
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
            typeof(CollectionPopulatorBehavior<ProjectVM, Project>),
            typeof(MappedPropertyBehavior<PersonVM, IEnumerable<Project>>)
         );

         property = _personFactory.MappedCollection(x => x.Projects).Of<ProjectVM>(PersonVM.Descriptor);
         AssertBehaviors(
            property,
            typeof(CollectionPopulatorBehavior<ProjectVM, Project>),
            typeof(MappedPropertyBehavior<PersonVM, IEnumerable<Project>>)
         );
      }

      [TestMethod]
      public void ViewModelProperty() {
         var property = _rootFactory.MappedVM(x => x.Person.CurrentProject).Of<ProjectVM>();
         AssertBehaviors(
            property,
            typeof(ViewModelPropertyInitializerBehavior<ProjectVM, Project>),
            typeof(MappedPropertyBehavior<PersonVM, Project>)
         );

         property = _personFactory.MappedVM(x => x.CurrentProject).Of<ProjectVM>();
         AssertBehaviors(
            property,
            typeof(ViewModelPropertyInitializerBehavior<ProjectVM, Project>),
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
}
