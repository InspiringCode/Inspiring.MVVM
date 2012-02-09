namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class DependencyTargetBuilder<TSourceDescriptor> :
      PathDefinitionBuilder<TSourceDescriptor>,
      IRefreshTargetBuilder<TSourceDescriptor>
      where TSourceDescriptor : IVMDescriptor {

      private readonly DependencyBuilderOperation _context;

      public DependencyTargetBuilder(DependencyBuilderOperation context)
         : base(context.TargetProperties) {
         _context = context;
      }

      public IRefreshTargetBuilder<TSourceDescriptor> AndExecuteRefreshDependencies {
         get {
            _context.ExecuteRefreshDependencies = true;
            return this;
         }
      }

      public void Self() {
         // No-op
      }
   }
}