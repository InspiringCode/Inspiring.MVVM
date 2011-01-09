namespace Inspiring.MvvmTest.ViewModels.Core {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class PropertySelectorTests {
      [TestMethod]
      public void GetValue_NonGenericProperty_ReturnsValue() {
         const int expectedSalary = 5000;

         var descriptor = new EmployeeVMDescriptor();
         var employee = new ViewModelStub(descriptor);
         employee.SetValue(descriptor.Salary, expectedSalary);


         var selector = PropertySelector.Create((EmployeeVMDescriptor x) => x.Salary);

         Assert.AreEqual(expectedSalary, selector.GetPropertyValue(employee));
      }

      private class EmployeeVMDescriptor : VMDescriptor {
         public EmployeeVMDescriptor() {
            Salary = new VMProperty<int>();
            Salary.Behaviors.Successor = new InstancePropertyBehavior<int>();
            Salary.Behaviors.Initialize(this, Salary);
         }

         public IVMProperty<int> Salary { get; set; }
      }
   }
}