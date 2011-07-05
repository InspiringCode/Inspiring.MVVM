namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Collections.Generic;

   // TODO: Make interface for this?
   internal sealed class PropertyPreloaderBehavior<TValue> :
      Behavior,
      IValueAccessorBehavior<TValue> {

      public PropertyPreloaderBehavior() {
         PreloadedProperties = new List<IVMPropertyDescriptor>();
      }

      public List<IVMPropertyDescriptor> PreloadedProperties { get; private set; }

      public TValue GetValue(IBehaviorContext context) {
         PreloadProperties(context);
         return this.GetValueNext<TValue>(context);
      }

      public void SetValue(IBehaviorContext context, TValue value) {
         PreloadProperties(context);
         this.SetValueNext(context, value);
      }

      private void PreloadProperties(IBehaviorContext context) {
         foreach (IVMPropertyDescriptor p in PreloadedProperties) {
            var kernel = context.VM.Kernel;
            if (!kernel.IsLoaded(p)) {
               kernel.Load(p);
            }
         }
      }
   }
}
