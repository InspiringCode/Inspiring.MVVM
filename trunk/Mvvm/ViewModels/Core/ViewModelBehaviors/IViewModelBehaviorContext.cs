namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;
   using Inspiring.Mvvm.ViewModels.Core.Contracts;

   /// <summary>
   ///   Holds that state needed by operations of a <see cref="ViewModelBehavior"/>.
   /// </summary>
   [ContractClass(typeof(IViewModelBehaviorContextContract))]
   public interface IViewModelBehaviorContext {
      IViewModel VM { get; }
   }

   namespace Contracts {
      [ContractClassFor(typeof(IViewModelBehaviorContext))]
      internal class IViewModelBehaviorContextContract : IViewModelBehaviorContext {
         public IViewModel VM {
            get {
               Contract.Ensures(Contract.Result<IViewModel>() != null);
               return default(IViewModel);
            }
         }
      }
   }
}