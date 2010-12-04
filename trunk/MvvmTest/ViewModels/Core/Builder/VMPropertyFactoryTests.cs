namespace Inspiring.MvvmTest.ViewModels.Core.Builder {
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Core.Builder.Properties;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

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
         var p = f.Mapped(x => x.SourcePerson.Name).Property();

         Assert.IsTrue(ContainsBehavior<MappedPropertyAccessor<EmployeeVM, string>>(p));
         AssertDefaultPropertyBehaviors(p);
      }

      [TestMethod]
      public void CalculatedProperty_RootFactory_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.Calculated(x => x.SourcePerson.Name).Property();

         Assert.IsTrue(ContainsBehavior<CalculatedPropertyAccessor<EmployeeVM, EmployeeVM, string>>(p));
         AssertDefaultPropertyBehaviors(p);
      }

      [TestMethod]
      public void LocalProperty_RootFactory_InsertsProperBehaviors() {
         var f = CreateRootFactory();
         var p = f.Local().Property<string>();

         Assert.IsTrue(ContainsBehavior<InstancePropertyBehavior<string>>(p));
         AssertDefaultPropertyBehaviors(p);
      }

      private bool ContainsBehavior<T>(VMPropertyBase property) where T : IBehavior {
         var conf = Configuration.PropertyConfigurations[property];
         BehaviorChain chain = conf.CreateChain();

         T behavior;
         return chain.TryGetBehavior<T>(out behavior);
      }

      private void AssertDefaultPropertyBehaviors(VMPropertyBase p) {
         Assert.IsTrue(ContainsBehavior<PropertyDescriptorBehavior>(p));

         Assert.IsFalse(ContainsBehavior<PropertyValidationBehavior<string>>(p));
         Assert.IsFalse(ContainsBehavior<AllowInvalidDisplayValuesBehavior>(p));
         Assert.IsFalse(ContainsBehavior<RefreshableValueCacheBehavior<string>>(p));
      }

      private VMPropertyFactory<EmployeeVM, EmployeeVM> CreateRootFactory() {
         return new VMPropertyFactory<EmployeeVM, EmployeeVM>(
            PropertyPath.Empty<EmployeeVM>(),
            Configuration
         );
      }

      private VMPropertyFactory<EmployeeVM, Employee> CreateSourceObjectFactory() {
         return new VMPropertyFactory<EmployeeVM, Employee>(
            PropertyPath.Create((EmployeeVM x) => x.SourcePerson),
            Configuration
         );
      }

      private class EmployeeVM : ViewModelStub {
         public Employee SourcePerson { get; private set; }
      }

      private class Employee {
         public string Name { get; private set; }
      }
   }
}