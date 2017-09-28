using System;
using System.Collections.Generic;
using System.Linq;
using Inspiring.Mvvm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest {
   [TestClass]
   public class CheckTests {
      private const string ParameterName = "param";

      [TestMethod]
      public void NotNull_NullArgument_ThrowsArgumentException() {
         var ex = AssertHelper.Throws<ArgumentException>(() =>
             Check.NotNull(null, ParameterName)).Exception;

         Assert.AreEqual(ParameterName, ex.ParamName);
         Assert.IsNull(ex.InnerException);
      }

      [TestMethod]
      public void NotNull_ValidArguments_ThrowsNoException() {
         Check.NotNull(new object(), ParameterName);
      }

      [TestMethod]
      public void NotNullWithCustomException_NullArgument_ThrowsCustomException() {
         var ex = AssertHelper.Throws<InvalidOperationException>(() =>
             Check.NotNull<InvalidOperationException>(null)).Exception;

         Assert.IsNull(ex.InnerException);

         var ex2 = AssertHelper.Throws<ArgumentException>(() =>
             Check.NotNull<ArgumentException>(null)).Exception;

         Assert.IsNull(ex2.InnerException);
      }

      [TestMethod]
      public void NotNullWithCustomException_NullArgumentWithMessage_ThrowsCustomException() {
         const string message = "Test Message";
         var ex = AssertHelper.Throws<InvalidOperationException>(() =>
             Check.NotNull<InvalidOperationException>(null, message)).Exception;

         Assert.AreEqual(message, ex.Message);
         Assert.IsNull(ex.InnerException);

         var ex2 = AssertHelper.Throws<ArgumentException>(() =>
             Check.NotNull<ArgumentException>(null, message)).Exception;

         Assert.AreEqual(message, ex2.Message);
         Assert.IsNull(ex2.InnerException);
      }

      [TestMethod]
      public void NotNullWithCustomException_ValidArgument_ThrowsNoException() {
         Check.NotNull<InvalidOperationException>(new object());
      }

      [TestMethod]
      public void NotEmptyString_NullArgument_ThrowsArgumentException() {
         var ex = AssertHelper.Throws<ArgumentException>(() =>
             Check.NotEmpty((string)null, ParameterName)).Exception;

         Assert.AreEqual(ParameterName, ex.ParamName);
         Assert.IsNull(ex.InnerException);
      }

      [TestMethod]
      public void NotEmptyString_EmptyStringArgument_ThrowsArgumentException() {
         var ex = AssertHelper.Throws<ArgumentException>(() =>
             Check.NotEmpty(String.Empty, ParameterName)).Exception;

         Assert.AreEqual(ParameterName, ex.ParamName);
         Assert.IsNull(ex.InnerException);
      }

      [TestMethod]
      public void NotEmptyString_ValidArgument_ThrowsNoException() {
         Check.NotEmpty("A", ParameterName);
         Check.NotEmpty(" ", ParameterName);
      }

      [TestMethod]
      public void NotEmptyEnumerable_NullArgument_ThrowsArgumentException() {
         var ex = AssertHelper.Throws<ArgumentException>(() =>
             Check.NotEmpty((IEnumerable<object>)null, ParameterName)).Exception;

         Assert.AreEqual(ParameterName, ex.ParamName);
         Assert.IsNull(ex.InnerException);
      }

      [TestMethod]
      public void NotEmptyEnumerable_EmptyEnumerableArgument_ThrowsArgumentException() {
         var ex = AssertHelper.Throws<ArgumentException>(() =>
             Check.NotEmpty(Enumerable.Empty<object>(), ParameterName)).Exception;

         Assert.AreEqual(ParameterName, ex.ParamName);
         Assert.IsNull(ex.InnerException);
      }

      [TestMethod]
      public void NotEmptyEnumerable_ValidArgument_ThrowsNoException() {
         Check.NotEmpty(new[] { new object() }, ParameterName);
         Check.NotEmpty(new[] { 1, 2, 3, 4 }, ParameterName);
         Check.NotEmpty(Enumerable.Repeat("test", 3), ParameterName);
      }

      [TestMethod]
      public void NotDefault_CtorGuidArgument_ThrowsArgumentException() {
         var ex = AssertHelper.Throws<ArgumentException>(() =>
             Check.NotDefault(new Guid(), ParameterName)).Exception;

         Assert.AreEqual(ParameterName, ex.ParamName);
         Assert.IsNull(ex.InnerException);
      }

      [TestMethod]
      public void NotDefault_CtorDateTimeArgument_ThrowsArgumentException() {
         var ex = AssertHelper.Throws<ArgumentException>(() =>
             Check.NotDefault(new DateTime(), ParameterName)).Exception;

         Assert.AreEqual(ParameterName, ex.ParamName);
         Assert.IsNull(ex.InnerException);
      }

      [TestMethod]
      public void NotDefault_ValidArgument_ThrowsNoException() {
         Check.NotDefault(Guid.NewGuid(), ParameterName);
         Check.NotDefault(DateTime.Now, ParameterName);
      }

      [TestMethod]
      public void Requires_FalseArgument_ThrowsArgumentException() {
         var ex = AssertHelper.Throws<ArgumentException>(() =>
             Check.Requires(false)).Exception;

         Assert.IsNull(ex.ParamName);
         Assert.IsNull(ex.InnerException);
      }

      [TestMethod]
      public void Requires_FalseArgumentWithMessage_ThrowsArgumentException() {
         const string message = "Test Message";
         var ex = AssertHelper.Throws<ArgumentException>(() =>
             Check.Requires(false, message)).Exception;

         Assert.AreEqual(message, ex.Message);
         Assert.IsNull(ex.ParamName);
         Assert.IsNull(ex.InnerException);
      }

      [TestMethod]
      public void Requires_FalseArgumentWithParamName_ThrowsArgumentException() {
         const string message = "Test Message";
         var ex = AssertHelper.Throws<ArgumentException>(() =>
             Check.Requires(false, message, ParameterName)).Exception;

         Assert.IsTrue(ex.Message.Contains(message));
         Assert.AreEqual(ParameterName, ex.ParamName);
         Assert.IsNull(ex.InnerException);
      }

      [TestMethod]
      public void Requires_TrueArgument_ThrowsNoException() {
         Check.Requires(true);
      }

      [TestMethod]
      public void RequiresWithCustomException_FalseArgument_ThrowsCustomException() {
         var ex = AssertHelper.Throws<InvalidOperationException>(() =>
             Check.Requires<InvalidOperationException>(false)).Exception;

         Assert.IsNull(ex.InnerException);
      }

      [TestMethod]
      public void RequiresWithCustomException_FalseArgumentWithMessage_ThrowsCustomException() {
         const string message = "Test Message";
         var ex = AssertHelper.Throws<InvalidOperationException>(() =>
             Check.Requires<InvalidOperationException>(false, message)).Exception;

         Assert.AreEqual(message, ex.Message);
         Assert.IsNull(ex.InnerException);
      }

      [TestMethod]
      public void RequiresWithCustomException_TrueArgument_ThrowsNoException() {
         Check.Requires<InvalidOperationException>(true);
      }
   }
}
