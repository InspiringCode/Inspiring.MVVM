namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionValidationFixture {
      protected ValidatorMockConfigurationFluent Setup { get; private set; }
      protected ListVM List { get; private set; }
      protected ItemVMDescriptor ItemDescriptor { get; private set; }

      [TestInitialize]
      public void BaseSetup() {
         Setup = new ValidatorMockConfigurationFluent();
         List = new ListVM(Setup);
         ItemDescriptor = ItemVM.ClassDescriptor;
      }

      protected ItemVM CreateItem(string name = "Item") {
         return new ItemVM(Setup, name);
      }


      //
      //   V I E W   M O D E L S
      // 

      protected class ListVM : TestViewModel<ListVMDescriptor> {
         public static readonly ListVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ListVMDescriptor>()
            .For<ListVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Items = v.Collection.Of<ItemVM>(ItemVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.CheckCollection(x => x.Items, x => x.CollectionProperty)
                  .Custom<ItemVM>(args => args.Owner.Setup.PerformValidation(args));

               b.CheckCollection(x => x.Items)
                  .Custom(args => args.Owner.Setup.PerformValidation(args));
            })
            .Build();

         public ListVM(ValidatorMockConfiguration setup)
            : base(ClassDescriptor) {
            Setup = setup;
         }

         private ValidatorMockConfiguration Setup { get; set; }

         public IVMCollection<ItemVM> Items {
            get { return GetValue(Descriptor.Items); }
         }
      }

      protected class ListVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ItemVM>> Items { get; set; }
      }

      protected class ItemVM : TestViewModel<ItemVMDescriptor> {
         public static readonly ItemVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ItemVMDescriptor>()
            .For<ItemVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.ItemProperty = v.Property.Of<string>();
               d.CollectionProperty = v.Property.Of<string>();
            })
            .WithValidators(b => {
               b.Check(x => x.ItemProperty).Custom(args => args.Owner.Setup.PerformValidation(args));
               b.Check(x => x.CollectionProperty).Custom(args => args.Owner.Setup.PerformValidation(args));
               b.CheckViewModel(args => args.Owner.Setup.PerformValidation(args));

               b.EnableParentValidation(x => x.CollectionProperty);
               b.EnableParentViewModelValidation();
            })
            .Build();

         private readonly string _name;

         public ItemVM(ValidatorMockConfiguration setup, string name)
            : base(ClassDescriptor) {
            _name = name;
            Setup = setup;
         }

         public ValidationError ItemPropertyError { get; set; }

         private ValidatorMockConfiguration Setup { get; set; }

         public override string ToString() {
            return _name;
         }
      }

      protected class ItemVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> ItemProperty { get; set; }
         public IVMPropertyDescriptor<string> CollectionProperty { get; set; }
      }
   }
}