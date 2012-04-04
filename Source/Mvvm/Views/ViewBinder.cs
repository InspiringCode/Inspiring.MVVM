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
         IBindableView<IViewModelExpression<TDescriptor>> view,
         Action<IVMBinder<TDescriptor>> bindingConfigurator
      ) where TDescriptor : IVMDescriptor {
         if (DesignerProperties.GetIsInDesignMode((DependencyObject)view)) {
            return;
         }

         VMPropertyBinder<TDescriptor> binder = new VMPropertyBinder<TDescriptor>();
         bindingConfigurator(binder);
         binder.Execute();
      }

      public static void BindVM<TDescriptor>(
         Action<IVMBinder<TDescriptor>> bindingConfigurator
      ) where TDescriptor : IVMDescriptor {
         // See http://sweux.com/blogs/laranjeira/index.php/wpf/design-mode-wpf/how-to-get-design-mode-property-in-wpf/
         if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
            return;
         }

         VMPropertyBinder<TDescriptor> binder = new VMPropertyBinder<TDescriptor>();
         bindingConfigurator(binder);
         binder.Execute();
      }

      public static void BindScreen<TScreen>(
         IView<TScreen> screen,
         Action<IScreenBinder<TScreen>> bindingConfigurator
      ) where TScreen : IScreenBase {
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
         Expression<Func<TScreen, IViewModelExpression<TDescriptor>>> viewModelSelector,
         Action<IVMBinder<TDescriptor>> bindingConfigurator
      ) where TDescriptor : VMDescriptor;

      IBindToExpression<IViewModel> BindVM(
         Expression<Func<TScreen, IViewModel>> viewModelSelector
      );

      IBindToExpression<IScreenBase> BindChildScreen(Expression<Func<TScreen, IScreenBase>> screenSelector);
   }

   public interface IVMBinder<TDescriptor> {
      /// <summary>
      ///   This is a personal and temporar HACK (!!!) for Stefan! Do NOT USE this!
      /// </summary>
      string PathPrefix { get; }

      IBindToExpression<T> Property<T>(Expression<Func<TDescriptor, IVMPropertyDescriptor<T>>> sourcePropertySelector);

      IBindCollectionExpression<TItemDescriptor> Collection<TItemDescriptor>(
         Expression<Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<TItemDescriptor>>>>> collectionPropertySelector
      ) where TItemDescriptor : VMDescriptor;

      IBindCollectionExpression<TItemDescriptor> CollectionManual<TItemDescriptor>(
         Expression<Func<TDescriptor, IVMPropertyDescriptor<object>>> collectionPropertySelector
      ) where TItemDescriptor : VMDescriptor;

      void VM<TChildDescriptor>(
         Expression<Func<TDescriptor, IVMPropertyDescriptor<IViewModelExpression<TChildDescriptor>>>> viewModelPropertySelector,
         Action<IVMBinder<TChildDescriptor>> viewModelBinder
      ) where TChildDescriptor : VMDescriptor;
   }

   public interface IVMCollectionPropertyBinder<TDescriptor> : IVMBinder<TDescriptor> {
      DependencyObject BoundControl { get; }
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
      IOptionsExpression<T> NullValue(object value);
      IOptionsExpression<T> StringFormat(string format);
   }

   public interface IBindCollectionExpression<TDescriptor> {
      void To(DependencyObject itemsControl, Action<IVMCollectionPropertyBinder<TDescriptor>> itemBinder);
   }

   public class ScreenBinder<TScreen> : BinderRootExpression, IScreenBinder<TScreen> where TScreen : IScreenBase {
      public ScreenBinder() {
         // TODO: Does this config apply to all queued executions?
         BinderBuildStepRegistry.AddVMPropertyBuildSteps(this);
      }

      public void BindVM<TDescriptor>(Expression<Func<TScreen, IViewModelExpression<TDescriptor>>> viewModelSelector, Action<IVMBinder<TDescriptor>> bindingConfigurator) where TDescriptor : VMDescriptor {
         string pathPrefix = ExpressionService.GetPropertyPathString(viewModelSelector);
         VMPropertyBinder<TDescriptor> binder = new VMPropertyBinder<TDescriptor>(pathPrefix);
         bindingConfigurator(binder);
         binder.Execute();
      }

      public IBindToExpression<IScreenBase> BindChildScreen(Expression<Func<TScreen, IScreenBase>> screenSelector) {
         BinderContext context = QueueBuilderExecution();
         context.SourcePropertyType = typeof(IScreenBase); // TODO: Clean up how types are assigned, also check if IBindToExpression has to be generic...
         context.ExtendPropertyPath(
            ExpressionService.GetPropertyPathString(screenSelector)
         );

         return new PropertyBinderExpression<IScreenBase>(context);
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
         ConfigureBinding<Binding>(x => x.Mode = BindingMode.TwoWay);
         return this;
      }

      public IOptionsExpression<T> OneWay() {
         ConfigureBinding<Binding>(x => x.Mode = BindingMode.OneWay);
         return this;
      }

      public IOptionsExpression<T> OneWayToSource() {
         ConfigureBinding<Binding>(x => x.Mode = BindingMode.OneWayToSource);
         return this;
      }

      public IOptionsExpression<T> OneTime() {
         ConfigureBinding<Binding>(x => x.Mode = BindingMode.OneTime);
         return this;
      }

      public IOptionsExpression<T> PropertyChanged() {
         ConfigureBinding<Binding>(x => x.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged);
         return this;
      }

      public IOptionsExpression<T> With(IValueConverter converter, object parameter = null) {
         ConfigureBinding<Binding>(x => {
            x.Converter = converter;
            x.ConverterParameter = parameter;
         });

         return this;
      }

      public IOptionsExpression<T> StringFormat(string format) {
         ConfigureBinding<BindingBase>(x => x.StringFormat = format);
         return this;
      }

      public IOptionsExpression<T> FallbackValue(object value) {
         ConfigureBinding<BindingBase>(x => x.FallbackValue = value);
         return this;
      }

      public IOptionsExpression<T> NullValue(object value) {
         ConfigureBinding<Binding>(x => x.TargetNullValue = value);
         return this;
      }

      private void ConfigureBinding<TBinding>(
         Action<TBinding> configurationAction
      ) where TBinding : BindingBase {
         BinderExpression.ExposeContext(this, c => {
            TBinding casted = c.Binding as TBinding;
            if (casted == null) {
               throw new InvalidOperationException(ExceptionTexts.OperationInvalidForBindingType);
            }

            configurationAction(casted);
         });
      }
   }
}
