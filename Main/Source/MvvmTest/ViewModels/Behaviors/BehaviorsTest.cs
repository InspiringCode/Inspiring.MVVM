using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.ViewModels.Behaviors {
   [TestClass]
   public class BehaviorsTest {
      [TestMethod]
      public void InstanceProperty() {
         EmployeeVM vm = new EmployeeVM();

         //VMProperty<int> intProperty = new VMProperty<int>(new InstancePropertyBehavior<int>());
         //VMProperty<string> stringProperty = new VMProperty<string>(new InstancePropertyBehavior<string>());
         //intProperty.Initialize(EmployeeVM.Descriptor.DynamicFields, "Test");
         //stringProperty.Initialize(EmployeeVM.Descriptor.DynamicFields, "Test");

         //Assert.AreEqual(0, intProperty.GetValue(vm));
         //intProperty.SetValue(vm, 5);
         //Assert.AreEqual(5, intProperty.GetValue(vm));

         //Assert.AreEqual(null, stringProperty.GetValue(vm));
         //stringProperty.SetValue(vm, "Test");
         //Assert.AreEqual("Test", stringProperty.GetValue(vm));

      }

      //private class PersonVM : ViewModel<PersonVMDescriptor> {

      //}

      //private class PersonVMDescriptor : VMDescriptor {

      //}
   }
}
