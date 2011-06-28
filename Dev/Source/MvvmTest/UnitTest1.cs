using System.Collections.Generic;
using Inspiring.Mvvm.ViewModels;
using Inspiring.Mvvm.Views;
using Inspiring.Mvvm.Views.Binder.Xpf;
using Inspiring.MvvmTest.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Sequences;

namespace Inspiring.MvvmTest {
   //public interface IBuilder<out T> {

   //}

   //public static class ExtensionsTest {
   //   public static IBuilder<IViewModel<T>> Test<TDesc, T>(this IBuilder<IViewModel<TDesc>> b, Func<TDesc, IVMPropertyDescriptor<IViewModel<T>>> selector) where TDesc : IVMDescriptor {
   //      return null;
   //   }

   //   public static IBuilder<TC> Test<T, TC>(this IBuilder<T> b, Func<T, TC> selector) {
   //      return null;
   //   }
   //}

   [TestClass]
   public class UnitTest1 : TestBase {


      //[TestMethod]
      //public void MyTestMethod() {
      //   IBuilder<IViewModel<Inspiring.MvvmTest.ApiTests.ViewModels.Undo.ProjectVMDescriptor>> foo = null;
      //   IBuilder<Binding> bar = null;

      //   foo.Test(x => x.Customer);




      //}

      [TestMethod]
      public void MyTestMethod() {
         //ObjectBinder.BindType<TestObject>(b => {
         //   b.Collection(x => x.Collection).To(null).BindItem(i => {
         //      i.Property(x => x.ItemProperty).To(null);
         //   });
         //});

         //ViewModelBinder.BindDescriptor<TestVMDescriptor>(b => {
         //   b.Collection(x => x.Collection).To(null).BindItem(i => {
         //      i.Property(x => x.ItemProperty).To(null);
         //   });
         //});
      }

      private class TestObject {
         public string ObjectProperty { get; set; }
         public ICollection<TestItem> Collection { get; set; }
      }

      public class TestItem {
         public string ItemProperty { get; set; }
      }


      public sealed class TestVM : ViewModel<TestVMDescriptor> {
         public static readonly TestVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<TestVMDescriptor>()
            .For<TestVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();


            })
            .Build();

         public TestVM()
            : base(ClassDescriptor) {
         }
      }

      public sealed class TestVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<TestItemVM>> Collection { get; set; }
      }



      public sealed class TestItemVM : ViewModel<TestItemVMDescriptor> {
         public static readonly TestItemVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<TestItemVMDescriptor>()
            .For<TestItemVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();


            })
            .Build();

         public TestItemVM()
            : base(ClassDescriptor) {
         }
      }

      public sealed class TestItemVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> ItemProperty { get; set; }
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
