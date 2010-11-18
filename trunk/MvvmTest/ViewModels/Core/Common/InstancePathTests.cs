namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using System.Collections;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
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

         AssertHelper.Throws<InvalidOperationException>(() =>
            path.PrependCollection(coll)
         );
      }

      [TestMethod]
      public void PrependCollection_WithTwoStepPath_ModifiesFirstStep() {
         var vm1 = new Mock<IViewModel>().Object;
         var vm2 = new Mock<IViewModel>().Object;
         var coll = new Mock<IEnumerable>().Object;

         var path = new InstancePath();
         path = path.PrependVM(vm1);
         path = path.PrependVM(vm2);
         path.PrependCollection(coll);

         var step = path.Steps.First();
         Assert.AreSame(coll, step.ParentCollection);
      }

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
      public void Subpath_StartOneCountMaxInt_ReturnsOneStepPath() {
         var vm1 = new Mock<IViewModel>().Object;
         var vm2 = new Mock<IViewModel>().Object;

         var path = new InstancePath(vm1, vm2);

         var result = path.Subpath(1, Int32.MaxValue);
         AssertHelper.AreEqual(CreateSteps(vm2), result.Steps, StepsAreEqual);
      }

      [TestMethod]
      public void MatchStart_WithOnePropertyOnDefaultInstance_DoesNotMatch() {
         var currentProjectProperty = new Mock<IVMProperty>().Object;
         var properties = new VMPropertyPath(currentProjectProperty);

         var path = new InstancePath();
         InstancePathMatch match = path.MatchStart(properties);

         Assert.IsFalse(match.Success);
      }

      [TestMethod]
      public void MatchStart_WithOnePropertyOnOneStepPath_DoesNotMatch() {
         var currentProjectProperty = new Mock<IVMProperty>().Object;
         var properties = new VMPropertyPath(currentProjectProperty);

         var employeeVM = new Mock<IViewModel>().Object;
         var path = new InstancePath(employeeVM);
         InstancePathMatch match = path.MatchStart(properties);

         Assert.IsFalse(match.Success);
      }

      [TestMethod]
      public void MatchStart_EmptyPropertyPath_DoesMatch() {
         var vm = new Mock<IViewModel>().Object;
         var path = new InstancePath(vm);
         var result = path.MatchStart(new VMPropertyPath());

         Assert.IsTrue(result.Success);
         AssertHelper.AreEqual(CreateSteps(vm), result.MatchedPath.Steps, StepsAreEqual);
         Assert.IsTrue(result.RemainingPath.IsEmpty);
      }

      [TestMethod]
      public void MatchStart_WithTwoMatcingPropertiesOnFourStepPath_DoesMatch() {
         var currentProjectProperty = new Mock<IVMProperty>().Object;
         var customerProperty = new Mock<IVMProperty>().Object;
         var properties = new VMPropertyPath(currentProjectProperty, customerProperty);

         var employeeVM = new Mock<IViewModel>();
         var projectVM = new Mock<IViewModel>();
         var customerVM = new Mock<IViewModel>();
         var addressVM = new Mock<IViewModel>();

         employeeVM.Setup(x => x.GetValue(currentProjectProperty, ValueStage.PreValidation)).Returns(projectVM.Object);
         projectVM.Setup(x => x.GetValue(customerProperty, ValueStage.PreValidation)).Returns(customerVM.Object);

         var path = new InstancePath(addressVM.Object);
         path = path.PrependVM(customerVM.Object);
         path = path.PrependVM(projectVM.Object);
         path = path.PrependVM(employeeVM.Object);

         InstancePathMatch match = path.MatchStart(properties);

         Assert.IsTrue(match.Success);

         AssertHelper.AreEqual(
            CreateSteps(employeeVM.Object, projectVM.Object, customerVM.Object),
            match.MatchedPath.Steps,
            StepsAreEqual
         );

         AssertHelper.AreEqual(
            CreateSteps(addressVM.Object),
            match.RemainingPath.Steps,
            StepsAreEqual
         );
      }

      [TestMethod]
      public void MatchStart_WithTwoNonMatchingPropertiesOnThreeStepPath_DoesNotMatch() {
         var currentProjectProperty = new Mock<IVMProperty>().Object;
         var customerProperty = new Mock<IVMProperty>().Object;
         var properties = new VMPropertyPath(currentProjectProperty, customerProperty);

         var employeeVM = new Mock<IViewModel>();
         var projectVM = new Mock<IViewModel>();
         var customerVM = new Mock<IViewModel>();

         employeeVM.Setup(x => x.GetValue(currentProjectProperty, ValueStage.PreValidation)).Returns(projectVM.Object);
         projectVM.Setup(x => x.GetValue(customerProperty, ValueStage.PreValidation)).Returns(new Object());

         var path = new InstancePath(customerVM.Object);
         path = path.PrependVM(projectVM.Object);
         path = path.PrependVM(employeeVM.Object);

         InstancePathMatch match = path.MatchStart(properties);

         Assert.IsFalse(match.Success);
      }

      [TestMethod]
      public void MatchStart_WithCollectionProperty_DoesMatch() {
         var projectsProperty = new Mock<IVMProperty>().Object;
         var customerProperty = new Mock<IVMProperty>().Object;
         var properties = new VMPropertyPath(projectsProperty, customerProperty);

         var employeeVM = new Mock<IViewModel>();
         var projectVM = new Mock<IViewModel>();
         var customerVM = new Mock<IViewModel>();

         var projectsCollection = new Mock<IEnumerable>().Object;

         employeeVM.Setup(x => x.GetValue(projectsProperty, ValueStage.PreValidation)).Returns(projectsCollection);
         projectVM.Setup(x => x.GetValue(customerProperty, ValueStage.PreValidation)).Returns(customerVM.Object);

         var path = new InstancePath(customerVM.Object);

         path = path.PrependVM(projectVM.Object);
         path.PrependCollection(projectsCollection);

         path = path.PrependVM(employeeVM.Object);

         InstancePathMatch match = path.MatchStart(properties);

         Assert.IsTrue(match.Success);

         AssertHelper.AreEqual(
            new InstancePathStep[] { 
               new InstancePathStep(employeeVM.Object), 
               new InstancePathStep(projectVM.Object) { ParentCollection = projectsCollection },
               new InstancePathStep(customerVM.Object)
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
            Object.ReferenceEquals(x.VM, y.VM) &&
            Object.ReferenceEquals(x.ParentCollection, y.ParentCollection);
      }
   }
}