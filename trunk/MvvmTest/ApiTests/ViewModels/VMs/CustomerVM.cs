namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public sealed class CustomerVM : ViewModel<CustomerVMDescriptor>, ICanInitializeFrom<Customer>, IVMCollectionItem<Customer> {
      public static readonly CustomerVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<CustomerVMDescriptor>()
         .For<CustomerVM>()
         .WithProperties((d, c) => {
            var cust = c.GetPropertyBuilder(x => x.CustomerSource);

            d.Title = cust.Property.MapsTo(x => x.Title);
         })
         .Build();

      public CustomerVM()
         : base(ClassDescriptor) {
      }

      public Customer CustomerSource { get; private set; }

      public void InitializeFrom(Customer source) {
         CustomerSource = source;
      }

      public Customer Source {
         get { return CustomerSource; }
      }
   }

   public sealed class CustomerVMDescriptor : VMDescriptor {
      public IVMProperty<string> Title { get; set; }
   }
}
