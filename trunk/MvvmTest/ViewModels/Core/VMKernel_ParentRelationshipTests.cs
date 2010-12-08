namespace Inspiring.MvvmTest.ViewModels.Core {
   using System;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class VMKernel_ParentRelationshipTests : TestBase {
      // TODO: Verify ChildChanged/SelfChanged...

      [TestMethod]
      public void NotifyChange_InvokesBehavior() {
         var behaviorSpy = new OnChangedSpy();

         var kernel = (IBehaviorContext)CreateKernel(withBehavior: behaviorSpy);

         var changeArgs = new ChangeArgs(ChangeType.ValidationStateChanged, kernel.VM);
         kernel.NotifyChange(changeArgs);

         Assert.AreEqual(1, behaviorSpy.IncovationCount, "Behavior was not called.");
         Assert.AreEqual(changeArgs, behaviorSpy.Args);
         AssertPathsAreEquals(new InstancePath(kernel.VM), behaviorSpy.ChangedPath);
      }

      [TestMethod]
      public void NotifyValidating_InvokesBehavior() {
         var behaviorSpy = new OnValidatingSpy();

         var kernel = (IBehaviorContext)CreateKernel(withBehavior: behaviorSpy);

         var args = ValidationArgs.CreateViewModelValidationArgs(new ValidationState(), new InstancePath(kernel.VM));
         kernel.NotifyValidating(args);

         Assert.AreEqual(1, behaviorSpy.IncovationCount, "Behavior was not called.");
         Assert.AreEqual(args, behaviorSpy.Args);
      }

      [TestMethod]
      public void NotifyChange_InvokesBehaviorOfAncestors() {
         var parentSpy = new OnChangedSpy();
         var grandParentSpy = new OnChangedSpy();

         var kernel = CreateKernel();
         var parentKernel = CreateKernel(withBehavior: parentSpy);
         var grandParentKernel = CreateKernel(withBehavior: grandParentSpy);

         var kernelContext = (IBehaviorContext)kernel;
         var parentContext = (IBehaviorContext)parentKernel;
         var grandParentContext = (IBehaviorContext)grandParentKernel;

         kernel.Parent = parentContext.VM;
         parentKernel.Parent = grandParentContext.VM;

         var changeArgs = new ChangeArgs(ChangeType.ValidationStateChanged, kernelContext.VM);
         kernelContext.NotifyChange(changeArgs);

         Assert.AreEqual(1, parentSpy.IncovationCount, "Behavior of parent was not invoked.");
         Assert.AreEqual(changeArgs, parentSpy.Args);
         AssertPathsAreEquals(new InstancePath(parentContext.VM, kernelContext.VM), parentSpy.ChangedPath);

         Assert.AreEqual(1, grandParentSpy.IncovationCount);
         Assert.AreEqual(changeArgs, grandParentSpy.Args, "Behavior of grand parent was not invoked.");
         AssertPathsAreEquals(new InstancePath(grandParentContext.VM, parentContext.VM, kernelContext.VM), grandParentSpy.ChangedPath);
      }

      [TestMethod]
      public void NotifyValidating_InvokesBehaviorOfAncestors() {
         var parentSpy = new OnValidatingSpy();
         var grandParentSpy = new OnValidatingSpy();

         var kernel = CreateKernel();
         var parentKernel = CreateKernel(withBehavior: parentSpy);
         var grandParentKernel = CreateKernel(withBehavior: grandParentSpy);

         var kernelContext = (IBehaviorContext)kernel;
         var vm = ((IBehaviorContext)kernel).VM;
         var parentVM = ((IBehaviorContext)parentKernel).VM;
         var grandParentVM = ((IBehaviorContext)grandParentKernel).VM;

         kernel.Parent = parentVM;
         parentKernel.Parent = grandParentVM;

         var args = ValidationArgs.CreateViewModelValidationArgs(new ValidationState(), new InstancePath(kernelContext.VM));
         kernelContext.NotifyValidating(args);

         Assert.AreEqual(1, parentSpy.IncovationCount, "Behavior of parent was not invoked.");
         Assert.AreSame(args.Errors, parentSpy.Args.Errors);
         Assert.AreSame(args.ChangedProperty, parentSpy.Args.ChangedProperty);
         Assert.AreSame(args.TargetProperty, parentSpy.Args.TargetProperty);
         AssertPathsAreEquals(args.ChangedPath, parentSpy.Args.ChangedPath);
         AssertPathsAreEquals(new InstancePath(parentVM, vm), parentSpy.Args.TargetPath);

         Assert.AreEqual(1, grandParentSpy.IncovationCount);
         Assert.AreSame(args.Errors, grandParentSpy.Args.Errors);
         Assert.AreSame(args.ChangedProperty, grandParentSpy.Args.ChangedProperty);
         Assert.AreSame(args.TargetProperty, grandParentSpy.Args.TargetProperty);
         AssertPathsAreEquals(args.ChangedPath, grandParentSpy.Args.ChangedPath);
         AssertPathsAreEquals(new InstancePath(grandParentVM, parentVM, vm), grandParentSpy.Args.TargetPath);
      }

      private VMKernel CreateKernel(ViewModelBehavior withBehavior = null) {
         var vmMock = new Mock<IViewModel>();

         var descriptor = new VMDescriptorStub();
         descriptor.Behaviors.Successor = withBehavior;

         var kernel = new VMKernel(vmMock.Object, descriptor, ServiceLocator.Current);
         vmMock.Setup(x => x.Kernel).Returns(kernel);

         return kernel;
      }

      private class OnChangedSpy : ViewModelBehavior {
         protected internal override void OnChanged(IBehaviorContext context, ChangeArgs args, InstancePath changedPath) {
            base.OnChanged(context, args, changedPath);
            IncovationCount++;
            Args = args;
            ChangedPath = changedPath;
         }

         public int IncovationCount { get; private set; }

         public ChangeArgs Args { get; private set; }

         public InstancePath ChangedPath { get; private set; }
      }

      private class OnValidatingSpy : ViewModelBehavior {
         protected internal override void OnValidating(IBehaviorContext context, ValidationArgs args) {
            IncovationCount++;
            Args = args;
         }

         public int IncovationCount { get; private set; }

         public ValidationArgs Args { get; private set; }
      }

      private static void AssertPathsAreEquals(InstancePath expected, InstancePath actual) {
         AssertHelper.AreEqual(expected.Steps, actual.Steps, StepsAreEqual, "Instance paths are not equal.");
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