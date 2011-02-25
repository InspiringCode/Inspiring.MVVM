namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using System.Collections;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class InstancePathTests {
      [TestMethod]
      public void Steps_OfDefaultInstance_ReturnsEmptyCollection() {
         var path = new InstancePath();
         Assert.IsTrue(path.IsEmpty);
      }

      [TestMethod]
      public void Steps_ViewModelConstructor_ReturnsSingleStep() {
         var vm = new Mock<IViewModel>().Object;

         var path = new InstancePath(vm);

         Assert.IsFalse(path.IsEmpty);

         AssertHelper.AreEquivalent(CreateSteps(vm), path.Steps, StepsAreEqual);
      }

      [TestMethod]
      public void PrependVM_OnAnyInstance_DoesNotModifyInstance() {
         var vm = new Mock<IViewModel>().Object;

         var path = new InstancePath();
         path.PrependVM(vm);

         Assert.IsTrue(path.IsEmpty);
      }

      [TestMethod]
      public void PrependVM_CalledTwoTimes_AddsTwoSteps() {
         var vm1 = new Mock<IViewModel>().Object;
         var vm2 = new Mock<IViewModel>().Object;

         var path = new InstancePath();
         path = path.PrependVM(vm1);
         path = path.PrependVM(vm2);

         AssertHelper.AreEqual(CreateSteps(vm2, vm1), path.Steps, StepsAreEqual);
      }

      [TestMethod]
      public void PrependCollection_OnDefaultInstance_ThrowsInvalidOperationException() {
         var coll = new Mock<IEnumerable>().Object;

         var path = new InstancePath();

         //AssertHelper.Throws<InvalidOperationException>(() =>
         //   path.PrependCollection(coll)
         //);
      }

      //[TestMethod]
      //public void PrependCollection_WithTwoStepPath_ModifiesFirstStep() {
      //   var vm1 = new Mock<IViewModel>().Object;
      //   var vm2 = new Mock<IViewModel>().Object;
      //   var coll = new Mock<IEnumerable>().Object;

      //   var path = new InstancePath();
      //   path = path.PrependVM(vm1);
      //   path = path.PrependVM(vm2);
      //   //path.PrependCollection(coll);

      //   var step = path.Steps.First();
      //   Assert.AreSame(coll, step.ParentCollection);
      //}

      [TestMethod]
      public void Subpath_ZeroCountOnEmptyPath_ReturnsEmptyPath() {
         var path = new InstancePath();
         var result = path.Subpath(0, 0);
         Assert.IsTrue(result.IsEmpty);
      }

      [TestMethod]
      public void Subpath_StartZeroCountOne_ReturnsOneStepPath() {
         var vm1 = new Mock<IViewModel>().Object;
         var vm2 = new Mock<IViewModel>().Object;

         var path = new InstancePath(vm1, vm2);

         var result = path.Subpath(0, 1);
         AssertHelper.AreEqual(CreateSteps(vm1), result.Steps, StepsAreEqual);
      }

      [TestMethod]
      public void Subpath_StartOneCountMaxInt_ReturnsPathWithRemainingSteps() {
         var vm1 = new Mock<IViewModel>().Object;
         var vm2 = new Mock<IViewModel>().Object;

         var path = new InstancePath(vm1, vm2);

         var result = path.Subpath(1, Int32.MaxValue);
         AssertHelper.AreEqual(CreateSteps(vm2), result.Steps, StepsAreEqual);
      }

      [TestMethod]
      public void MatchStart_WithOnePropertyOnDefaultInstance_DoesNotMatch() {
         var employeeDescriptor = new EmployeeVMDescriptor();
         var properties = new VMPropertyPath()
            .AddProperty(PropertySelector.CreateExactlyTyped((EmployeeVMDescriptor d) => d.CurrentProject));

         var path = new InstancePath();
         InstancePathMatch match = path.MatchStart(properties);

         Assert.IsFalse(match.Success);
      }

      [TestMethod]
      public void MatchStart_WithOnePropertyOnOneStepPath_DoesNotMatch() {
         var employeeDescriptor = new EmployeeVMDescriptor();
         var properties = new VMPropertyPath()
            .AddProperty(PropertySelector.CreateExactlyTyped((EmployeeVMDescriptor d) => d.CurrentProject));

         var employeeVM = new ViewModelStub(employeeDescriptor);
         var path = new InstancePath(employeeVM);
         InstancePathMatch match = path.MatchStart(properties);

         Assert.IsFalse(match.Success);
      }

      [TestMethod]
      public void MatchStart_EmptyPropertyPath_DoesMatch() {
         var vm = new ViewModelStub(new EmployeeVMDescriptor());
         var path = new InstancePath(vm);
         var result = path.MatchStart(VMPropertyPath.Empty);

         Assert.IsTrue(result.Success);
         AssertHelper.AreEqual(CreateSteps(vm), result.MatchedPath.Steps, StepsAreEqual);
         Assert.IsTrue(result.RemainingPath.IsEmpty);
      }

      [TestMethod]
      public void MatchStart_WithTwoMatchingPropertiesOnFourStepPath_DoesMatch() {
         var employeeDescriptor = new EmployeeVMDescriptor();
         var projectDescriptor = new ProjectVMDescriptor();

         var properties = new VMPropertyPath()
            .AddProperty(PropertySelector.CreateExactlyTyped((EmployeeVMDescriptor d) => d.CurrentProject))
            .AddProperty(PropertySelector.CreateExactlyTyped((ProjectVMDescriptor d) => d.Customer));

         var employeeVM = new ViewModelStub(employeeDescriptor);
         var projectVM = new ViewModelStub(projectDescriptor);
         var customerVM = new ViewModelStub(new CustomerVMDescriptor());
         var addressVM = new ViewModelStub(new AddressVMDescriptor());

         employeeVM.SetValue(employeeDescriptor.CurrentProject, projectVM);
         projectVM.SetValue(projectDescriptor.Customer, customerVM);

         var path = new InstancePath(addressVM);
         path = path.PrependVM(customerVM);
         path = path.PrependVM(projectVM);
         path = path.PrependVM(employeeVM);

         InstancePathMatch match = path.MatchStart(properties);

         Assert.IsTrue(match.Success);

         AssertHelper.AreEqual(
            CreateSteps(employeeVM, projectVM, customerVM),
            match.MatchedPath.Steps,
            StepsAreEqual
         );

         AssertHelper.AreEqual(
            CreateSteps(addressVM),
            match.RemainingPath.Steps,
            StepsAreEqual
         );
      }

      [TestMethod]
      public void MatchStart_WithTwoNonMatchingPropertiesOnThreeStepPath_DoesNotMatch() {
         var employeeDescriptor = new EmployeeVMDescriptor();
         var projectDescriptor = new ProjectVMDescriptor();

         var properties = new VMPropertyPath()
            .AddProperty(PropertySelector.CreateExactlyTyped((EmployeeVMDescriptor d) => d.CurrentProject))
            .AddProperty(PropertySelector.CreateExactlyTyped((ProjectVMDescriptor d) => d.Customer));

         var employeeVM = new ViewModelStub(employeeDescriptor);
         var projectVM = new ViewModelStub(projectDescriptor);
         var customerVM = new ViewModelStub(new CustomerVMDescriptor());

         var someOtherVM = new ViewModelStub();

         employeeVM.SetValue(employeeDescriptor.CurrentProject, projectVM);
         projectVM.SetValue(projectDescriptor.Customer, someOtherVM);

         var path = new InstancePath(customerVM);
         path = path.PrependVM(projectVM);
         path = path.PrependVM(employeeVM);

         InstancePathMatch match = path.MatchStart(properties);

         Assert.IsFalse(match.Success);
      }

      [TestMethod]
      public void MatchStart_WithCollectionProperty_DoesMatch() {
         var employeeDescriptor = new EmployeeVMDescriptor();
         var projectDescriptor = new ProjectVMDescriptor();

         var properties = new VMPropertyPath()
            .AddProperty(PropertySelector.CreateExactlyTyped((EmployeeVMDescriptor d) => d.Projects))
            .AddProperty(PropertySelector.CreateExactlyTyped((ProjectVMDescriptor d) => d.Customer));

         var employeeVM = new ViewModelStub(employeeDescriptor);
         var projectVM = new ViewModelStub(projectDescriptor);
         var customerVM = new ViewModelStub(new CustomerVMDescriptor());

         var projectsCollection = new VMCollection<IViewModel>(new BehaviorChain(), employeeVM) { projectVM };

         employeeVM.SetValue(employeeDescriptor.Projects, projectsCollection);
         projectVM.SetValue(projectDescriptor.Customer, customerVM);

         projectVM.Kernel.OwnerCollection = projectsCollection;

         var path = new InstancePath(customerVM);

         path = path.PrependVM(projectVM);
         //path.PrependCollection(projectsCollection);

         path = path.PrependVM(employeeVM);

         InstancePathMatch match = path.MatchStart(properties);

         Assert.IsTrue(match.Success);

         AssertHelper.AreEqual(
            new InstancePathStep[] { 
               new InstancePathStep(employeeVM), 
               new InstancePathStep(projectVM),// { ParentCollection = projectsCollection },
               new InstancePathStep(customerVM)
            },
            match.MatchedPath.Steps,
            StepsAreEqual
         );

         Assert.IsTrue(match.RemainingPath.IsEmpty);
      }

      private static InstancePathStep[] CreateSteps(params IViewModel[] steps) {
         return steps.Select(x => new InstancePathStep(x)).ToArray();
      }

      private static bool StepsAreEqual(InstancePathStep x, InstancePathStep y) {
         if (Object.ReferenceEquals(x, y)) {
            return true;
         }

         return
            x != null &&
            y != null &&
            Object.ReferenceEquals(x.VM, y.VM);// &&
         //Object.ReferenceEquals(x.ParentCollection, y.ParentCollection);
      }

      private class EmployeeVMDescriptor : VMDescriptor {
         //   public static EmployeeVMDescriptor Create() {
         //      return VMDescriptorBuilder
         //         .OfType<EmployeeVMDescriptor>()
         //         .For<IViewModel>()
         //         .WithProperties((d, b) => {
         //            var v = b.GetPropertyBuilder();
         //            d.CurrentProject = v.Collection.Of<IViewModel>(
         //         })
         //         .Build();
         //}
         public EmployeeVMDescriptor() {
            CurrentProject = new VMPropertyDescriptor<IViewModel>();
            CurrentProject.Behaviors.Successor = new InstancePropertyBehavior<IViewModel>();
            CurrentProject.Behaviors.Initialize(this, CurrentProject);
            Projects = new VMPropertyDescriptor<IVMCollection<IViewModel>>();
            Projects.Behaviors.Successor = new InstancePropertyBehavior<IVMCollection<IViewModel>>();
            Projects.Behaviors.Initialize(this, Projects);
         }

         public IVMPropertyDescriptor<IViewModel> CurrentProject { get; set; }
         public IVMPropertyDescriptor<IVMCollection<IViewModel>> Projects { get; set; }
      }

      private class ProjectVMDescriptor : VMDescriptor {
         public ProjectVMDescriptor() {
            Customer = new VMPropertyDescriptor<IViewModel>();
            Customer.Behaviors.Successor = new InstancePropertyBehavior<IViewModel>();
            Customer.Behaviors.Initialize(this, Customer);
         }

         public IVMPropertyDescriptor<IViewModel> Customer { get; set; }
      }

      private class CustomerVMDescriptor : VMDescriptor {
         public CustomerVMDescriptor() {
            Address = new VMPropertyDescriptor<IViewModel>();
         }

         public IVMPropertyDescriptor<IViewModel> Address { get; set; }
      }

      private class AddressVMDescriptor : VMDescriptor {

      }
   }
}