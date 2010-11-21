namespace Inspiring.Mvvm.ViewModels.Core {
   using System.Diagnostics.Contracts;
   using Contracts;

   /// <summary>
   ///   Holds that state needed by operations of a <see cref="ViewModelBehavior"/>.
   /// </summary>
   [ContractClass(typeof(IBehaviorContextContract))]
   public interface IBehaviorContext {
      IViewModel VM { get; }

      FieldValueHolder FieldValues { get; }

      IServiceLocator ServiceLocator { get; }

      void NotifyValidating(_ValidationArgs args);

      void NotifyChange(ChangeArgs args);
   }

   namespace Contracts {
      [ContractClassFor(typeof(IBehaviorContext))]
      internal class IBehaviorContextContract : IBehaviorContext {
         public IViewModel VM {
            get {
               Contract.Ensures(Contract.Result<IViewModel>() != null);
               return default(IViewModel);
            }
         }

         public FieldValueHolder FieldValues {
            get {
               Contract.Ensures(Contract.Result<FieldValueHolder>() != null);
               return default(FieldValueHolder);
            }
         }

         public void NotifyChange(ChangeArgs args) {
            Contract.Requires(args != null);
         }

         public void NotifyValidating(_ValidationArgs args) {
            Contract.Requires(args != null);
         }


         public IServiceLocator ServiceLocator {
            get {
               Contract.Ensures(Contract.Result<IServiceLocator>() != null);
               return default(IServiceLocator);
            }
         }
      }
   }
}