namespace Inspiring.Mvvm.Screens {
   using System.Collections.Generic;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;

   public class ScreenBase : IScreenBase {
      public ScreenBase(EventAggregator aggregator) {
         Children = new ScreenChildrenCollection<object>(aggregator, this);
         Lifecycle = new ScreenLifecycle(aggregator, this);
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

      public ViewModelScreenBase(EventAggregator aggregator, IServiceLocator serviceLocator = null)
         : base(serviceLocator) {
         Children = new ScreenChildrenCollection<object>(aggregator, this);
         Lifecycle = new ScreenLifecycle(aggregator, this);
      }

      public ViewModelScreenBase(TDescriptor descriptor, EventAggregator aggregator, IServiceLocator serviceLocator = null)
         : base(descriptor, serviceLocator) {
         Children = new ScreenChildrenCollection<object>(aggregator, this);
         Lifecycle = new ScreenLifecycle(aggregator, this);
      }

      public ScreenChildrenCollection<object> Children { get; private set; }

      public IScreenBase Parent { get; set; }

      ICollection<object> IScreenBase.Children {
         get { return Children; }
      }

      protected ScreenLifecycle Lifecycle { get; private set; }
   }
}
