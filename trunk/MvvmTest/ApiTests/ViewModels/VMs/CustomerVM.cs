namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public sealed class CustomerVM : ViewModel<CustomerVMDescriptor>, IHasSourceObject<Customer> {
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

      Customer IHasSourceObject<Customer>.Source {
         get { return CustomerSource; }
         set { CustomerSource = value; }
      }
   }

   public sealed class CustomerVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Title { get; set; }
   }
}
