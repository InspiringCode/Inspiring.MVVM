namespace Inspiring.Mvvm.Views.Binder {
   using System;
   using System.Linq.Expressions;
   using System.Windows;
   using System.Windows.Input;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

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
         Expression<Func<TDescriptor, IVMPropertyDescriptor<T>>> sourcePropertySelector
      ) {
         string path = ExpressionService.GetPropertyPathString(sourcePropertySelector);

         BinderContext context = QueueBuilderExecution();
         context.ExtendPropertyPath(path);
         context.SourcePropertyType = typeof(T);

         // HACK ATTACK: My collegas needed a quick fix!!!!
         if (typeof(T) == typeof(ICommand)) {
            context.Binding.FallbackValue = DelegateCommand.AlwaysDisabled;
         }

         // TODO: Always necessary? Is there a better place?
         context.Binding.ValidatesOnDataErrors = true;

         return new PropertyBinderExpression<T>(context);
      }


      public IBindCollectionExpression<TItemDescriptor> Collection<TItemDescriptor>(
         Expression<Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModel<TItemDescriptor>>>>> collectionPropertySelector
      ) where TItemDescriptor : VMDescriptor {
         string path = ExpressionService.GetPropertyPathString(collectionPropertySelector);

         BinderContext context = QueueBuilderExecution();
         context.ExtendPropertyPath(path);

         return new VMCollectionBinder<TItemDescriptor>(context);
      }

      public void VM<TChildDescriptor>(
         Expression<Func<TDescriptor, IVMPropertyDescriptor<IViewModel<TChildDescriptor>>>> viewModelPropertySelector,
         Action<IVMBinder<TChildDescriptor>> viewModelBinder
      ) where TChildDescriptor : VMDescriptor {
         string path = ExpressionService.GetPropertyPathString(viewModelPropertySelector);
         var binder = new VMPropertyBinder<TChildDescriptor>(pathPrefix: path);
         viewModelBinder(binder);
         binder.Execute();
      }

      public override BinderContext QueueBuilderExecution() {
         BinderContext context = base.QueueBuilderExecution();
         context.ExtendPropertyPath(_pathPrefix);
         return context;
      }


      public IBindCollectionExpression<TItemDescriptor> CollectionManual<TItemDescriptor>(
         Expression<Func<TDescriptor, IVMPropertyDescriptor<object>>> collectionPropertySelector
      ) where TItemDescriptor : VMDescriptor {
         string path = ExpressionService.GetPropertyPathString(collectionPropertySelector);

         BinderContext context = QueueBuilderExecution();
         context.ExtendPropertyPath(path);

         return new VMCollectionBinder<TItemDescriptor>(context);
      }
   }

   public class VMCollectionBinder<TItemDescriptor> : PropertyBinderExpression<VMCollection<IViewModel<TItemDescriptor>>>, IBindCollectionExpression<TItemDescriptor>
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
