namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Diagnostics.Contracts;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;

   public class ScreenBase : ParentScreenLifecycle, IScreen {

   }

   public class ScreenBase<TDescriptor> :
      ViewModel<TDescriptor>,
      IViewModelScreen
      where TDescriptor : VMDescriptorBase {

      public ScreenBase(IServiceLocator serviceLocator = null)
         : base(serviceLocator) {
         Children = new ScreenLifecycleCollection<IScreenLifecycle>(this);
      }

      public ScreenBase(TDescriptor descriptor, IServiceLocator serviceLocator = null)
         : base(descriptor, serviceLocator) {
         Children = new ScreenLifecycleCollection<IScreenLifecycle>(this);
      }

      public ScreenLifecycleCollection<IScreenLifecycle> Children { get; private set; }

      public IScreenLifecycle Parent { get; set; }

      void IScreenLifecycle.Activate() {
         Children.ActivateAll(parentCallback: OnActivate);
      }

      void IScreenLifecycle.Deactivate() {
         Children.DeactivateAll(parentCallback: OnDeactivate);
      }

      bool IScreenLifecycle.RequestClose() {
         return Children.RequestCloseAll(parentCallback: OnRequestClose);
      }

      void IScreenLifecycle.Close() {
         Children.CloseAll(parentCallback: OnClose);
      }

      protected virtual void OnActivate() {
      }

      protected virtual void OnDeactivate() {
      }

      protected virtual bool OnRequestClose() {
         return true;
      }

      protected virtual void OnClose() {
      }

      protected void OpenChildScreen<T>(
         IVMPropertyDescriptor<IScreen> screenProperty,
         IScreenFactory<T> childScreen
      ) where T : IScreen {
         Contract.Requires<ArgumentNullException>(screenProperty != null);
         Contract.Requires<ArgumentNullException>(childScreen != null);

         T screen = Children.AddNew(childScreen);
         SetDisplayValue(screenProperty, screen);
      }

      protected bool CloseChildScreen(IVMPropertyDescriptor<IScreen> screenProperty) {
         Contract.Requires<ArgumentNullException>(screenProperty != null);

         var screen = (IScreen)GetDisplayValue(screenProperty);

         if (screen == null) {
            return true;
         }

         if (screen.RequestClose()) {
            screen.Close();
            return true;
         }

         return false;
      }

      // TODO: Required, senseful?
      protected void OnPropertyChanged<T>(Expression<Func<T>> propertySelector) {
         string propertyName = ExpressionService.GetPropertyName(propertySelector);
         OnPropertyChanged(propertyName);
      }
   }
}
