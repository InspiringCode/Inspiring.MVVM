namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Diagnostics.Contracts;
   using System.Linq.Expressions;
   using Inspiring.Mvvm.Common;
   using Inspiring.Mvvm.ViewModels;

   public class ScreenBase : ParentScreenLifecycle, IScreenBase {
      public IScreenBase Parent { get; set; }
   }

   public class ViewModelScreenBase<TDescriptor> :
      ViewModel<TDescriptor>,
      IViewModelScreenBase
      where TDescriptor : IVMDescriptor {

      public ViewModelScreenBase(IServiceLocator serviceLocator = null)
         : base(serviceLocator) {
         Children = new ScreenLifecycleCollection<IScreenLifecycle>(this);
      }

      public ViewModelScreenBase(TDescriptor descriptor, IServiceLocator serviceLocator = null)
         : base(descriptor, serviceLocator) {
         Children = new ScreenLifecycleCollection<IScreenLifecycle>(this);
      }

      public ScreenLifecycleCollection<IScreenLifecycle> Children { get; private set; }

      public IScreenBase Parent { get; set; }

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

      public void Corrupt(object data = null) {
         Children.CorruptAll(data, parentCallback: () => OnCorrupt(data));
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

      protected virtual void OnCorrupt(object data) {
      }

      protected void OpenChildScreen<T>(
         IVMPropertyDescriptor<IScreenBase> screenProperty,
         IScreenFactory<T> childScreen
      ) where T : IScreenBase {
         Contract.Requires<ArgumentNullException>(screenProperty != null);
         Contract.Requires<ArgumentNullException>(childScreen != null);

         T screen = Children.AddNew(childScreen);
         screen.Activate();
         SetDisplayValue(screenProperty, screen);
      }

      protected bool CloseChildScreen(IVMPropertyDescriptor<IScreenBase> screenProperty) {
         Contract.Requires<ArgumentNullException>(screenProperty != null);

         var screen = (IScreenBase)GetDisplayValue(screenProperty);

         if (screen == null) {
            return true;
         }

         if (screen.RequestClose()) {
            screen.Deactivate();
            screen.Close();
            Children.Remove(screen);
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
