namespace Inspiring.MvvmTest {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Reflection;
   using System.Text;
   using System.Threading;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   internal sealed class ParameterizedTest {
      private readonly List<ParameterizedTestCase> _testCases = new List<ParameterizedTestCase>();

      public static ITestCaseBuilder<T> TestCase<T>(T firstParameter) {
         return new TestCaseBuilder<T>(new ParameterizedTest())
            .TestCase(firstParameter);
      }

      public static ITestCaseBuilder<T> TestCase<T>(string testCaseName, T firstParameter) {
         return new TestCaseBuilder<T>(new ParameterizedTest())
            .TestCase(testCaseName, firstParameter);
      }

      public static ITestCaseBuilder<T1, T2> TestCase<T1, T2>(T1 firstParameter, T2 secondParameter) {
         return new TestCaseBuilder<T1, T2>(new ParameterizedTest())
            .TestCase(firstParameter, secondParameter);
      }

      public static ITestCaseBuilder<T1, T2> TestCase<T1, T2>(string testCaseName, T1 firstParameter, T2 secondParameter) {
         return new TestCaseBuilder<T1, T2>(new ParameterizedTest())
            .TestCase(testCaseName, firstParameter, secondParameter);
      }


      private void AddCase(string name, params object[] parameters) {
         AddCase(new ParameterizedTestCase(name, parameters));
      }

      private void AddCase(ParameterizedTestCase testCase) {
         _testCases.Add(testCase);

         if (testCase.Name == null) {
            testCase.Name = _testCases.Count.ToString();
         }
      }

      private void RunTests(Delegate testAction) {
         StringBuilder result = new StringBuilder();

         foreach (var testCase in _testCases) {
            testCase.Execute(testAction, result);
         }

         if (result.Length > 0) {
            Assert.Fail(Environment.NewLine + result.ToString());
         }
      }

      private class ParameterizedTestCase {
         public ParameterizedTestCase(string name = null, params object[] parameters) {
            if (name == null) {
               name = String.Format(
                  "Test case with parameters {0}",
                  String.Join(", ", parameters)
               );
            }

            Name = name;
            Parameters = parameters;
         }

         public string Name { get; set; }
         public IList<object> Parameters { get; private set; }

         public void Execute(Delegate testAction, StringBuilder result) {
            try {
               testAction.DynamicInvoke(Parameters.ToArray());
            } catch (TargetInvocationException ex) {
               var testEx = ex.InnerException as UnitTestAssertException;

               if (testEx != null) {
                  result.AppendFormat(
                     "Test case {0} failed: {1}{2}",
                     Name,
                     testEx.Message,
                     Environment.NewLine
                  );
               } else {
                  if (IsCriticalException(ex.InnerException)) {
                     throw;
                  }

                  result.AppendFormat(
                     "Test case {0} throw exception: {1}{2}",
                     Name,
                     ex.InnerException.ToString(),
                     Environment.NewLine
                  );
               }
            }
         }

         public static bool IsCriticalException(Exception ex) {
            return
               ex is StackOverflowException ||
               ex is OutOfMemoryException ||
               ex is ThreadAbortException ||
               ex is AccessViolationException;
         }
      }

      private class TestCaseBuilder<T> : ITestCaseBuilder<T> {
         private readonly ParameterizedTest _test;

         public TestCaseBuilder(ParameterizedTest test) {
            _test = test;
         }

         public ITestCaseBuilder<T> TestCase(string testCaseName, T firstParameter) {
            _test.AddCase(testCaseName, firstParameter);
            return this;
         }

         public ITestCaseBuilder<T> TestCase(T firstParameter) {
            _test.AddCase(null, firstParameter);
            return this;
         }

         public void Run(Action<T> testAction) {
            _test.RunTests(testAction);
         }
      }

      private class TestCaseBuilder<T1, T2> : ITestCaseBuilder<T1, T2> {
         private readonly ParameterizedTest _test;

         public TestCaseBuilder(ParameterizedTest test) {
            _test = test;
         }

         public ITestCaseBuilder<T1, T2> TestCase(string testCaseName, T1 firstParameter, T2 secondParameter) {
            _test.AddCase(testCaseName, firstParameter, secondParameter);
            return this;
         }

         public ITestCaseBuilder<T1, T2> TestCase(T1 firstParameter, T2 secondParameter) {
            _test.AddCase(null, firstParameter, secondParameter);
            return this;
         }

         public void Run(Action<T1, T2> testAction) {
            _test.RunTests(testAction);
         }
      }
   }

   internal interface ITestCaseBuilder<T> {
      ITestCaseBuilder<T> TestCase(string testCaseName, T firstParameter);
      ITestCaseBuilder<T> TestCase(T firstParameter);
      void Run(Action<T> testCode);
   }

   internal interface ITestCaseBuilder<T1, T2> {
      ITestCaseBuilder<T1, T2> TestCase(string testCaseName, T1 firstParameter, T2 secondParameter);
      ITestCaseBuilder<T1, T2> TestCase(T1 firstParameter, T2 secondParameter);
      void Run(Action<T1, T2> testCode);
   }
}
