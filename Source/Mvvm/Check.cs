using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Inspiring.Mvvm {
   /// <summary>
   ///   Provides static methods for parameter checks (similar to code contracts).
   /// </summary>
   /// <remarks>
   ///   Use 'nameof' operator for 'parameterName' parameters!
   /// </remarks>
   [DebuggerStepThrough]
   internal static class Check {
      /// <summary>
      ///   Verifies that the supplied value is not a null reference.
      /// </summary>
      /// <param name="value">The value to verify.</param>
      /// <param name="parameterName">Use 'nameof' operator!</param>
      /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is null.</exception>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void NotNull(object value, string parameterName) {
         if (value == null) {
            BreakIfDebugging();

            // ArgumentNullException is not used here because the method might also be used to check
            // properties on arguments instead of the argument itself.
            throw new ArgumentException("The argument or a property on the argument cannot be null.", parameterName);
         }
      }

      /// <summary>
      ///   Verifies that the supplied value is not a null reference.
      /// </summary>
      /// <param name="value">The value to verify.</param>
      /// <param name="message">Optional: The error message that explains the reason for the exception.</param>
      /// <typeparam name="TException">The type of the exception that will be thrown.</typeparam>
      /// <remarks>
      ///   Can be used to check something that isn't an argument (e.g. with <see cref="InvalidOperationException"/>).
      /// </remarks>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void NotNull<TException>(object value, string message = null) where TException : Exception {
         if (value == null) {
            BreakIfDebugging();
            throw (TException)Activator.CreateInstance(typeof(TException), message);
         }
      }

      /// <summary>
      ///   Verifies that the supplied string value is neither a null reference nor an empty string.
      /// </summary>
      /// <param name="value">The value to verify.</param>
      /// <param name="parameterName">Use 'nameof' operator!</param>
      /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is null or an empty string.</exception>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void NotEmpty(string value, string parameterName) {
         if (String.IsNullOrEmpty(value)) {
            BreakIfDebugging();
            throw new ArgumentException("The argument or a property on the argument cannot be null or an empty string.", parameterName);
         }
      }

      /// <summary>
      ///   Verifies that the supplied enumerable is neither a null reference nor empty.
      /// </summary>
      /// <param name="value">The value to verify.</param>
      /// <param name="parameterName">Use 'nameof' operator!</param>
      /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is null or an empty enumerable.</exception>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void NotEmpty<T>(IEnumerable<T> value, string parameterName) {
         if (value == null || !value.Any()) {
            BreakIfDebugging();
            throw new ArgumentException("The argument or a property on the argument cannot be null or an empty enumerable.", parameterName);
         }
      }

      /// <summary>
      ///   Verifies that the supplied value is not the default value of the type <typeparamref name="T"/>.
      /// </summary>
      /// <param name="value">The value to verify.</param>
      /// <param name="parameterName">Use 'nameof' operator!</param>
      /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> equals the default value of <typeparamref name="T"/>.</exception>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void NotDefault<T>(T value, string parameterName) where T : struct {
         if (value.Equals(default(T))) {
            BreakIfDebugging();
            throw new ArgumentException("The argument or a property on the argument cannot have the default value.", parameterName);
         }
      }

      /// <summary>
      ///   Verifies that the supplied boolean value is true.
      /// </summary>
      /// <param name="condition">The condition to verify.</param>
      /// <param name="message">Optional: The error message that explains the reason for the exception.</param>
      /// <param name="parameterName">Optional: The name of the parameter that caused the current exception. Use 'nameof' operator!</param>
      /// <exception cref="ArgumentException">Thrown if <paramref name="condition"/> is false.</exception>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void Requires(bool condition, string message = null, string parameterName = null) {
         if (!condition) {
            BreakIfDebugging();
            throw new ArgumentException(message, parameterName);
         }
      }

      /// <summary>
      ///   Verifies that the supplied boolean value is true.
      /// </summary>
      /// <param name="condition">The condition to verify.</param>
      /// <param name="message">Optional: The error message that explains the reason for the exception.</param>
      /// <typeparam name="TException">The type of exception that is thrown.</typeparam>
      /// <remarks>
      ///   Can be used to check something that isn't an argument (e.g. with <see cref="InvalidOperationException"/>).
      /// </remarks>
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static void Requires<TException>(bool condition, string message = null) where TException : Exception {
         if (!condition) {
            BreakIfDebugging();
            throw (TException)Activator.CreateInstance(typeof(TException), message);
         }
      }

      /// <summary>
      ///   Breaks execution if a debugger is attached. Only compiled in debug builds.
      /// </summary>
      [Conditional("DEBUG")]
      private static void BreakIfDebugging() {
         if (Debugger.IsAttached) {
            Debugger.Break();
         }
      }
   }
}
