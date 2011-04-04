namespace Inspiring.MvvmTest.ApiTests.ViewModels.Undo {
   using Inspiring.Mvvm.ViewModels;

   public sealed class CustomerVM : DefaultViewModelWithSourceBase<CustomerVMDescriptor, Customer> {
      public static readonly CustomerVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<CustomerVMDescriptor>()
         .For<CustomerVM>()
         .WithProperties((d, c) => {
            var b = c.GetPropertyBuilder(x => x.Source);

            d.Name = b.Property.MapsTo(x => x.Name);
         })
         .WithViewModelBehaviors(b => {
            b.EnableUndo();
         })
         .Build();

      public CustomerVM()
         : base(ClassDescriptor) {
      }
   }

   public sealed class CustomerVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Name { get; set; }
   }
}
