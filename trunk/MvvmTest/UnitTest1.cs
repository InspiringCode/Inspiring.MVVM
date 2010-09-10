using System;
using System.Reflection;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Inspiring.MvvmTest {
   /// <summary>
   /// Summary description for UnitTest1
   /// </summary>
   [TestClass]
   public class UnitTest1 {
      public class MyAttribute : Attribute {
         public MyAttribute(object obj) {
            INeedsInitialization<object> bar = new Handler();
            INeedsInitialization<B> x = bar;

         }
      }

      [TestMethod]
      public void Tryis() {
         var x = new Handler();
         Assert.IsTrue(x is INeedsInitialization<int>);
      }

      class Handler : INeedsInitialization<object> {

         public void Init(object subject) {
            throw new NotImplementedException();
         }
      }

      public interface INeedsInitialization<in T> {
         void Init(T subject);
      }

      class A {

      }

      class B : A {

      }

      //public class Test<T> {
      //   public void Bar<U>() where T : U { }
      //}

      //public class Foo<T> {
      //   public void Bar<Z>() where T : Z {

      //   }
      //}

      [TestMethod]
      public void MyTestMethod() {
         Window w = new Window();
         w.Closing += new System.ComponentModel.CancelEventHandler(w_Closing);
         w.Closed += new EventHandler(w_Closed);
         w.Close();
      }

      void w_Closed(object sender, EventArgs e) {
         Assert.Fail("Yeah. It worked...");
      }

      void w_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
         e.Cancel = true;
      }

      [TestMethod]
      public void ThisShouldWork() {
         var a = new Mock<IFoo>(MockBehavior.Strict);

         MockSequence t = new MockSequence();
         a.InSequence(t).Setup(x => x.M(100)).Returns(101);
         a.InSequence(t).Setup(x => x.M(200)).Returns(201);

         a.Object.M(100);
         a.Object.M(200);
      }

      [TestMethod]
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


      //private void Test<T, T2>(Func<ViewModel<T>> selector, Func<T, T2> mapper) {

      //}

      public void Test2() {

      }

      //private class A {

      //}

      //private class B : A {
      //   public object FirstName { get; set; }
      //}

      //private class ViewModel<TDesc> where TDesc : A {
      //   public static TDesc Descriptor { get { return null; } }
      //}

      private TestContext testContextInstance;

      /// <summary>
      ///Gets or sets the test vm which provides
      ///information about and functionality for the current test run.
      ///</summary>
      public TestContext TestContext {
         get {
            return testContextInstance;
         }
         set {
            testContextInstance = value;
         }
      }

      #region Additional test attributes
      //
      // You can use the following additional attributes as you write your tests:
      //
      // Use ClassInitialize to run code before running the _first test in the class
      // [ClassInitialize()]
      // public static void MyClassInitialize(TestContext testContext) { }
      //
      // Use ClassCleanup to run code after all tests in a class have run
      // [ClassCleanup()]
      // public static void MyClassCleanup() { }
      //
      // Use TestInitialize to run code before running each test 
      // [TestInitialize()]
      // public void MyTestInitialize() { }
      //
      // Use TestCleanup to run code after each test has run
      // [TestCleanup()]
      // public void MyTestCleanup() { }
      //
      #endregion

      [TestMethod]
      public void TestMethod1() {
         Delegate d = Delegate.CreateDelegate(typeof(Func<TestClass, int>), typeof(TestClass).GetProperty("Test", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true));

         int x = ((Func<TestClass, int>)d)(new TestSubClass());

         Assert.AreEqual(5, x);

         Assert.AreEqual(5, d.DynamicInvoke(new TestClass()));

         //
         // TODO: Add test logic here
         //
      }

      //private class PropertyAccessor {
      //   PropertyAccessor Create(PropertyInfo property) {

      //   }
      //}

      //private class PropertyAccessor<TObject, TProperty> {

      //}

      private class TestClass {
         private int Test { get; set; }

         public TestClass() {
            Test = 5;
         }
      }

      private class TestSubClass : TestClass {

      }
   }
}
