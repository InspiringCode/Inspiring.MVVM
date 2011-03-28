namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class ItemListVM : ViewModel<ItemListVMDescriptor> {

      public static readonly ItemListVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<ItemListVMDescriptor>()
         .For<ItemListVM>()
         .WithProperties((d, c) => {
            var b = c.GetPropertyBuilder();

            d.Items = b.Collection.Of<ItemVM>(ItemVM.ClassDescriptor);
         })
         .WithValidators(b => {
            b.CheckCollection<ItemVMDescriptor, string>(x => x.Items, x => x.Name)
               .Custom<ItemVM>(FirstCollectionValidator);
         })
         .Build();

      public ItemListVM()
         : base(ClassDescriptor) {
      }

      public IVMCollection<ItemVM> Items {
         get { return GetValue(Descriptor.Items); }
      }

      private static void FirstCollectionValidator(
         ItemVM item,
         IEnumerable<ItemVM> items,
         IVMPropertyDescriptor<string> property,
         ValidationArgs args
      ) {

      }
   }

   public class ItemListVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<IVMCollection<ItemVM>> Items { get; set; }
   }
}
