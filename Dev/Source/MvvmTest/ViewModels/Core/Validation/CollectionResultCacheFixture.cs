namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   /// <remarks>
   ///   <![CDATA[                                  
   ///                                     ___ItemA
   ///                                   /   
   ///                                  /       
   ///                 --> CollectionA  -------       
   ///                |                 \      |
   ///               Owner1              \     |    
   ///                |                   \    |
   ///                 --> CollectionB ----\--ItemAB
   ///                                 \    \
   ///                                  \    \
   ///                                   \____ItemABC 
   ///                                             \
   ///                                              ----- CollectionC <--- Owner2
   ///                                             /
   ///                                        ItemC
   ///   ]]>                                       
   /// </remarks>
   [TestClass]
   public class CollectionResultCacheFixture : TestBase {
      protected TwoItemListsVM Owner1 { get; private set; }
      protected ItemListVM Owner2 { get; private set; }

      protected ItemVM ItemA { get; private set; }
      protected ItemVM ItemAB { get; private set; }
      protected ItemVM ItemABC { get; private set; }
      protected ItemVM ItemC { get; private set; }

      protected const string CollectionAValidatorKey = "CollectionA";
      protected const string CollectionBValidatorKey = "CollectionB";
      protected const string CollectionCValidatorKey = "CollectionC";

      protected ValidatorMockConfigurationFluent Results { get; private set; }

      [TestInitialize]
      public void FixtureSetup() {
         Results = new ValidatorMockConfigurationFluent();
         var initialState = Results.GetState();

         Owner1 = new TwoItemListsVM(Results);
         Owner2 = new ItemListVM(Results);

         ItemA = new ItemVM(Results, "ItemA");
         ItemAB = new ItemVM(Results, "ItemAB");
         ItemABC = new ItemVM(Results, "ItemABC");
         ItemC = new ItemVM(Results, "ItemC");

         Owner1.CollectionA.Add(ItemA);
         Owner1.CollectionA.Add(ItemAB);
         Owner1.CollectionA.Add(ItemABC);

         Owner1.CollectionB.Add(ItemAB);
         Owner1.CollectionB.Add(ItemABC);

         Owner2.CollectionC.Add(ItemABC);
         Owner2.CollectionC.Add(ItemC);

         initialState.RestoreToState();
      }

      public class TwoItemListsVM : TestViewModel<TwoItemListsVMDescriptor> {
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

      public class TwoItemListsVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ItemVM>> CollectionA { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ItemVM>> CollectionB { get; set; }
      }


      public class ItemListVM : TestViewModel<ItemListVMDescriptor> {
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

      public class ItemListVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ItemVM>> CollectionC { get; set; }
      }

      public class ItemVM : TestViewModel<ItemVMDescriptor> {
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

      public class ItemVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> ItemProperty { get; set; }
      }
   }
}