﻿namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ManualUpdatePropertyBehavior :
      InitializableBehavior,
      IBehaviorInitializationBehavior,
      IManualUpdateBehavior {

      private IVMPropertyDescriptor _property;

      public void Initialize(BehaviorInitializationContext context) {
         _property = context.Property;
         this.InitializeNext(context);
         SetInitialized();
      }

      public void UpdatePropertyFromSource(IBehaviorContext context) {
         RequireInitialized();

         this.UpdatePropertyFromSourceNext(context);

         ValidationContext.BeginValidation(); // TODO: Is there a nicer solution?
         this.RevalidateNext(context, ValidationContext.Current, ValidationMode.DiscardInvalidValues);
         ValidationContext.CompleteValidation(ValidationMode.DiscardInvalidValues);

         //var validationBehavior = GetNextBehavior<IRevalidationBehavior>();
         //validationBehavior.Revalidate(context, ValidationMode.DiscardInvalidValues);

         context.NotifyChange(
            new ChangeArgs(
               ChangeType.PropertyChanged,
               context.VM,
               _property
            )
         );
      }

      public void UpdatePropertySource(IBehaviorContext context) {
         RequireInitialized();
         this.UpdatePropertySourceNext(context);
      }
   }
}