namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidationResultCachingTests {
      private OwnerVM Owner { get; set; }

      [TestInitialize]
      public void Setup() {
         Owner = new OwnerVM();
      }

      [TestMethod]
      public void OwnerResult_WhenDelegatedViewModelPropertyIsRefreshedAndReturnsNewInvalidViewModel_ContainsError() {
         Owner.DelegatedPropertyResultToReturn = new ItemVM();
         Owner.Load(x => x.DelegatedProperty);
         ValidationAssert.IsValid(Owner);

         var invalidChild = CreateInvalidItem();
         Owner.DelegatedPropertyResultToReturn = invalidChild;
         ValidationAssert.IsValid(Owner);

         Owner.Refresh(x => x.DelegatedProperty);
         Assert.IsFalse(Owner.DelegatedProperty.IsValid);

         ValidationAssert.ErrorMessages(Owner.ValidationResult, invalidChild.ErrorToReturn);
      }

      [TestMethod]
      public void OwnerResult_WhenInvalidItemIsInTwoCollections_ContainsItemErrorOnlyOnce() {
         var invalidItem = CreateInvalidItem();
         Owner.CollectionOne.Add(invalidItem);
         Owner.CollectionTwo.Add(invalidItem);

         ValidationAssert.ErrorMessages(Owner.ValidationResult, invalidItem.ErrorToReturn);
      }

      [TestMethod]
      public void OwnerResult_WhenInvalidItemIsInTwoCollectionAndRemovedFromOne_StillContainsItemError() {
         var invalidItem = CreateInvalidItem();
         Owner.CollectionOne.Add(invalidItem);
         Owner.CollectionTwo.Add(invalidItem);

         Owner.CollectionTwo.Remove(invalidItem);

         ValidationAssert.ErrorMessages(Owner.ValidationResult, invalidItem.ErrorToReturn);
      }

      [TestMethod]
      public void OwnerResult_WhenInvalidItemIsInTwoCollectionAndRemovedFromBoth_DoesNotContainItemErrorAnymore() {
         var invalidItem = CreateInvalidItem();
         Owner.CollectionOne.Add(invalidItem);
         Owner.CollectionTwo.Add(invalidItem);

         Owner.CollectionOne.Remove(invalidItem);
         Owner.CollectionTwo.Remove(invalidItem);

         ValidationAssert.IsValid(Owner);
      }

      [TestMethod]
      public void OwnerResult_WhenInvalidItemIsInCollectionAndReferencedByProperty_ContainsItemErrorOnlyOnce() {
         var invalidItem = CreateInvalidItem();
         Owner.CollectionOne.Add(invalidItem);
         Owner.ItemProperty = invalidItem;

         ValidationAssert.ErrorMessages(Owner.ValidationResult, invalidItem.ErrorToReturn);
      }

      [TestMethod]
      public void OwnerResult_WhenInvalidItemIsInCollectionAndReferencedByPropertyAndRemovedFromCollection_StillContainsItemError() {
         var invalidItem = CreateInvalidItem();
         Owner.CollectionOne.Add(invalidItem);
         Owner.ItemProperty = invalidItem;

         Owner.CollectionOne.Remove(invalidItem);

         ValidationAssert.ErrorMessages(Owner.ValidationResult, invalidItem.ErrorToReturn);
      }

      [TestMethod]
      public void OwnerResult_WhenInvalidItemIsInCollectionAndReferencedByPropertyAndPropertyIsSetToNull_StillContainsItemError() {
         var invalidItem = CreateInvalidItem();
         Owner.CollectionOne.Add(invalidItem);
         Owner.ItemProperty = invalidItem;

         Owner.ItemProperty = null;

         ValidationAssert.ErrorMessages(Owner.ValidationResult, invalidItem.ErrorToReturn);
      }

      [TestMethod]
      public void OwnerResult_WhenInvalidItemIsRemovedFromCollectionAndPropertyIsSetToNull_DoesNotContainItemErrorAnymore() {
         var invalidItem = CreateInvalidItem();
         Owner.CollectionOne.Add(invalidItem);
         Owner.ItemProperty = invalidItem;

         Owner.CollectionOne.Remove(invalidItem);
         Owner.ItemProperty = null;

         ValidationAssert.IsValid(Owner);
      }

      [TestMethod]
      public void OwnerResult_WhenDelegatedViewModelPropertyIsLoadedAndReturnsNewInvalidViewModel_ContainsError() {
         // This IsValid call is important, otherwise the bug that is tested here isn't triggered!
         ValidationAssert.IsValid(Owner);

         var invalidChild = CreateInvalidItem();
         Owner.DelegatedPropertyResultToReturn = invalidChild;
         Owner.Load(x => x.DelegatedProperty);

         ValidationAssert.ErrorMessages(Owner.ValidationResult, invalidChild.ErrorToReturn);
      }

      private ItemVM CreateInvalidItem() {
         var item = new ItemVM { ErrorToReturn = "Item error" };
         item.Revalidate();
         return item;
      }

      private class OwnerVM : TestViewModel<OwnerVMDescriptor> {
         public static readonly OwnerVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<OwnerVMDescriptor>()
            .For<OwnerVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.CollectionOne = v.Collection.Of<ItemVM>(ItemVM.ClassDescriptor);
               d.CollectionTwo = v.Collection.Of<ItemVM>(ItemVM.ClassDescriptor);
               d.ItemProperty = v.VM.Of<ItemVM>();
               d.DelegatedProperty = v.VM.DelegatesTo(x => x.DelegatedPropertyResultToReturn);
            })
            .Build();

         public OwnerVM()
            : base(ClassDescriptor) {
         }

         public ItemVM DelegatedPropertyResultToReturn { get; set; }

         public IVMCollection<ItemVM> CollectionOne {
            get { return GetValue(Descriptor.CollectionOne); }
         }

         public IVMCollection<ItemVM> CollectionTwo {
            get { return GetValue(Descriptor.CollectionTwo); }
         }

         public ItemVM ItemProperty {
            get { return GetValue(Descriptor.ItemProperty); }
            set { SetValue(Descriptor.ItemProperty, value); }
         }

         public ItemVM DelegatedProperty {
            get { return GetValue(Descriptor.DelegatedProperty); }
            set { SetValue(Descriptor.DelegatedProperty, value); }
         }
      }

      private class OwnerVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ItemVM>> CollectionOne { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ItemVM>> CollectionTwo { get; set; }
         public IVMPropertyDescriptor<ItemVM> ItemProperty { get; set; }
         public IVMPropertyDescriptor<ItemVM> DelegatedProperty { get; set; }
      }

      private class ItemVM : ViewModel<ItemVMDescriptor> {
         public static readonly ItemVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ItemVMDescriptor>()
            .For<ItemVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
            })
            .WithValidators(b => {
               b.CheckViewModel(args => {
                  string errorMessage = args.Owner.ErrorToReturn;
                  if (errorMessage != null) {
                     args.AddError(errorMessage);
                  }
               });
            })
            .Build();

         public ItemVM()
            : base(ClassDescriptor) {
         }

         public string ErrorToReturn { get; set; }
      }

      private class ItemVMDescriptor : VMDescriptor {
      }
   }
}