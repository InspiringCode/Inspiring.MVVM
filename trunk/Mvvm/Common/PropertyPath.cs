namespace Inspiring.Mvvm.Common {
   using System;
   using System.Collections.Generic;
   using System.Diagnostics.Contracts;
   using System.Linq;
   using System.Linq.Expressions;
   using System.Reflection;

   /// <summary>
   /// An object that gets or sets the value of a property or multi-step property
   /// path ('Address.City') of an object.
   /// </summary>
   internal partial class PropertyPath {
      /// <summary>
      ///   Creates a new 'PropertyPath' that throws a 'NullReferenceException'
      ///   when 'GetValue' or 'SetValue' is called and one of the property path 
      ///   steps (except the last) return null. To create a 'PropertyPath' for 
      ///   the 'Address.City' property of the 'Person' class call 
      ///   '<![CDATA[Create<Person, string>(p => p.Address.City)]]>'.
      /// </summary>
      public static PropertyPath<TSource, TValue> Create<TSource, TValue>(
         Expression<Func<TSource, TValue>> propertyPathSelector
      ) {
         return new PropertyPath<TSource, TValue>(
            ExpressionService.GetProperties(propertyPathSelector),
            useDefaultValue: false
         );
      }

      /// <summary>
      ///   Creates a new 'PropertyPath' that returns the given 'defaultValue'
      ///   when 'GetValue' is called and one of the property path steps (except 
      ///   the last) returns null. Calls to 'SetValue' do nothinng in this case.
      ///   To create a 'PropertyPath' for the 'Address.City' property of the 
      ///   'Person' class call  '<![CDATA[
      ///      CreateWithDefaultValue<Person, string>(p => p.Address.City, "Unknown")
      ///   ]]>'.
      /// </summary>
      public static PropertyPath<TSource, TValue> CreateWithDefaultValue<TSource, TValue>(
         Expression<Func<TSource, TValue>> propertyPathSelector,
         TValue defaultValue = default(TValue)
      ) {
         return new PropertyPath<TSource, TValue>(
            ExpressionService.GetProperties(propertyPathSelector),
            useDefaultValue: true,
            defaultValue: defaultValue
         );
      }

      /// <summary>
      ///   Combines to PropertyPath objects into one object. Calling 'Conact' for
      ///   '[Person].Address' and '[Address].City.PostalCode' returns the path
      ///   '[Person].Address.City.PostalCode'.
      /// </summary>
      public static PropertyPath<TSource, TValue> Concat<TSource, T, TValue>(
         PropertyPath<TSource, T> first,
         PropertyPath<T, TValue> second
      ) {
         return PropertyPath<TSource, TValue>.Concat(first, second);
      }

      /// <summary>
      ///   Creates an empty property path which simple return the source value when
      ///   'GetValue' is called and throws an expcetion when 'SetValue' is called. 
      ///   This construct is useful when concating paths.
      /// </summary>
      public static PropertyPath<T, T> Empty<T>() {
         return PropertyPath.Create<T, T>(o => o);
      }
   }

   /// <summary>
   /// An object that gets or sets the value of a property or multi-step property
   /// path ('Address.City') of an object.
   /// </summary>
   internal partial class PropertyPath<TSource, TValue> : PropertyPath {
      private Step[] _prefixSteps;
      private Step _lastStep;
      private bool _useDefaultValue;
      private TValue _defaultValue;

      /// <summary>
      ///   Use 'PropertyPath.Create' to create a new property path!  
      /// </summary>
      /// <param propertyName="properties">
      ///   Pass the 'PropertyInfo' for each single step of the property path. 
      ///   For [Person].Address.City pass the 'PropertyInfo' for 'Address' and
      ///   for 'City'.
      /// </param>
      /// <param propertyName="useDefaultValue">
      ///   If true and one of the property steps returns null, the specified
      ///   'defaultValue' is returned by the 'GetValue' method and 'SetValue'
      ///   operations are ignored. If false, an exception is thrown in this 
      ///   case.
      /// </param>
      internal PropertyPath(
         PropertyInfo[] properties,
         bool useDefaultValue,
         TValue defaultValue = default(TValue)
      )
         : this(
            properties
               .Select(pi => new Step(pi))
               .ToArray(),
            useDefaultValue,
            defaultValue
         ) {
         Contract.Requires(properties != null);
      }

      /// <summary>
      /// Used by the 'Concat' method.
      /// </summary>
      private PropertyPath(Step[] steps, bool useDefaultValue, TValue defaultValue) {
         if (steps.Length > 0) {
            _prefixSteps = steps
               .Take(steps.Length - 1)
               .ToArray();
            _lastStep = steps.Last();
         } else {
            _prefixSteps = null;
            _lastStep = null;
         }

         _useDefaultValue = useDefaultValue;
         _defaultValue = defaultValue;
      }

      /// <summary>
      ///   Returns true if the property path does not contain any property access
      ///   like in 'p => p'. An empty property path simple returns the source value
      ///   when 'GetValue' is called and throws an expcetion when 'SetValue' is 
      ///   called. This construct is useful when concating paths.
      /// </summary>
      public bool IsEmpty {
         get { return _lastStep == null; }
      }

      /// <summary>
      ///   Gets the value of the property path by sequentially getting the value
      ///   of all property steps. Pass in the object for which the value of the 
      ///   property path should be returned.
      /// </summary>
      public TValue GetValue(TSource source) {
         Contract.Requires<ArgumentNullException>(source != null);

         if (IsEmpty) {
            Contract.Assert(typeof(TSource) == typeof(TValue));
            return (TValue)(object)source;
         }

         object prefixValue = GetPrefixValue(source);

         if (_useDefaultValue && prefixValue == null) {
            return _defaultValue;
         } else {
            Contract.Assert(prefixValue != null);
            return (TValue)_lastStep.Accessor(prefixValue);
         }
      }

      /// <summary>
      ///   Sets the value of the property path. This involves sequentially getting
      ///   the value of all property steps but the last and actually setting the 
      ///   value on the last step. Pass in the object for which the value of the 
      ///   property path should be set.
      /// </summary>
      public void SetValue(TSource source, TValue value) {
         Contract.Requires<ArgumentNullException>(source != null);

         if (IsEmpty) {
            throw new InvalidOperationException(ExceptionTexts.CannotSetValueOfEmptyPropertyPath);
         }

         object prefixValue = GetPrefixValue(source);

         if (_useDefaultValue && prefixValue == null) {
            // Do nothing
         } else {
            Contract.Assert(prefixValue != null);

            if (_lastStep.Mutator == null) {
               throw new InvalidOperationException(
                  ExceptionTexts.PropertyIsReadonly.FormatWith(
                     GetFormattedPropertyPath(source.GetType()),
                     _lastStep.PropertyName
                  )
               );
            }

            _lastStep.Mutator(prefixValue, value);
         }
      }

      /// <summary>
      /// See 'PropertyPath.Concat'.
      /// </summary>
      internal static new PropertyPath<T1, T3> Concat<T1, T2, T3>(
         PropertyPath<T1, T2> first,
         PropertyPath<T2, T3> second
      ) {
         return new PropertyPath<T1, T3>(
            first.GetSteps().Concat(second.GetSteps()).ToArray(),
            second._useDefaultValue,
            second._defaultValue
         );
      }

      /// <summary>
      /// The 'prefix' includes all steps of the property path except the last.
      /// So for '[Person].Address.City.PostalCode' this method would return the
      /// value of 'Address.City'. For single step properties it just returnd
      /// 'source'.
      /// </summary>
      private object GetPrefixValue(object source) {
         Contract.Requires(source != null);

         object stepValue = source;

         foreach (Step step in _prefixSteps) {
            stepValue = step.Accessor(stepValue);

            if (stepValue == null) {
               if (_useDefaultValue) {
                  return null;
               } else {
                  int stepCount = Array.IndexOf(_prefixSteps, step) + 1;
                  IEnumerable<Step> nullSteps = _prefixSteps.Take(stepCount);

                  throw new NullReferenceException(
                     ExceptionTexts.PropertyStepIsNull.FormatWith(
                        GetFormattedPropertyPath(source.GetType()),
                        GetFormattedPropertyPath(source.GetType(), nullSteps),
                        source
                     )
                  );
               }
            }
         }

         return stepValue;
      }

      /// <summary>
      ///   Returns a string like "[Person].Address.City" where [Person] is the
      ///   class on which 'Address' is declared (or a subtype from it).
      /// </summary>
      private string GetFormattedPropertyPath(
         Type sourceObjectType,
         IEnumerable<Step> includeOnlyTheseSteps = null
      ) {
         IEnumerable<Step> steps = includeOnlyTheseSteps ?? GetSteps();

         IEnumerable<string> parts = new string[] { "[" + sourceObjectType.Name + "]" }
            .Concat(steps.Select(s => s.PropertyName));

         return String.Join(".", parts);
      }

      private IEnumerable<Step> GetSteps() {
         return IsEmpty ?
            new Step[] { } :
            _prefixSteps.Concat(new Step[] { _lastStep });
      }
   }

   partial class PropertyPath {
      /// <summary>
      ///   A representation of single step of a 'PropertyPath' like 'Address' or
      ///   'City' in '[Person].Address.City'. Provides two delegates to get and 
      ///   set the property it represents.
      /// </summary>
      /// <remarks>
      ///   This class has to be in the non-generic PropertyPath class to implement
      ///   to 'Conact' function.
      /// </remarks>
      protected class Step {
         private static readonly MethodInfo _createWeakPropertyAccessorMethod = typeof(Step)
            .GetMethod("CreateWeakPropertyAccessor", BindingFlags.NonPublic | BindingFlags.Static);

         private static readonly MethodInfo _createWeakPropertyMutatorMethod = typeof(Step)
            .GetMethod("CreateWeakPropertyMutator", BindingFlags.NonPublic | BindingFlags.Static);

         internal Step(PropertyInfo property) {
            Contract.Requires(property != null);

            PropertyName = property.Name;
            Accessor = CreateAccessor(property);

            if (property.CanWrite) {
               Mutator = CreateMutator(property);
            }
         }

         public string PropertyName { get; set; }

         public Func<object, object> Accessor { get; private set; }

         public Action<object, object> Mutator { get; private set; }

         /// <summary>
         ///   Calls generic 'CreateWeakPropertyAccessor' method with the correct 
         ///   type arguments.
         /// </summary>
         private static Func<object, object> CreateAccessor(PropertyInfo property) {
            MethodInfo createWeakAccessor = _createWeakPropertyAccessorMethod.MakeGenericMethod(
               property.DeclaringType, // TSource
               property.PropertyType   // TValue
            );

            return (Func<object, object>)createWeakAccessor
               .Invoke(null, new object[] { property });
         }

         /// <summary>
         ///   Calls generic 'CreateWeakPropertyMutator' method with the correct 
         ///   type arguments.
         /// </summary>
         private static Action<object, object> CreateMutator(PropertyInfo property) {
            MethodInfo createWeakMutator = _createWeakPropertyMutatorMethod.MakeGenericMethod(
               property.DeclaringType, // TSource
               property.PropertyType   // TValue
            );

            return (Action<object, object>)createWeakMutator
               .Invoke(null, new object[] { property });
         }

         /// <summary>
         ///   Returns a delegate that casts its first argument to 'TObject' and
         ///   returns the value of the passed 'property' on this object. 'TProperty'
         ///   is the type of the property.
         /// </summary>
         /// <remarks>
         ///   This methods creates a delegate that directly calls the getter method 
         ///   of the property (without reflection) which is ways faster than using
         ///   'PropertyInfo.GetValue'. For this to work it is required that we know
         ///   the type of the object and property at compile time because (1) the
         ///   the delegate must be strongly typed and (2) therefore also must be
         ///   called in a strongly typed fashion (Delegate.InvokeInvoke would be
         ///   slow).
         ///   
         ///   Unfortunately we don't know these two types at compile time. To work
         ///   arround this, we call this method using Reflection and fill in the
         ///   two generic type parameters at runtime.
         /// </remarks>
         private static Func<object, object> CreateWeakPropertyAccessor<TObject, TProperty>(
            PropertyInfo property
         ) {
            Func<TObject, TProperty> strongAccessor = (Func<TObject, TProperty>)
               Delegate.CreateDelegate(
                  typeof(Func<TObject, TProperty>),
                  property.GetGetMethod(true)
               );

            return delegate(object target) {
               TObject typedTarget = (TObject)target;
               return strongAccessor(typedTarget);
            };
         }

         /// <summary>
         ///   Returns a delegate that casts its first argument to 'TObject' and its
         ///   second to 'TProperty' and sets the value of the passed 'property' on
         ///   the object in the first argument to the value in the second argument.
         /// </summary>
         /// <remarks>
         ///   See 'CreateWeakPropertyAccessor'.
         /// </remarks>
         private static Action<object, object> CreateWeakPropertyMutator<TObject, TProperty>(
            PropertyInfo property
         ) {
            Action<TObject, TProperty> strongMutator = (Action<TObject, TProperty>)
               Delegate.CreateDelegate(
                  typeof(Action<TObject, TProperty>),
                  property.GetSetMethod(true)
               );

            return delegate(object target, object value) {
               TObject typedTarget = (TObject)target;
               TProperty typedValue = (TProperty)value;
               strongMutator(typedTarget, typedValue);
            };
         }
      }
   }
}