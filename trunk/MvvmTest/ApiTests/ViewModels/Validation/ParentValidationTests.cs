namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.ViewModels.Core;

   [TestClass]
   public class ParentValidationTests : HierarchyValidationFixture {
      [TestMethod]
      public void PropertyValidatorDefinedForCollectionItems_ThatMatches_IsCalledWithCorrectArguments() {
         Log.ExpectCalls(Validator.ProjectTitle, Validator.ProjectDescription);
         
         EmployeeVM emp = CreateEmployeeVM();
         ProjectVM project = CreateProjectVM();
         emp.Projects.Add(project);

         project.Revalidate();

         Log.VerifyCalls();

         ValidationArgs args = Log.GetArgs(Validator.ProjectTitle);
         Assert.AreSame(project, args.ChangedVM);
         Assert.AreSame(project, args.TargetVM);
         Assert.AreSame(emp, args.OwnerVM);
         Assert.AreSame(ProjectVM.Descriptor.Title, args.ChangedProperty);
         Assert.AreSame(ProjectVM.Descriptor.Title, args.TargetProperty);
         Assert.AreEqual(ValidationType.PropertyValue, args.ValidationType);
      }

      [TestMethod]
      public void PropertyValidatorDefinedForChildVM_ThatMatches_IsCalledWithCorrectArguments() {
         Log.ExpectCalls(Validator.CustomerAddress, Validator.CustomerPostalCode);

         ProjectVM project = CreateProjectVM();
         CustomerVM customer = CreateCustomerVM();
         project.Customer = customer;

         customer.Revalidate();

         Log.VerifyCalls();

         ValidationArgs args = Log.GetArgs(Validator.CustomerAddress);
         Assert.AreSame(customer, args.ChangedVM);
         Assert.AreSame(customer, args.TargetVM);
         Assert.AreSame(project, args.OwnerVM);
         Assert.AreSame(CustomerVM.Descriptor.Address, args.ChangedProperty);
         Assert.AreSame(CustomerVM.Descriptor.Address, args.TargetProperty);
         Assert.AreEqual(ValidationType.PropertyValue, args.ValidationType);
      }

      [TestMethod]
      public void PropertyValidatorDefinedForGrandchildVM_ThatMatches_IsCalledWithCorrectArguments() {
         Log.ExpectCalls(Validator.CustomerName, Validator.CustomerAddress, Validator.CustomerPostalCode);

         EmployeeVM employee = CreateEmployeeVM();
         ProjectVM project = CreateProjectVM();
         CustomerVM customer = CreateCustomerVM();
         project.Customer = customer;
         employee.Projects.Add(project);
         employee.Projects.Add(CreateProjectVM());

         customer.Revalidate();

         Log.VerifyCalls();

         ValidationArgs args = Log.GetArgs(Validator.CustomerName);
         Assert.AreSame(customer, args.ChangedVM);
         Assert.AreSame(customer, args.TargetVM);
         Assert.AreSame(employee, args.OwnerVM);
         Assert.AreSame(CustomerVM.Descriptor.Name, args.ChangedProperty);
         Assert.AreSame(CustomerVM.Descriptor.Name, args.TargetProperty);
         Assert.AreEqual(ValidationType.PropertyValue, args.ValidationType);
      }

      //[TestMethod]
      //public void PropertyValidatorDefinedForChildVM_ThatDoesNotMatches_IsNotCalled() {

      //}

      //[TestMethod]
      //public void PropertyValidatorDefinedForGrandchildVM_ThatDoesNotMatches_IsNotCalled() {

      //}
   }
}