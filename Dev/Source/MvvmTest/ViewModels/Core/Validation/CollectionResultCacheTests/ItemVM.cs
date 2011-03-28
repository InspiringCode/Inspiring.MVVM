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
            b.Check(x => x.Name).Custom(FirstPropertyValidator);
         })
         .Build();

      public ItemVM()
         : base(ClassDescriptor) {
      }

      private static void FirstPropertyValidator(
         ItemVM vm,
         string value,
         ValidationArgs args) {

         if (false) {
            args.AddError("Property validator error");
         }
      }
   }

   public class ItemVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Name { get; set; }
   }
}