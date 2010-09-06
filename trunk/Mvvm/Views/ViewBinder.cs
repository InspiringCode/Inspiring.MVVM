namespace Inspiring.Mvvm.Views {
   using System;
   using System.Linq.Expressions;
   using System.Windows;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.Views.Binder;

   public class ViewBinder {
      public static void BindVM<TDescriptor>(
         IBindableView<ViewModel<TDescriptor>> view,
         Action<IVMBinder<TDescriptor>> bindingConfigurator
      ) where TDescriptor : VMDescriptor {
         VMBinder<TDescriptor> binder = new VMBinder<TDescriptor>();
         bindingConfigurator(binder);
         binder.Execute();
      }

      public static void BindScreen<TScreen>(
         IView<TScreen> screen,
         Action<IScreenBinder<TScreen>> bindingConfigurator
      ) where TScreen : IScreen {
         ScreenBinder<TScreen> binder = new ScreenBinder<TScreen>();
         bindingConfigurator(binder);
         binder.Execute();
      }
   }

   public interface IScreenBinder<TScreen> {
      void BindVM<TDescriptor>(
         Expression<Func<TScreen, ViewModel<TDescriptor>>> viewModelSelector,
         Action<IVMBinder<TDescriptor>> bindingConfigurator
      ) where TDescriptor : VMDescriptor;

      IBindToExpression<IScreen> BindChildScreen(Expression<Func<TScreen, IScreen>> screenSelector);
   }

   public interface IVMBinder<TDescriptor> {
      IBindToExpression<T> Property<T>(Expression<Func<TDescriptor, T>> sourcePropertySelector);
   }

   public interface IBindToExpression<T> {
      IOptionsExpression<T> To(DependencyObject target);
   }

   public interface IOptionsExpression<T> {
      IOptionsExpression<T> On(DependencyProperty property);
   }

   public class ScreenBinder<TScreen> : BinderRootExpression, IScreenBinder<TScreen> where TScreen : IScreen {

      public void BindVM<TDescriptor>(Expression<Func<TScreen, ViewModel<TDescriptor>>> viewModelSelector, Action<IVMBinder<TDescriptor>> bindingConfigurator) where TDescriptor : VMDescriptor {
         string pathPrefix = ExpressionService.GetPropertyPathString(viewModelSelector);
         VMBinder<TDescriptor> binder = new VMBinder<TDescriptor>(pathPrefix);
         bindingConfigurator(binder);
         binder.Execute();
      }

      public IBindToExpression<IScreen> BindChildScreen(Expression<Func<TScreen, IScreen>> screenSelector) {
         VMBinderExpression<IScreen> exp = new VMBinderExpression<IScreen>(
            context: QueueBuilderExecution()
         );

         BinderExpression.ExposeContext(this, c => {
            string pathPostfix = ExpressionService.GetPropertyPathString(screenSelector);
            c.ExtendPropertyPath(pathPostfix);
         });

         return exp;
      }
   }

   public class VMBinder<TDescriptor> : BinderRootExpression, IVMBinder<TDescriptor> {
      private string _pathPrefix;

      public VMBinder(string pathPrefix = null) {
         _pathPrefix = pathPrefix;
         InsertBuildStep(new SetBindingBuildStep());
         InsertBuildStep(new SetDefaultPropertyBuildStep());
      }

      public IBindToExpression<T> Property<T>(
         Expression<Func<TDescriptor, T>> sourcePropertySelector
      ) {
         VMBinderExpression<T> exp = new VMBinderExpression<T>(
            context: QueueBuilderExecution()
         );

         BinderExpression.ExposeContext(this, c => {
            string pathPostfix = ExpressionService.GetPropertyPathString(sourcePropertySelector);
            c.ExtendPropertyPath(pathPostfix);
         });

         return exp;
      }

      public override BinderContext QueueBuilderExecution() {
         BinderContext context = base.QueueBuilderExecution();
         context.ExtendPropertyPath(_pathPrefix);
         return context;
      }
   }

   public class VMBinderExpression<T> : BinderExpression, IBindToExpression<T>, IOptionsExpression<T> {
      public VMBinderExpression(BinderContext context)
         : base(context) {
      }

      public IOptionsExpression<T> To(DependencyObject target) {
         BinderExpression.ExposeContext(this, c => {
            c.TargetObject = target;
         });

         return this;
      }

      public IOptionsExpression<T> On(DependencyProperty property) {
         BinderExpression.ExposeContext(this, c => {
            c.TargetProperty = property;
         });

         return this;
      }
   }
}
