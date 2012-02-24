namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;

   public class ScreenBase : IScreenBase {
      public ScreenBase(EventAggregator aggregator) {
         Children = new ScreenChildrenCollection<object>(this);

      }

      public IScreenBase Parent { get; set; }

      public ScreenChildrenCollection<object> Children { get; private set; }

      ICollection<object> IScreenBase.Children {
         get { return Children; }
      }

      protected ScreenLifecycle Lifecycle { get; private set; }
   }

   public class ViewModelScreenBase<TDescriptor> :
      ViewModel<TDescriptor>,
      IViewModelScreenBase
      where TDescriptor : IVMDescriptor {

      public ViewModelScreenBase(IServiceLocator serviceLocator = null)
         : base(serviceLocator) {
         Children = new ScreenChildrenCollection<object>(this);
      }

      public ViewModelScreenBase(TDescriptor descriptor, IServiceLocator serviceLocator = null)
         : base(descriptor, serviceLocator) {
         Children = new ScreenChildrenCollection<object>(this);
      }

      public ScreenChildrenCollection<object> Children { get; private set; }

      public IScreenBase Parent { get; set; }

      ICollection<object> IScreenBase.Children {
         get { return Children; }
      }

      // TODO: Required, senseful?
      protected void OnPropertyChanged<T>(Expression<Func<T>> propertySelector) {
         string propertyName = ExpressionService.GetPropertyName(propertySelector);
         OnPropertyChanged(propertyName);
      }
   }
}
