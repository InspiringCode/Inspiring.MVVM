//namespace Inspiring.Mvvm.ViewModels.Core {
//   using System;

//   internal sealed class CollectionPopulatorBehavior<TItemVM> :
//      Behavior,
//      IBehaviorInitializationBehavior,
//      IValueAccessorBehavior<IVMCollection<TItemVM>>,
//      IPopulationBehavior,
//      IMutabilityCheckerBehavior
//      where TItemVM : IViewModel {

//      private static readonly FieldDefinitionGroup IsPopulatedGroup = new FieldDefinitionGroup();

//      private FieldDefinition<bool> _isPopulatedField;

//      public void Initialize(BehaviorInitializationContext context) {
//         _isPopulatedField = context.Fields.DefineField<bool>(IsPopulatedGroup);
//         this.InitializeNext(context);
//      }

//      public IVMCollection<TItemVM> GetValue(IBehaviorContext context) {
//         var coll = this.GetValueNext<IVMCollection<TItemVM>>(context);

//         if (!context.FieldValues.GetValueOrDefault(_isPopulatedField)) {
//            context.FieldValues.SetValue(_isPopulatedField, true);
//            Repopulate(context, coll);
//         }

//         return coll;
//      }

//      public void SetValue(IBehaviorContext vm, IVMCollection<TItemVM> value) {
//         throw new NotSupportedException(
//            ExceptionTexts.CannotSetVMCollectionProperties
//         );
//      }

//      public bool IsMutable(IBehaviorContext vm) {
//         return false;
//      }

//      public void Populate(IBehaviorContext context) {
//         context.FieldValues.SetValue(_isPopulatedField, true);

//         var coll = this.GetValueNext<IVMCollection<TItemVM>>(context);
//         Repopulate(context, coll);
//      }

//      private void Repopulate(IBehaviorContext context, IVMCollection<TItemVM> collection) {
//         var behavior = collection
//            .Behaviors
//            .GetNextBehavior<IPopulatorCollectionBehavior<TItemVM>>();

//         behavior.Repopulate(context, collection);

//         this.PopulateNext(context);
//      }
//   }
//}
