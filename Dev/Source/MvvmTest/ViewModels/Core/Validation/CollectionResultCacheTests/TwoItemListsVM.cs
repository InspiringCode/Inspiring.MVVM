namespace Inspiring.MvvmTest.ViewModels.Core.Validation {

   using System.Collections.Generic;
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

      public TwoItemListsVM(CollectionResultCacheTests testFixture)
         : base(ClassDescriptor) {
         _testFixture = testFixture;
      }

      public CollectionResultCacheTests TestFixture {
         get { return _testFixture; }
      }

      public int Collection1ViewModelValidatorCount { get; private set; }
      public int Collection1PropertyValidatorCount { get; private set; }
      public int Collection2ViewModelValidatorCount { get; private set; }
      public int Collection2PropertyValidatorCount { get; private set; }

      public void ResetValidatorInvocationCounts() {
         Collection1ViewModelValidatorCount = 0;
         Collection1PropertyValidatorCount = 0;
         Collection2ViewModelValidatorCount = 0;
         Collection2PropertyValidatorCount = 0;
      }

      private static void Collection1ViewModelValidator(
         ItemVM item,
         IEnumerable<ItemVM> items,
         ValidationArgs args
      ) {
         var owner = (TwoItemListsVM)args.OwnerVM;
         owner.Collection1ViewModelValidatorCount++;

         var invalidItems = owner.TestFixture.InvalidItemsOfCollection1ViewModelValidator.ToList();

         if (invalidItems.Contains(item)) {
            args.AddError(item, CollectionResultCacheTests.Collection1ViewModelValidationErrorMessage);
         }
      }

      private static void Collection1PropertyValidator(
         ItemVM item,
         IEnumerable<ItemVM> items,
         IVMPropertyDescriptor<string> property,
         ValidationArgs args
      ) {
         var owner = (TwoItemListsVM)args.OwnerVM;
         owner.Collection1PropertyValidatorCount++;

         var invalidItems = owner.TestFixture.InvalidItemsOfCollection1PropertyValidator.ToList();

         if (invalidItems.Contains(item)) {
            args.AddError(item, CollectionResultCacheTests.Collection1PropertyValidationErrorMessage);
         }
      }

      private static void Collection2ViewModelValidator(
         ItemVM item,
         IEnumerable<ItemVM> items,
         ValidationArgs args
      ) {
         var owner = (TwoItemListsVM)args.OwnerVM;
         owner.Collection2ViewModelValidatorCount++;

         var invalidItems = owner.TestFixture.InvalidItemsOfCollection2ViewModelValidator.ToList();

         if (invalidItems.Contains(item)) {
            args.AddError(item, CollectionResultCacheTests.Collection2ViewModelValidationErrorMessage);
         }
      }

      private static void Collection2PropertyValidator(
         ItemVM item,
         IEnumerable<ItemVM> items,
         IVMPropertyDescriptor<string> property,
         ValidationArgs args
      ) {
         var owner = (TwoItemListsVM)args.OwnerVM;
         owner.Collection2PropertyValidatorCount++;
         var invalidItems = owner.TestFixture.InvalidItemsOfCollection2PropertyValidator.ToList();

         if (invalidItems.Contains(item)) {
            args.AddError(item, CollectionResultCacheTests.Collection2PropertyValidationErrorMessage);
         }
      }
   }

   public class TwoItemListsVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<IVMCollection<ItemVM>> Collection1 { get; set; }
      public IVMPropertyDescriptor<IVMCollection<ItemVM>> Collection2 { get; set; }
   }
}