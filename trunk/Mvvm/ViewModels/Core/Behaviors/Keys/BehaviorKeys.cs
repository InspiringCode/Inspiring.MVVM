﻿namespace Inspiring.Mvvm.ViewModels.Core {

   public static class BehaviorKeys {

      public static readonly BehaviorKey TypeDescriptor = new BehaviorKey("TypeDescriptor");

      public static readonly BehaviorKey InvalidDisplayValueCache = new BehaviorKey("InvalidDisplayValueCache");
      public static readonly BehaviorKey DisplayValueAccessor = new BehaviorKey("DisplayValueAccessor");
      public static readonly BehaviorKey Validator = new BehaviorKey("Validator");
      public static readonly BehaviorKey PropertyChangedTrigger = new BehaviorKey("PropertyChangedTrigger");
      public static readonly BehaviorKey PreValidationValueCache = new BehaviorKey("PropertyChangedTrigger");

      public static readonly BehaviorKey PropertyValueCache = new BehaviorKey("PropertyValueCache");
      public static readonly BehaviorKey ManualUpdateBehavior = new BehaviorKey("ManualUpdateBehavior");

      /// <summary>
      ///   A <see cref="IValueAccessorBehavior"/> gets and sets the actual 
      ///   property value. The generic type argument is the type of the property
      ///   value.
      /// </summary>
      public static readonly BehaviorKey PropertyValueAcessor = new BehaviorKey("PropertyValueAcessor");

      /// <summary>
      ///   
      /// </summary>
      public static readonly BehaviorKey SourceValueAccessor = new BehaviorKey("SourceValueAccessor");

   }
}