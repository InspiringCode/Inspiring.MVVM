namespace Inspiring.MvvmTest.ViewModels.Core.Builder {
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Core.Builder.Properties;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class PropertyFactoryTests {
      private VMPropertyFactory<EmployeeVM, EmployeeVM> _factory;
      private VMDescriptorConfiguration _configuration;

      [TestInitialize]
      public void Setup() {
         var viewModelTemplate =
            BehaviorChainTemplateRegistry.GetTemplate(BehaviorChainTemplateKeys.ViewModel);

         var viewModelConfiguration = viewModelTemplate.CreateConfiguration<EmployeeVM>();

         _configuration = new VMDescriptorConfiguration(viewModelConfiguration);

         _factory = new VMPropertyFactory<EmployeeVM, EmployeeVM>(PropertyPath.Empty<EmployeeVM>(), _configuration);
      }

      [TestMethod]
      public void MappedProperty_RootFactory_InsertsProperBehaviors() {
         VMProperty<string> p = _factory.Mapped(x => x.SourcePerson.Name).Property();

         Assert.IsTrue(ContainsBehavior<IValueAccessorBehavior<string>>(p));
         Assert.IsFalse(ContainsBehavior<RefreshableValueCahche<string>>(p));
      }

      private bool ContainsBehavior<T>(VMPropertyBase property) where T : IBehavior {
         var conf = _configuration.PropertyConfigurations[property];
         BehaviorChain chain = conf.CreateChain();

         T behavior;
         return chain.TryGetBehavior<T>(out behavior);
      }

      private class EmployeeVM : ViewModelStub {
         public Employee SourcePerson { get; private set; }
      }

      private class Employee {
         public string Name { get; private set; }
      }
   }
}