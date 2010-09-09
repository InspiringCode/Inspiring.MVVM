namespace Inspiring.Mvvm.ViewModels.Behaviors {
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.Common;

   /// <summary>
   ///   A <see cref="IAccessPropertyBehavior"/> that implements the get/set 
   ///   operation of a <see cref="VMProperty"/> by using the target of a CLR
   ///   property defined on a target object of the view model.
   /// </summary>
   internal sealed class MappedPropertyBehavior<TVM, TValue> : VMPropertyBehavior, IAccessPropertyBehavior<TValue>
      where TVM : IBehaviorContext {

      private PropertyPath<TVM, TValue> _path;

      public override BehaviorPosition Position {
         get { return BehaviorPosition.SourceValueAccessor; }
      }

      public MappedPropertyBehavior(PropertyPath<TVM, TValue> path) {
         Contract.Requires(path != null);
         _path = path;
      }

      public TValue GetValue(IBehaviorContext vm) {
         return _path.GetValue((TVM)vm);
      }

      public void SetValue(IBehaviorContext vm, TValue value) {
         _path.SetValue((TVM)vm, value);
      }
   }
}
