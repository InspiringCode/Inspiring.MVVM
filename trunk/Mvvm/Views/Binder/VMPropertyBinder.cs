namespace Inspiring.Mvvm.Views.Binder {
   using System;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;

   /// <summary>
   /// Binds a property of a <see cref="VMDescriptor"/>.
   /// </summary>
   public class VMPropertyBinder<TDescriptor> : BinderRootExpression, IVMBinder<TDescriptor> {
      private string _pathPrefix;

      public VMPropertyBinder(string pathPrefix = null) {
         _pathPrefix = pathPrefix;
         InsertBuildStep(new SetBindingBuildStep());
         InsertBuildStep(new SetDefaultPropertyBuildStep());
      }

      public IBindToExpression<T> Property<T>(
         Expression<Func<TDescriptor, T>> sourcePropertySelector
      ) {
         string path = ExpressionService.GetPropertyPathString(sourcePropertySelector);

         BinderContext context = QueueBuilderExecution();
         context.ExtendPropertyPath(path);

         return new PropertyBinderExpression<T>(context);
      }

      public override BinderContext QueueBuilderExecution() {
         BinderContext context = base.QueueBuilderExecution();
         context.ExtendPropertyPath(_pathPrefix);
         return context;
      }
   }
}
