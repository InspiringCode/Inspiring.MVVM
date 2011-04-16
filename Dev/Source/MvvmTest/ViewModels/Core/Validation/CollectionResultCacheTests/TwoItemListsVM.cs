namespace Inspiring.MvvmTest.ViewModels.Core.Validation {

   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class TwoItemListsVM : ViewModel<TwoItemListsVMDescriptor> {
      public static readonly TwoItemListsVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<TwoItemListsVMDescriptor>()
         .For<TwoItemListsVM>()
         .WithProperties((d, c) => {
            var b = c.GetPropertyBuilder();

            d.Collection1 = b.Collection.Of<ItemVM>(ItemVM.ClassDescriptor);
            d.Collection2 = b.Collection.Of<ItemVM>(ItemVM.ClassDescriptor);
         })
         .WithValidators(b => {
            b.CheckCollection(x => x.Collection1)
               .Custom(Collection1ViewModelValidator);

            b.CheckCollection<ItemVMDescriptor, string>(x => x.Collection1, x => x.Name)
               .Custom<ItemVM>(Collection1PropertyValidator);

            b.CheckCollection(x => x.Collection2)
               .Custom(Collection2ViewModelValidator);

            b.CheckCollection<ItemVMDescriptor, string>(x => x.Collection2, x => x.Name)
               .Custom<ItemVM>(Collection2PropertyValidator);
         })
         .Build();

      private readonly CollectionResultCacheTests _testFixture;
      private readonly ValidatorInvocationLog _invocationLog;

      public TwoItemListsVM(CollectionResultCacheTests testFixture, ValidatorInvocationLog invocationLog)
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

      private static void Collection1ViewModelValidator(
         CollectionValidationArgs<TwoItemListsVM, ItemVM> args
      ) {
         args.Owner.InvocationLog.AddCall(
            CollectionResultCacheTests.Validator.Collection1ViewModelValidator,
            args
         );
         var invalidItems = args.Owner.TestFixture.InvalidItemsOfCollection1ViewModelValidator.ToList();

         invalidItems.ForEach(i =>
            args.AddError(i, CollectionResultCacheTests.Collection1ViewModelValidationErrorMessage)
         );
      }

      private static void Collection1PropertyValidator(
         CollectionValidationArgs<TwoItemListsVM, ItemVM, string> args
      ) {
         args.Owner.InvocationLog.AddCall(
            CollectionResultCacheTests.Validator.Collection1PropertyValidator,
            args
         );
         var invalidItems = args.Owner.TestFixture.InvalidItemsOfCollection1PropertyValidator.ToList();

         invalidItems.ForEach(i =>
            args.AddError(i, CollectionResultCacheTests.Collection1PropertyValidationErrorMessage)
         );
      }

      private static void Collection2ViewModelValidator(
         CollectionValidationArgs<TwoItemListsVM, ItemVM> args
      ) {
         args.Owner.InvocationLog.AddCall(
            CollectionResultCacheTests.Validator.Collection2ViewModelValidator,
            args
         );

         var invalidItems = args.Owner.TestFixture.InvalidItemsOfCollection2ViewModelValidator.ToList();

         invalidItems.ForEach(i =>
            args.AddError(i, CollectionResultCacheTests.Collection2ViewModelValidationErrorMessage)
         );
      }

      private static void Collection2PropertyValidator(
         CollectionValidationArgs<TwoItemListsVM, ItemVM, string> args
      ) {
         args.Owner.InvocationLog.AddCall(
            CollectionResultCacheTests.Validator.Collection2PropertyValidator,
            args
         );
         var invalidItems = args.Owner.TestFixture.InvalidItemsOfCollection2PropertyValidator.ToList();

         invalidItems.ForEach(i =>
            args.AddError(i, CollectionResultCacheTests.Collection2PropertyValidationErrorMessage)
         );
      }
   }

   public class TwoItemListsVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<IVMCollection<ItemVM>> Collection1 { get; set; }
      public IVMPropertyDescriptor<IVMCollection<ItemVM>> Collection2 { get; set; }
   }
}