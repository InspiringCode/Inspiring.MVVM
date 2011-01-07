namespace Inspiring.Mvvm.Views {
   using System;
   using System.ComponentModel;
   using System.Linq.Expressions;
   using System.Windows;
   using System.Windows.Data;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.Views.Binder;

   public class ViewBinder {
      public static void BindVM<TDescriptor>(
         IBindableView<ViewModel<TDescriptor>> view,
         Action<IVMBinder<TDescriptor>> bindingConfigurator
      ) where TDescriptor : VMDescriptor {
         if (DesignerProperties.GetIsInDesignMode((DependencyObject)view)) {
            return;
         }

         VMPropertyBinder<TDescriptor> binder = new VMPropertyBinder<TDescriptor>();
         bindingConfigurator(binder);
         binder.Execute();
      }

      public static void BindScreen<TScreen>(
         IView<TScreen> screen,
         Action<IScreenBinder<TScreen>> bindingConfigurator
      ) where TScreen : IScreen {
         if (DesignerProperties.GetIsInDesignMode((DependencyObject)screen)) {
            return;
         }

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

      IBindToExpression<IViewModel> BindVM(
         Expression<Func<TScreen, IViewModel>> viewModelSelector
      );

      IBindToExpression<IScreen> BindChildScreen(Expression<Func<TScreen, IScreen>> screenSelector);
   }

   public interface IVMBinder<TDescriptor> {
      IBindToExpression<T> Property<T>(Expression<Func<TDescriptor, IVMProperty<T>>> sourcePropertySelector);

      IBindCollectionExpression<TItemDescriptor> Collection<TItemDescriptor>(
         Expression<Func<TDescriptor, IVMProperty<IVMCollectionExpression<ViewModel<TItemDescriptor>>>>> collectionPropertySelector
      ) where TItemDescriptor : VMDescriptor;

      void VM<TChildDescriptor>(
         Expression<Func<TDescriptor, IVMProperty<ViewModel<TChildDescriptor>>>> viewModelPropertySelector,
         Action<IVMBinder<TChildDescriptor>> viewModelBinder
      ) where TChildDescriptor : VMDescriptor;
   }

   public interface IBindToExpression<T> {
      IOptionsExpression<T> To(DependencyObject target);
   }

   public interface IOptionsExpression<T> {
      IOptionsExpression<T> On(DependencyProperty property);
      IOptionsExpression<T> TwoWay();
      IOptionsExpression<T> OneWay();
      IOptionsExpression<T> OneWayToSource();
      IOptionsExpression<T> OneTime();
      IOptionsExpression<T> PropertyChanged();
      IOptionsExpression<T> With(IValueConverter converter, object parameter = null);
      IOptionsExpression<T> FallbackValue(object value);
   }

   public interface IBindCollectionExpression<TDescriptor> {
      void To(DependencyObject itemsControl, Action<IVMBinder<TDescriptor>> itemBinder);
   }

   public class ScreenBinder<TScreen> : BinderRootExpression, IScreenBinder<TScreen> where TScreen : IScreen {
      public ScreenBinder() {
         // TODO: Does this config apply to all queued executions?
         BinderBuildStepRegistry.AddVMPropertyBuildSteps(this);
      }

      public void BindVM<TDescriptor>(Expression<Func<TScreen, ViewModel<TDescriptor>>> viewModelSelector, Action<IVMBinder<TDescriptor>> bindingConfigurator) where TDescriptor : VMDescriptor {
         string pathPrefix = ExpressionService.GetPropertyPathString(viewModelSelector);
         VMPropertyBinder<TDescriptor> binder = new VMPropertyBinder<TDescriptor>(pathPrefix);
         bindingConfigurator(binder);
         binder.Execute();
      }

      public IBindToExpression<IScreen> BindChildScreen(Expression<Func<TScreen, IScreen>> screenSelector) {
         BinderContext context = QueueBuilderExecution();
         context.SourcePropertyType = typeof(IScreen); // TODO: Clean up how types are assigned, also check if IBindToExpression has to be generic...
         context.ExtendPropertyPath(
            ExpressionService.GetPropertyPathString(screenSelector)
         );

         return new PropertyBinderExpression<IScreen>(context);
      }

      public IBindToExpression<IViewModel> BindVM(Expression<Func<TScreen, IViewModel>> viewModelSelector) {
         BinderContext context = QueueBuilderExecution();
         context.SourcePropertyType = typeof(IViewModel); // TODO: Clean up how types are assigned, also check if IBindToExpression has to be generic...
         context.ExtendPropertyPath(
            ExpressionService.GetPropertyPathString(viewModelSelector)
         );

         return new PropertyBinderExpression<IViewModel>(context);
      }
   }



   public class PropertyBinderExpression<T> : BinderExpression, IBindToExpression<T>, IOptionsExpression<T> {
      public PropertyBinderExpression(BinderContext context)
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


      public IOptionsExpression<T> TwoWay() {
         BinderExpression.ExposeContext(this, c => {
            c.Binding.Mode = BindingMode.TwoWay;
         });

         return this;
      }

      public IOptionsExpression<T> OneWay() {
         BinderExpression.ExposeContext(this, c => {
            c.Binding.Mode = BindingMode.OneWay;
         });

         return this;
      }

      public IOptionsExpression<T> OneWayToSource() {
         BinderExpression.ExposeContext(this, c => {
            c.Binding.Mode = BindingMode.OneWayToSource;
         });

         return this;
      }


      public IOptionsExpression<T> OneTime() {
         BinderExpression.ExposeContext(this, c => {
            c.Binding.Mode = BindingMode.OneTime;
         });

         return this;
      }

      public IOptionsExpression<T> PropertyChanged() {
         BinderExpression.ExposeContext(this, c => {
            c.Binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
         });

         return this;
      }

      public IOptionsExpression<T> With(IValueConverter converter, object parameter = null) {
         BinderExpression.ExposeContext(this, c => {
            c.Binding.Converter = converter;
            c.Binding.ConverterParameter = parameter;
         });

         return this;
      }


      public IOptionsExpression<T> FallbackValue(object value) {
         BinderExpression.ExposeContext(this, c => {
            c.Binding.FallbackValue = value;
         });

         return this;
      }
   }
}
