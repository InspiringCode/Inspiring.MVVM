namespace Inspiring.MvvmTest.ViewModels.Tracing {
   using Inspiring.Mvvm.ViewModels.Tracing;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using System;

   [TestClass]
   public class TraceEntryTests {
      [TestMethod]
      public void ToStringWithChildren_ConcatsEntriesCorrectly() {
         var root = new TestEntry("root");
         var child1 = new TestEntry("child1");
         var child2 = new TestEntry("child2");
         var grandchild1 = new TestEntry("grandchild1");
         var grandchild2 = new TestEntry("grandchild2");
         var grandchild3 = new TestEntry("grandchild3");

         root.Children.Add(child1);
         root.Children.Add(child2);

         child1.Children.Add(grandchild1);
         child1.Children.Add(grandchild2);
         child2.Children.Add(grandchild3);

         string expected = String.Format(
            "{1}{0}\t{2}{0}\t\t{3}{0}\t\t{4}{0}\t{5}{0}\t\t{6}{0}",
            Environment.NewLine,
            root,
            child1,
            grandchild1,
            grandchild2,
            child2,
            grandchild3
         );

         string actual = root.ToStringWithChildren();

         Assert.AreEqual(expected, actual);
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