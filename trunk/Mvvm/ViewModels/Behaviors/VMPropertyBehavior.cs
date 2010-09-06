namespace Inspiring.Mvvm.ViewModels.Behaviors {
   using System;
   using System.Diagnostics.Contracts;

   public abstract class VMPropertyBehavior : IBehavior {
      private bool _initialized = false;

      public IBehavior Successor { get; set; }

      /// <summary>
      ///   Gets the next behavior in the stack and throws an 'ArgumentException'
      ///   if no behavior that implements 'TBehavior' can be found.
      /// </summary>
      public TBehavior GetNextBehavior<TBehavior>() {
         Contract.Ensures(Contract.Result<TBehavior>() != null);

         TBehavior result;
         if (!TryGetBehavior<TBehavior>(out result)) {
            throw new ArgumentException(
               ExceptionTexts.BehaviorNotFound.FormatWith(typeof(TBehavior).Name)
            );
         }
         return result;
      }

      /// <summary>
      ///   Tries to get the next behavior in the stack and returns whether a
      ///   behavior that implements 'TBehavior' was found.
      /// </summary>
      public bool TryGetBehavior<TBehavior>(out TBehavior result) {
         Contract.Ensures(
            !Object.Equals(Contract.ValueAtReturn<TBehavior>(out result), null) ||
            !Contract.Result<bool>()
         );

         IBehavior behavior;
         if (!TryFindBehavior<TBehavior>(out behavior)) {
            result = default(TBehavior);
            return false;
         }

         result = (TBehavior)(object)behavior;
         return true;
      }

      internal void Initialize(FieldDefinitionCollection fields, string propertyName) {
         OnDefineDynamicFields(fields);
         OnInitialize(propertyName);
         _initialized = true;
      }

      /// <summary>
      ///   Override this method if your behavior requires dynamic fields that 
      ///   whose values are stored with each <see cref="ViewModel"/> instance.
      ///   The value of fields defined in this way can be accessed via the 
      ///   <see cref="IBehaviorContext.FieldValues"/> property.
      /// </summary>
      protected virtual void OnDefineDynamicFields(FieldDefinitionCollection fields) {
      }

      protected virtual void OnInitialize(string propertyName) {
      }

      protected void AssertInitialized() {
         Contract.Assert(
            _initialized,
            "This behavior was not initialized correctly. Make sure 'DefineDynamicFields(...)' " +
            "was called right after instantiation."
         );
      }

      private bool TryFindBehavior<TBehavior>(out IBehavior result) {
         for (IBehavior b = this.Successor; b != null; b = b.Successor) {
            if (b is TBehavior) {
               result = b;
               return true;
            }
         }

         result = null;
         return false;
      }
   }
}
