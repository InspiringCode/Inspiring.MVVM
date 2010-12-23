namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public sealed class CustomerVM : ViewModel<CustomerVMDescriptor>, ICanInitializeFrom<Customer> {
      public static readonly CustomerVMDescriptor Descriptor = VMDescriptorBuilder
         .OfType<CustomerVMDescriptor>()
         .For<CustomerVM>()
         .WithProperties((d, c) => {
            var cust = c.GetPropertyBuilder(x => x.CustomerSource);

            d.Title = cust.Property.MapsTo(x => x.Title);
         })
         .Build();

      public CustomerVM()
         : base(Descriptor) {
      }

      public Customer CustomerSource { get; private set; }

      public void InitializeFrom(Customer source) {
         CustomerSource = source;
      }
   }

   public sealed class CustomerVMDescriptor : VMDescriptor {
      public VMProperty<string> Title { get; set; }
   }
}
