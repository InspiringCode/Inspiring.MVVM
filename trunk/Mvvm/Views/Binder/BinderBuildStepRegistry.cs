namespace Inspiring.Mvvm.Views.Binder {
   using System.Collections.Generic;

   internal sealed class BinderBuildStepRegistry {
      private static List<IBinderBuildStep> _propertySteps = new List<IBinderBuildStep>();
      private static List<IBinderBuildStep> _collectionSteps = new List<IBinderBuildStep>();

      static BinderBuildStepRegistry() {
         _propertySteps.Add(new SetBindingBuildStep());
         _propertySteps.Add(new DefaultPropertyBuildStep());
         _propertySteps.Add(new DataGridBinderBuildStep());
      }

      public static void RegisterPropertyBuildStep(IBinderBuildStep step) {
         _propertySteps.Add(step);
      }

      public static void RegisterCollectionBuildStep(IBinderBuildStep step) {
         _collectionSteps.Add(step);
      }

      internal static void AddVMPropertyBuildSteps(BinderRootExpression propertyBinder) {
         _propertySteps.ForEach(propertyBinder.InsertBuildStep);
      }

      internal static void AddVMCollectionBuildSteps(BinderRootExpression collectionBinder) {
         _collectionSteps.ForEach(collectionBinder.InsertBuildStep);
      }
   }
}
