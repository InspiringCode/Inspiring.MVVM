using Inspiring.Mvvm.ViewModels.Core.Common;
namespace Inspiring.Mvvm.ViewModels.Core.Kernel {

   internal interface IPropertyBehaviorContext {
      void NotifyPropertyValidating(object todo);

      void NotifyChange(ChangeArgs args);
   }
}
