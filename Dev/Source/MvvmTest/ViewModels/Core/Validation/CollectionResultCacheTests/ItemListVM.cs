namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core.Validation.Validators;

   public class ItemListVM : ViewModel<ItemListVMDescriptor> {
      public static readonly ItemListVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<ItemListVMDescriptor>()
         .For<ItemListVM>()
         .WithProperties((d, c) => {
            var b = c.GetPropertyBuilder();

            d.Collection3 = b.Collection.Of<ItemVM>(ItemVM.ClassDescriptor);
         })
         .WithValidators(b => {
            b.CheckCollection(x => x.Collection3)
              .Custom(Collection3ViewModelValidator);

            b.CheckCollection<ItemVMDescriptor, string>(x => x.Collection3, x => x.Name)
               .Custom<ItemVM>(Collection3PropertyValidator);
         })
         .Build();

      private readonly CollectionResultCacheTests _testFixture;
      private readonly ValidatorInvocationLog _invocationLog;

      public ItemListVM(CollectionResultCacheTests testFixture, ValidatorInvocationLog invocationLog)
         : base(ClassDescriptor) {
         _testFixture = testFixture;
         _invocationLog = invocationLog;
      }

      public CollectionResultCacheTests TestFixture {
         get { return _testFixture; }
      }

      private ValidatorInvocationLog InvocationLog {
         get { return _invocationLog; }
      }

      private static void Collection3ViewModelValidator(
         CollectionValidationArgs<ItemListVM, ItemVM> args
      ) {
         args.Owner.InvocationLog.AddCall(
            CollectionResultCacheTests.Validator.Collection3ViewModelValidator,
            args
         );
         var invalidItems = args.Owner.TestFixture.InvalidItemsOfCollection3ViewModelValidator.ToList();

         invalidItems.ForEach(i =>
            args.AddError(i, CollectionResultCacheTests.Collection3ViewModelValidationErrorMessage)
         );
      }

      private static void Collection3PropertyValidator(
         CollectionValidationArgs<ItemListVM, ItemVM, string> args
      ) {
         args.Owner.InvocationLog.AddCall(
            CollectionResultCacheTests.Validator.Collection3PropertyValidator,
            args
         );
         var invalidItems = args.Owner.TestFixture.InvalidItemsOfCollection3PropertyValidator.ToList();

         invalidItems.ForEach(i =>
            args.AddError(i, CollectionResultCacheTests.Collection3PropertyValidationErrorMessage)
         );
      }
   }

   public class ItemListVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<IVMCollection<ItemVM>> Collection3 { get; set; }
   }
}