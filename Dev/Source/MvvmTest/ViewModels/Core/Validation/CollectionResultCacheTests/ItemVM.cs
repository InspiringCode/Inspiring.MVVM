namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class ItemVM : ViewModel<ItemVMDescriptor> {
      public static readonly ItemVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<ItemVMDescriptor>()
         .For<ItemVM>()
         .WithProperties((d, c) => {
            var b = c.GetPropertyBuilder();
            d.Name = b.Property.Of<string>();
         })
         .WithValidators(b => {
            b.Check(x => x.Name).Custom(PropertyValidator);
         })
         .Build();

      private readonly CollectionResultCacheTests _testFixture;

      public ItemVM(CollectionResultCacheTests testFixture, string name)
         : base(ClassDescriptor) {
         _testFixture = testFixture;
         SetValue(Descriptor.Name, name);
      }

      public CollectionResultCacheTests TestFixture {
         get { return _testFixture; }
      }

      public void Revalidate() {
         base.Revalidate();
      }

      private static void PropertyValidator(
         PropertyValidationArgs<ItemVM, ItemVM, string> args
      ) {
         if (args.Owner.TestFixture.InvalidItemsOfItemVMPropertyValidator.Contains(args.Target)) {
            args.AddError(CollectionResultCacheTests.NamePropertyValidatorErrorMessage);
         }
      }
   }

   public class ItemVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Name { get; set; }
   }
}