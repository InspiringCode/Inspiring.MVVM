namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

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

      public ItemListVM(CollectionResultCacheTests testFixture)
         : base(ClassDescriptor) {
         _testFixture = testFixture;
      }

      public CollectionResultCacheTests TestFixture {
         get { return _testFixture; }
      }

      public int Collection3ViewModelValidatorCount { get; private set; }
      public int Collection3PropertyValidatorCount { get; private set; }

      public void ResetValidatorInvocationCounts() {
         Collection3ViewModelValidatorCount = 0;
         Collection3PropertyValidatorCount = 0;
      }

      private static void Collection3ViewModelValidator(
         ItemVM item,
         IEnumerable<ItemVM> items,
         ValidationArgs args
      ) {
         var owner = (ItemListVM)args.OwnerVM;
         owner.Collection3ViewModelValidatorCount++;

         var invalidItems = owner.TestFixture.InvalidItemsOfCollection3ViewModelValidator.ToList();

         if (invalidItems.Contains(item)) {
            args.AddError(item, CollectionResultCacheTests.Collection3ViewModelValidationErrorMessage);
         }
      }

      private static void Collection3PropertyValidator(
         ItemVM item,
         IEnumerable<ItemVM> items,
         IVMPropertyDescriptor<string> property,
         ValidationArgs args
      ) {
         var owner = (ItemListVM)args.OwnerVM;
         owner.Collection3PropertyValidatorCount++;

         var invalidItems = owner.TestFixture.InvalidItemsOfCollection3PropertyValidator.ToList();

         if (invalidItems.Contains(item)) {
            args.AddError(item, CollectionResultCacheTests.Collection3PropertyValidationErrorMessage);
         }
      }
   }

   public class ItemListVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<IVMCollection<ItemVM>> Collection3 { get; set; }
   }
}