﻿namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class HierarchyValidationFixture {
      public ValidationLog Log { get; set; }

      [TestInitialize]
      public void Setup() {
         Log = new ValidationLog();
      }

      protected EmployeeVM CreateEmployeeVM() {
         return new EmployeeVM { Log = Log };
      }

      //protected EmployeeVM CreateEmployeeVMWithProjects() {
      //   var vm = CreateEmployeeVM();
      //   vm.Projects.Add(CreateProjectVM());
      //   vm.Projects.Add(CreateProjectVM());
      //   return vm;
      //}

      protected ProjectVM CreateProjectVM() {
         return new ProjectVM { Log = Log };
      }

      protected CustomerVM CreateCustomerVM() {
         return new CustomerVM { Log = Log };
      }

      public enum Validator {
         EmployeeName,
         ProjectTitle,
         ProjectDescription,
         CustomerName,
         CustomerAddress,
         CustomerPostalCode
      }

      public class ValidationLog {
         private List<Validator> _expectedCalls = new List<Validator>();
         private Dictionary<Validator, ValidationArgs> _actualCalls = new Dictionary<Validator, ValidationArgs>();

         public void ExpectCalls(params Validator[] toValidators) {
            _expectedCalls.AddRange(toValidators);
         }

         public void AddCall(Validator validator, ValidationArgs args) {
            Assert.IsTrue(
               _expectedCalls.Contains(validator),
               "Did not expect a call to validator {0}.",
               validator
            );

            _actualCalls.Add(validator, args);
         }

         public void VerifyCalls() {
            CollectionAssert.AreEquivalent(
               _expectedCalls,
               _actualCalls.Keys,
               "Not all expected validators were called."
            );
         }

         public ValidationArgs GetArgs(Validator forValidator) {
            return _actualCalls[forValidator];
         }
      }

      public sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static readonly EmployeeVMDescriptor Descriptor = VMDescriptorBuilder
            .For<EmployeeVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyFactory();

               return new EmployeeVMDescriptor {
                  Name = v.Local.Property<string>(),
                  Projects = v.Collection().Of<ProjectVM>(ProjectVM.Descriptor)
               };
            })
            .WithValidations((d, c) => {
               c.Check(x => x.Name).Custom((vm, val, args) => {
                  vm.Log.AddCall(Validator.EmployeeName, args);
               });
               c.CheckVMs(x => x.Projects).Check(x => x.Title).Custom((vm, val, args) => {
                  ((ProjectVM)vm).Log.AddCall(Validator.ProjectTitle, args);
               });
               c.CheckVMs(x => x.Projects).CheckVM(x => x.Customer).Check(x => x.Name).Custom((vm, val, args) => {
                  ((CustomerVM)vm).Log.AddCall(Validator.CustomerName, args);
               });
            })
            .Build();

         public EmployeeVM()
            : base(Descriptor) {
         }

         public ValidationLog Log { get; set; }

         public IVMCollection<ProjectVM> Projects {
            get { return GetValue(Descriptor.Projects); }
         }

         public void Revalidate(ValidationScope scope = ValidationScope.SelfOnly) {
            Revalidate(scope, ValidationMode.DiscardInvalidValues);
         }
      }

      public sealed class ProjectVM : ViewModel<ProjectVMDescriptor> {
         public static readonly ProjectVMDescriptor Descriptor = VMDescriptorBuilder
            .For<ProjectVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyFactory();

               return new ProjectVMDescriptor {
                  Title = v.Local.Property<string>(),
                  Description = v.Local.Property<string>(),
                  Customer = v.Local.VM<CustomerVM>()
               };
            })
            .WithValidations((d, c) => {
               c.EnableParentValidation(x => x.Title);

               c.Check(x => x.Description).Custom((vm, val, args) => {
                  vm.Log.AddCall(Validator.ProjectDescription, args);
               });
               c.CheckVM(x => x.Customer).Check(x => x.Address).Custom((vm, val, args) => {
                  ((CustomerVM)vm).Log.AddCall(Validator.CustomerAddress, args);
               });
            })
            .Build();

         public ProjectVM()
            : base(Descriptor) {
         }

         public ValidationLog Log { get; set; }

         public CustomerVM Customer {
            get { return GetValue(Descriptor.Customer); }
            set { SetValue(Descriptor.Customer, value); }
         }

         public void Revalidate(ValidationScope scope = ValidationScope.SelfOnly) {
            Revalidate(scope, ValidationMode.DiscardInvalidValues);
         }
      }

      public sealed class CustomerVM : ViewModel<CustomerVMDescriptor> {
         public static readonly CustomerVMDescriptor Descriptor = VMDescriptorBuilder
            .For<CustomerVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyFactory();

               return new CustomerVMDescriptor {
                  Name = v.Local.Property<string>(),
                  Address = v.Local.Property<string>(),
                  PostalCode = v.Local.Property<int>()
               };
            })
            .WithValidations((d, c) => {
               c.EnableParentValidation(x => x.Name);
               c.EnableParentValidation(x => x.Address);
               c.Check(x => x.PostalCode).Custom((vm, val, args) => {
                  vm.Log.AddCall(Validator.CustomerPostalCode, args);
               });
            })
            .Build();

         public CustomerVM()
            : base(Descriptor) {
         }

         public ValidationLog Log { get; set; }

         public void Revalidate(ValidationScope scope = ValidationScope.SelfOnly) {
            Revalidate(scope, ValidationMode.DiscardInvalidValues);
         }
      }

      public sealed class EmployeeVMDescriptor : VMDescriptor {
         public VMProperty<string> Name { get; set; }
         public VMProperty<IVMCollection<ProjectVM>> Projects { get; set; }
      }

      public sealed class ProjectVMDescriptor : VMDescriptor {
         public VMProperty<string> Title { get; set; }
         public VMProperty<string> Description { get; set; }
         public VMProperty<CustomerVM> Customer { get; set; }
      }

      public sealed class CustomerVMDescriptor : VMDescriptor {
         public VMProperty<string> Name { get; set; }
         public VMProperty<string> Address { get; set; }
         public VMProperty<int> PostalCode { get; set; }
      }
   }
}