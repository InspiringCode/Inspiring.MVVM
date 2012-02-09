namespace Inspiring.MvvmTest.ViewModels.Tracing {
   using System;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.ViewModels.Tracing;

   [TestClass]
   public class TraceBuilderTests {
      [TestMethod]
      public void BeginEntryAndEndLastEntry_BuildCorrectTree() {
         var root = new TestEntry("root");
         var child1 = new TestEntry("child1");
         var child2 = new TestEntry("child2");
         var grandchild1 = new TestEntry("grandchild1");
         var grandchild2 = new TestEntry("grandchild2");
         var grandchild3 = new TestEntry("grandchild3");

         var builder = new TraceBuilder();
         builder.IsEnabled = true;

         builder.BeginEntry(root);
         builder.BeginEntry(child1);
         
         builder.BeginEntry(grandchild1);
         builder.EndLastEntry();
         
         builder.BeginEntry(grandchild2);
         builder.EndLastEntry();

         builder.EndLastEntry();
         builder.BeginEntry(child2);
         
         builder.BeginEntry(grandchild3);
         builder.EndLastEntry();

         builder.EndLastEntry();
         builder.EndLastEntry();

         CollectionAssert.AreEqual(
            new[] { root },
            builder.Root.Children.ToArray()
         );

         CollectionAssert.AreEqual(
            new[] { child1, child2 },
            root.Children.ToArray()
         );

         CollectionAssert.AreEqual(
            new[] { grandchild1, grandchild2},
            child1.Children.ToArray()
         );

         CollectionAssert.AreEqual(
            new[] { grandchild3 },
            child2.Children.ToArray()
         );

         Assert.AreEqual(0, grandchild1.Children.Count);
         Assert.AreEqual(0, grandchild2.Children.Count);
         Assert.AreEqual(0, grandchild3.Children.Count);
      }

      private class TestEntry : TraceEntry {
         private readonly string _text;

         public TestEntry(string text) {
            _text = text;
         }

         public override string ToString() {
            return _text;
         }
      }
   }
}