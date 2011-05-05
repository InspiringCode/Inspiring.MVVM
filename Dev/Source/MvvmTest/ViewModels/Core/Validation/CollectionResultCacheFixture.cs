namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

   /// <remarks>
   ///   <![CDATA[                                  
   ///                                     ___ItemA
   ///                                   /   
   ///                                  /       
   ///                 --> CollectionA  -------       
   ///                |                 \      |
   ///               OwnerOfAB           \     |    
   ///                |                   \    |
   ///                 --> CollectionB ----\--ItemAB
   ///                                 \    \
   ///                                  \    \
   ///                                   \____ItemABC 
   ///                                             \
   ///                                              ----- CollectionC <--- OwnerOfC
   ///                                             /
   ///                                        ItemC
   ///   ]]>                                       
   /// </remarks>
   [TestClass]
   public class CollectionResultCacheFixture : TestBase {
      protected TwoItemListsVM OwnerOfAB { get; private set; }
      protected ItemListVM OwnerOfC { get; private set; }

      protected ItemVM ItemA { get; private set; }
      protected ItemVM ItemAB { get; private set; }
      protected ItemVM ItemABC { get; private set; }
      protected ItemVM ItemC { get; private set; }

      protected const string CollectionAValidatorKey = "CollectionA";
      protected const string CollectionBValidatorKey = "CollectionB";
      protected const string CollectionCValidatorKey = "CollectionC";

      protected CustomValidatorMockConfiguration Results { get; private set; }

      [TestInitialize]
      public void FixtureSetup() {
         Results = new CustomValidatorMockConfiguration();
         var initialState = Results.GetState();

         OwnerOfAB = new TwoItemListsVM(Results);
         OwnerOfC = new ItemListVM(Results);

         ItemA = new ItemVM(Results, "ItemA");
         ItemAB = new ItemVM(Results, "ItemAB");
         ItemABC = new ItemVM(Results, "ItemABC");
         ItemC = new ItemVM(Results, "ItemC");

         OwnerOfAB.CollectionA.Add(ItemA);
         OwnerOfAB.CollectionA.Add(ItemAB);
         OwnerOfAB.CollectionA.Add(ItemABC);

         OwnerOfAB.CollectionB.Add(ItemAB);
         OwnerOfAB.CollectionB.Add(ItemABC);

         OwnerOfC.CollectionC.Add(ItemABC);
         OwnerOfC.CollectionC.Add(ItemC);

         initialState.RestoreToState();
      }

      protected class CustomValidatorMockConfiguration : ValidatorMockConfigurationFluent {
         public CustomValidatorMockConfiguration() {
            EnabledValidators = ValidatorTypes.Both;
         }

         public ValidatorTypes EnabledValidators { get; set; }

         protected override void PerformValidation(
            Action<string> addValidationErrorAction, 
            ValidatorType type, 
            IViewModel owner, 
            IViewModel targetVM, 
            object validatorKey, 
            IVMPropertyDescriptor targetProperty = null
         ) {
            bool validate = false;

            switch (type) {
               case ValidatorType.Property:
                  if ((EnabledValidators & ValidatorTypes.Property) == ValidatorTypes.Property) {
                     validate = true;
                  }
                  break;
               case ValidatorType.ViewModel:
                  if ((EnabledValidators & ValidatorTypes.ViewModel) == ValidatorTypes.ViewModel) {
                     validate = true;
                  }
                  break;
            }

            if (validate) {
               base.PerformValidation(addValidationErrorAction, type, owner, targetVM, validatorKey, targetProperty);
            }
         }

         protected override void PerformCollectionValidation<TItemVM>(Action<TItemVM, string, object> addValidationErrorAction, ValidatorType type, IViewModel owner, IVMCollectionBase<TItemVM> targetCollection, object validatorKey, IVMPropertyDescriptor targetProperty = null) {
            bool validate = false;

            switch (type) {
               case ValidatorType.CollectionProperty:
                  if ((EnabledValidators & ValidatorTypes.Property) == ValidatorTypes.Property) {
                     validate = true;
                  }
                  break;
               case ValidatorType.CollectionViewModel:
                  if ((EnabledValidators & ValidatorTypes.ViewModel) == ValidatorTypes.ViewModel) {
                     validate = true;
                  }
                  break;
            }

            if (validate) {
               base.PerformCollectionValidation<TItemVM>(addValidationErrorAction, type, owner, targetCollection, validatorKey, targetProperty);
            }
         }
      }

      protected class TwoItemListsVM : TestViewModel<TwoItemListsVMDescriptor> {
         public static readonly TwoItemListsVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<TwoItemListsVMDescriptor>()
            .For<TwoItemListsVM>()
            .WithProperties((d, c) => {
               var b = c.GetPropertyBuilder();

               d.CollectionA = b.Collection.Of<ItemVM>(ItemVM.ClassDescriptor);
               d.CollectionB = b.Collection.Of<ItemVM>(ItemVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.CheckCollection(x => x.CollectionA)
                  .Custom(args => args.Owner.Results.PerformValidation(args, CollectionAValidatorKey));

               b.CheckCollection<ItemVMDescriptor, string>(x => x.CollectionA, x => x.ItemProperty)
                  .Custom<ItemVM>(args => args.Owner.Results.PerformValidation(args, CollectionAValidatorKey));

               b.CheckCollection(x => x.CollectionB)
                  .Custom(args => args.Owner.Results.PerformValidation(args, CollectionBValidatorKey));

               b.CheckCollection<ItemVMDescriptor, string>(x => x.CollectionB, x => x.ItemProperty)
                  .Custom<ItemVM>(args => args.Owner.Results.PerformValidation(args, CollectionBValidatorKey));
            })
            .Build();

         public TwoItemListsVM(ValidatorMockConfiguration results)
            : base(ClassDescriptor) {
            Results = results;
         }

         public IVMCollection<ItemVM> CollectionA {
            get { return GetValue(Descriptor.CollectionA); }
         }

         public IVMCollection<ItemVM> CollectionB {
            get { return GetValue(Descriptor.CollectionB); }
         }

         private ValidatorMockConfiguration Results { get; set; }
      }

      protected class TwoItemListsVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ItemVM>> CollectionA { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ItemVM>> CollectionB { get; set; }
      }


      protected class ItemListVM : TestViewModel<ItemListVMDescriptor> {
         public static readonly ItemListVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ItemListVMDescriptor>()
            .For<ItemListVM>()
            .WithProperties((d, c) => {
               var b = c.GetPropertyBuilder();

               d.CollectionC = b.Collection.Of<ItemVM>(ItemVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.CheckCollection(x => x.CollectionC)
                 .Custom(args => args.Owner.Results.PerformValidation(args, CollectionCValidatorKey));

               b.CheckCollection<ItemVMDescriptor, string>(x => x.CollectionC, x => x.ItemProperty)
                  .Custom<ItemVM>(args => args.Owner.Results.PerformValidation(args, CollectionCValidatorKey));
            })
            .Build();

         public ItemListVM(ValidatorMockConfiguration results)
            : base(ClassDescriptor) {
            Results = results;
         }

         public IVMCollection<ItemVM> CollectionC {
            get { return GetValue(Descriptor.CollectionC); }
         }

         private ValidatorMockConfiguration Results { get; set; }
      }

      protected class ItemListVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ItemVM>> CollectionC { get; set; }
      }

      protected class ItemVM : TestViewModel<ItemVMDescriptor> {
         public static readonly ItemVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ItemVMDescriptor>()
            .For<ItemVM>()
            .WithProperties((d, c) => {
               var b = c.GetPropertyBuilder();
               d.ItemProperty = b.Property.Of<string>();
            })
            .WithValidators(b => {
               b.Check(x => x.ItemProperty)
                  .Custom(args => args.Owner.Results.PerformValidation(args));

               b.CheckViewModel(args => args.Owner.Results.PerformValidation(args));
            })
            .Build();

         public ItemVM(ValidatorMockConfiguration results, string name)
            : base(ClassDescriptor, name) {
            Results = results;
         }

         private ValidatorMockConfiguration Results { get; set; }
      }

      protected class ItemVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> ItemProperty { get; set; }
      }

      [Flags]
      protected enum ValidatorTypes {
         Property = 1,
         ViewModel = 2,
         Both = Property | ViewModel
      }
   }
}