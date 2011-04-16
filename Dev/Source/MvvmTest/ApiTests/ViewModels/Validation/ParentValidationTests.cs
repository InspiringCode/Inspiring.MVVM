namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ParentValidationTests : HierarchyValidationFixture {
      [TestMethod]
      public void PropertyValidatorDefinedForCollectionItems_ThatMatches_IsCalledWithCorrectArguments() {
         EmployeeVM emp = CreateEmployeeVM();
         ProjectVM project = CreateProjectVM();
         emp.Projects.Add(project);

         Log.ExpectCalls(Validator.ProjectTitle, Validator.ProjectDescription);
         project.Revalidate();
         Log.VerifyCalls();

         var args = (PropertyValidationArgs<EmployeeVM, ProjectVM, string>)Log.GetArgs(Validator.ProjectTitle);
         Assert.AreSame(project, args.Target);
         Assert.AreSame(emp, args.Owner);
         Assert.AreSame(ProjectVM.ClassDescriptor.Title, args.TargetProperty);
      }

      [TestMethod]
      public void PropertyValidatorDefinedForChildVM_ThatMatches_IsCalledWithCorrectArguments() {
         ProjectVM project = CreateProjectVM();
         CustomerVM customer = CreateCustomerVM();
         project.Customer = customer;

         Log.ExpectCalls(Validator.CustomerAddress, Validator.CustomerPostalCode);
         customer.Revalidate();
         Log.VerifyCalls();

         var args = (PropertyValidationArgs<ProjectVM, CustomerVM, string>)Log.GetArgs(Validator.CustomerAddress);
         Assert.AreSame(customer, args.Target);
         Assert.AreSame(project, args.Owner);
         Assert.AreSame(CustomerVM.ClassDescriptor.Address, args.TargetProperty);
      }

      [TestMethod]
      public void PropertyValidatorDefinedForGrandchildVM_ThatMatches_IsCalledWithCorrectArguments() {
         EmployeeVM employee = CreateEmployeeVM();
         ProjectVM project = CreateProjectVM();
         CustomerVM customer = CreateCustomerVM();
         project.Customer = customer;
         employee.Projects.Add(project);
         employee.Projects.Add(CreateProjectVM());

         Log.ExpectCalls(Validator.CustomerName, Validator.CustomerAddress, Validator.CustomerPostalCode);
         customer.Revalidate();
         Log.VerifyCalls();

         var args = (PropertyValidationArgs<EmployeeVM, CustomerVM, string>)Log.GetArgs(Validator.CustomerName);
         Assert.AreSame(customer, args.Target);
         Assert.AreSame(employee, args.Owner);
         Assert.AreSame(CustomerVM.ClassDescriptor.Name, args.TargetProperty);
      }

      //[TestMethod]
      //public void PropertyValidatorDefinedForChildVM_ThatDoesNotMatches_IsNotCalled() {

      //}

      //[TestMethod]
      //public void PropertyValidatorDefinedForGrandchildVM_ThatDoesNotMatches_IsNotCalled() {

      //}
   }
}