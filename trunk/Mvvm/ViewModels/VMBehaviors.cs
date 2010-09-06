namespace Inspiring.Mvvm.ViewModels {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels.Behaviors;

   /// <summary>
   ///   A class that holds all available <see cref="VMBehavior"/>s that can be
   ///   enabled on a <see cref="VMDescriptor"/> instance. Use the 'WithBehaviors'
   ///   method of the <see cref="VMDescriptorBuilder"/> fluent interface.
   /// </summary>
   public static class VMBehaviors {
      public static readonly VMBehaviorFactory DisconnectedViewModel = new DisconnectedViewModelFactory();

      internal static readonly VMBehaviorFactory Validation = new ValidationFactory();
      internal static readonly VMBehaviorFactory DisplayValueValidation = new DisplayValueValidationFactory();
      private static List<Type> _behaviorStack = new List<Type>();

      static VMBehaviors() {
         RegisterAfterLast(typeof(AllowInvalidDisplayValuesBehavior));
         RegisterAfterLast(typeof(DisplayValueValidationBehavior));
         RegisterAfterLast(typeof(DisplayValueAccessorBehavior<>));

         RegisterAfterLast(typeof(ValidationBehavior<>));
         RegisterAfterLast(typeof(DisconnectedVMBehavior<>));
         RegisterAfterLast(typeof(PropertyChangedBehavior<>));

         RegisterAfterLast(typeof(CalculatedPropertyBehavior<,>));
         RegisterAfterLast(typeof(MappedPropertyBehavior<,>));
         RegisterAfterLast(typeof(InstancePropertyBehavior<>));
      }

      /// <summary>
      ///   Determines the order of enabled behaviors. Behaviors that have lower 
      ///   indices are nearer to the caller of the view model and farer from the
      ///   value source of the VM property.
      /// </summary>
      internal static List<Type> BehaviorStack {
         get { return _behaviorStack; }
      }


      public static void RegisterBefore(Type before, Type behavior) {
         CheckBehaviorType(before);
         CheckBehaviorType(behavior);

         int index = GetIndexOf(behavior);
         _behaviorStack.Insert(index, behavior);
      }

      public static void RegisterAfter(Type after, Type behavior) {
         CheckBehaviorType(after);
         CheckBehaviorType(behavior);

         int index = GetIndexOf(behavior);
         _behaviorStack.Insert(index + 1, behavior);
      }

      public static void RegisterBeforeFirst(Type behavior) {
         CheckBehaviorType(behavior);
         _behaviorStack.Insert(0, behavior);

      }

      public static void RegisterAfterLast(Type behavior) {
         CheckBehaviorType(behavior);
         _behaviorStack.Add(behavior);
      }

      private static void CheckBehaviorType(Type type) {
         if (!typeof(IBehavior).IsAssignableFrom(type) ||
             !type.IsClass ||
             type.IsAbstract) {
            throw new ArgumentException(ExceptionTexts.TypeIsNoBehavior);
         }

         if (type.IsGenericType && !type.IsGenericTypeDefinition) {
            throw new ArgumentException(ExceptionTexts.CannotPassClosedTypeBehavior);
         }
      }

      private static int GetIndexOf(Type behavior) {
         int index = _behaviorStack.IndexOf(behavior);

         if (index == -1) {
            throw new ArgumentException(
               ExceptionTexts.BehaviorNotRegistered.FormatWith(behavior.Name)
            );
         }

         return index;
      }

      private class DisconnectedViewModelFactory : VMBehaviorFactory {
         public override Type BehaviorType {
            get { return typeof(DisconnectedVMBehavior<>); }
         }

         public override IBehavior Create<TValue>() {
            return new DisconnectedVMBehavior<TValue>();
         }
      }

      private class ValidationFactory : VMBehaviorFactory {
         public override Type BehaviorType {
            get { return typeof(ValidationBehavior<>); }
         }

         public override IBehavior Create<TValue>() {
            return new ValidationBehavior<TValue>();
         }
      }

      private class DisplayValueValidationFactory : VMBehaviorFactory {
         public override Type BehaviorType {
            get { return typeof(DisplayValueValidationBehavior); }
         }

         public override IBehavior Create<TValue>() {
            return new DisplayValueValidationBehavior();
         }
      }
   }
}
