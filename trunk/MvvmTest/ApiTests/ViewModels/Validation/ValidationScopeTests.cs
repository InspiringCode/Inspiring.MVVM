namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.ViewModels;

   [TestClass]
   public class ValidationScopeTests : HierarchyValidationFixture {
      [TestMethod]
      public void Revalidate_SelfOnly_ValidatesOnlyOwnProperties() {
         EmployeeVM vm = CreateViewModelHierarchy();

         Log.ExpectCalls(Validator.EmployeeName);
         vm.Revalidate(ValidationScope.SelfOnly);
         Log.VerifyCalls();
      }

      [TestMethod]
      public void Revalidate_FullSubtree_ValidatesChildVM() {
         ProjectVM project = CreateProjectVM();
         project.Customer = CreateCustomerVM();

         Log.ExpectCalls(Validator.ProjectDescription, Validator.CustomerAddress, Validator.CustomerPostalCode);
         project.Revalidate(ValidationScope.FullSubtree);
         Log.VerifyCalls();
      }

      [TestMethod]
      public void Revalidate_FullSubtree_ValidatesChildCollection() {
         EmployeeVM employee = CreateEmployeeVM();
         employee.Projects.Add(CreateProjectVM());

         Log.ExpectCalls(Validator.EmployeeName, Validator.ProjectTitle);
         employee.Revalidate(ValidationScope.FullSubtree);
         Log.VerifyCalls();
      }

      [TestMethod]
      public void Revalidate_FullSubtree_ValidatesGrandchildren() {
         EmployeeVM employee = CreateEmployeeVM();
         ProjectVM project = CreateProjectVM();
         project.Customer = CreateCustomerVM();
         employee.Projects.Add(project);

         Log.ExpectCalls(
            Validator.EmployeeName, 
            Validator.ProjectDescription,
            Validator.ProjectTitle,
            Validator.CustomerAddress, 
            Validator.CustomerName,
            Validator.CustomerPostalCode
         );
         employee.Revalidate(ValidationScope.FullSubtree);
         Log.VerifyCalls();
      }

      [TestMethod]
      public void Revalidate_ValidatedChildren_ValidatesChildVM() {
         Assert.Inconclusive("TODO");
      }

      [TestMethod]
      public void Revalidate_ValidatedChildren_ValidatesChildCollection() {
         Assert.Inconclusive("TODO");
      }

      [TestMethod]
      public void Revalidate_ValidatedChildren_ValidatesGrandchildren() {
         Assert.Inconclusive("TODO");
      }

      private EmployeeVM CreateViewModelHierarchy() {
         var emp = CreateEmployeeVM();
         var project1 = CreateProjectVM();
         var project2 = CreateProjectVM();
         project1.Customer = CreateCustomerVM();
         project2.Customer = CreateCustomerVM();
         emp.Projects.Add(project1);
         emp.Projects.Add(project2);
         return emp;
      }
   }
}