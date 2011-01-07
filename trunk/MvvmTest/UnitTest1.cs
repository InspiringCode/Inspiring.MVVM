using System;
using System.ComponentModel;
using Inspiring.Mvvm;
using Inspiring.Mvvm.ViewModels;
using Inspiring.Mvvm.ViewModels.Core;
using Inspiring.Mvvm.ViewModels.Fluent;
using Inspiring.MvvmTest.Stubs;
using Inspiring.MvvmTest.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Sequences;

namespace Inspiring.MvvmTest {
   [TestClass]
   public class UnitTest1 : TestBase {
      //private class AVMDescriptor : VMDescriptor {
      //}

      //private class BVMDescriptor : AVMDescriptor {
      //}

      //private class AVM : IViewModel<AVMDescriptor> {
      //}

      //private class BVM : AVM {
      //}

      [TestMethod]
      public void MyTestMethod3() {
         //VMDescriptorBuilder<PersonVMDescriptor> test = null;
      }

      [TestMethod]
      public void MyTestMethod2() {
         ICustomTypeDescriptor d = new Test();
         Console.WriteLine(d.GetDefaultEvent());

         var b = new ValidatorBuilder<TestVM, TestVMDescriptor>(null, null);
         b.Check(x => x.LocalProperty);
         b.CheckCollection<ChildVM>(x => x.MappedCollectionProperty);
         b.CheckCollection<ChildVMDescriptor, string>(x => x.MappedCollectionProperty, x => x.MappedMutableProperty);

         b.CheckCollection(x => x.MappedCollectionProperty);
         b.CheckCollection(x => x.MappedCollectionProperty, x => x.MappedMutableProperty);

         b.Check(x => x.LocalProperty).Custom((vm, value, args) => { });
      }

      internal static void Validate(TestVM vm, decimal value, ValidationArgs args) {
         IVMPropertyBuilder<TestVM> x = null;

      }

      private class Test : ViewModelTypeDescriptor {

         protected override PropertyDescriptorCollection GetPropertyDescriptors() {
            throw new NotImplementedException();
         }
      }

      [TestMethod]
      public void MyTestMethod() {
         var kernel = new VMKernel(Mock<IViewModel>(), new VMDescriptorStub(), ServiceLocator.Current);

         var itemMock = new Mock<IViewModel>();
         itemMock.Setup(x => x.Kernel).Returns(kernel);

         var parent = Mock<IViewModel>();

         itemMock.Object.Kernel.Parent = parent;

         Assert.AreSame(parent, itemMock.Object.Kernel.Parent);

         DisplayValueAccessorBehavior<int> b = null;
         //b.CallNext(x => x.SetValue(null, null));
         //b.CallNext(x => x.GetValue(null));

         var mock = new Mock<IFoo>();

         mock.Setup(x => x.N(27)).Verifiable();

         mock.Object.N(27);

         mock.Verify();

         Action<int> action = x => x++;
         Console.WriteLine(action);

         action = Action1;
         Console.WriteLine(action);

         action = Action2;
         Console.WriteLine(action);
      }

      private void Action1(int x) {

      }

      private static void Action2(int x) {

      }

      [TestMethod]
      public void ThisShouldWork2() {
         var a = new Mock<IFoo>(MockBehavior.Strict);

         using (Sequence.Create()) {
            a.Setup(x => x.M(100)).InSequence().Returns(101);
            a.Setup(x => x.M(200)).InSequence().Returns(201);

            a.Object.M(100);
            a.Object.M(200);
         }
      }

      [TestMethod]
      [ExpectedException(typeof(SequenceException))]
      public void ThisShouldNotWork2() {
         var a = new Mock<IFoo>(MockBehavior.Strict);

         using (Sequence.Create()) {
            a.Setup(x => x.M(100)).InSequence().Returns(101);
            a.Setup(x => x.N(100)).InSequence().Returns(201);

            a.Object.N(100);
            a.Object.M(100);
         }
      }

      // [TestMethod]
      public void ThisShouldWork() {
         var a = new Mock<IFoo>(MockBehavior.Strict);

         MockSequence t = new MockSequence();
         a.InSequence(t).Setup(x => x.M(100)).Returns(101);
         a.InSequence(t).Setup(x => x.M(200)).Returns(201);

         a.Object.M(100);
         a.Object.M(200);
      }

      // [TestMethod]
      public void ThisShouldNotWork() {
         var a = new Mock<IFoo>(MockBehavior.Strict);

         MockSequence t = new MockSequence();
         a.InSequence(t).Setup(x => x.M(100)).Returns(101);
         a.InSequence(t).Setup(x => x.N(100)).Returns(201);

         a.Object.N(100);
         a.Object.M(100);
      }

      public interface IFoo {
         int M(int p);
         int N(int p);
      }
   }
}
