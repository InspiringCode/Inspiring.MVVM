namespace Inspiring.Mvvm.Views.Binder {
   using System;
   using System.Linq.Expressions;
   using System.Windows;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;

   /// <summary>
   /// Binds a property of a <see cref="VMDescriptor"/>.
   /// </summary>
   public class VMPropertyBinder<TDescriptor> : BinderRootExpression, IVMBinder<TDescriptor> {
      private string _pathPrefix;

      public VMPropertyBinder(string pathPrefix = null) {
         _pathPrefix = pathPrefix;
         BinderBuildStepRegistry.AddVMPropertyBuildSteps(this);
      }

      public IBindToExpression<T> Property<T>(
         Expression<Func<TDescriptor, VMProperty<T>>> sourcePropertySelector
      ) {
         string path = ExpressionService.GetPropertyPathString(sourcePropertySelector);

         BinderContext context = QueueBuilderExecution();
         context.ExtendPropertyPath(path);
         context.SourcePropertyType = typeof(T);

         return new PropertyBinderExpression<T>(context);
      }


      public IBindCollectionExpression<TItemDescriptor> Collection<TItemDescriptor>(
         Expression<Func<TDescriptor, IVMCollectionProperty<ViewModel<TItemDescriptor>>>> collectionPropertySelector
      ) where TItemDescriptor : VMDescriptor {
         string path = ExpressionService.GetPropertyPathString(collectionPropertySelector);

         BinderContext context = QueueBuilderExecution();
         context.ExtendPropertyPath(path);

         return new VMCollectionBinder<TItemDescriptor>(context);
      }

      public override BinderContext QueueBuilderExecution() {
         BinderContext context = base.QueueBuilderExecution();
         context.ExtendPropertyPath(_pathPrefix);
         return context;
      }
   }

   public class VMCollectionBinder<TItemDescriptor> : PropertyBinderExpression<VMCollection<TItemDescriptor>>, IBindCollectionExpression<TItemDescriptor>
      where TItemDescriptor : VMDescriptor {

      public VMCollectionBinder(BinderContext context)
         : base(context) {
      }

      public void To(DependencyObject itemsControl, Action<IVMBinder<TItemDescriptor>> itemBinder) {
         To(itemsControl);
         VMPropertyBinder<TItemDescriptor> binder = new VMPropertyBinder<TItemDescriptor>();
         itemBinder(binder);
         binder.Execute();
      }
   }
}
