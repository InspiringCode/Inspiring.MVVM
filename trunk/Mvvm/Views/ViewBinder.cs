﻿namespace Inspiring.Mvvm.Views {
   using System;
   using System.ComponentModel;
   using System.Linq.Expressions;
   using System.Windows;
   using System.Windows.Data;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.Screens;
   using Inspiring.Mvvm.ViewModels;
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

      IBindToExpression<ViewModel> BindVM(
         Expression<Func<TScreen, ViewModel>> viewModelSelector
      );

      IBindToExpression<IScreen> BindChildScreen(Expression<Func<TScreen, IScreen>> screenSelector);
   }

   public interface IVMBinder<TDescriptor> {
      IBindToExpression<T> Property<T>(Expression<Func<TDescriptor, VMProperty<T>>> sourcePropertySelector);
      IBindCollectionExpression<TItemDescriptor> Collection<TItemDescriptor>(
         Expression<Func<TDescriptor, IVMCollectionProperty<ViewModel<TItemDescriptor>>>> collectionPropertySelector
      ) where TItemDescriptor : VMDescriptor;
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
         PropertyBinderExpression<IScreen> exp = new PropertyBinderExpression<IScreen>(
            context: QueueBuilderExecution()
         );

         BinderExpression.ExposeContext(this, c => {
            string pathPostfix = ExpressionService.GetPropertyPathString(screenSelector);
            c.ExtendPropertyPath(pathPostfix);
         });

         return exp;
      }

      public IBindToExpression<ViewModel> BindVM(Expression<Func<TScreen, ViewModel>> viewModelSelector) {
         BinderContext context = QueueBuilderExecution();
         context.SourcePropertyType = typeof(ViewModel); // TODO: Clean up how types are assigned, also check if IBindToExpression has to be generic...
         context.ExtendPropertyPath(
            ExpressionService.GetPropertyPathString(viewModelSelector)
         );

         return new PropertyBinderExpression<ViewModel>(context);
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
   }
}
