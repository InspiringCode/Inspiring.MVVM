namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ParentSetterBehavior<TValue> :
      Behavior,
      IValueAccessorBehavior<TValue>,
      IValueInitializerBehavior
      where TValue : IViewModel {

      public TValue GetValue(IBehaviorContext context) {
         return this.GetValueNext<TValue>(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         var previousChild = this.GetValueNext<TValue>(context);
         RemoveParentFrom(previousChild, context);
         AddParentTo(value, context);
         this.SetValueNext(context, value);
      }

      public void InitializeValue(IBehaviorContext context) {
         var child = this.GetValueNext<TValue>(context);
         AddParentTo(child, context);
         this.InitializeValueNext(context);
      }

      private static void AddParentTo(TValue child, IBehaviorContext context) {
         if (child != null) {
            var parent = context.VM;
            child.Kernel.Parents.Add(parent);
         }
      }

      private static void RemoveParentFrom(TValue child, IBehaviorContext context) {
         if (child != null) {
            var parent = context.VM;
            child.Kernel.Parents.Remove(parent);
         }
      }
   }
}
