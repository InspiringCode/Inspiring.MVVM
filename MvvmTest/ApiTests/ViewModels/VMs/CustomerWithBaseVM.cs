namespace Inspiring.MvvmTest.ApiTests.ViewModels.VMs {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public sealed class CustomerWithBaseVM : BaseVM, IHasSourceObject<Customer> {

      public new static readonly CustomerWithBaseVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .Inherits(BaseVM.ClassDescriptor)
         .OfType<CustomerWithBaseVMDescriptor>()
         .For<CustomerWithBaseVM>()
         .WithProperties((d, c) => {
            var cust = c.GetPropertyBuilder(x => x.CustomerSource);

            d.Title = cust.Property.MapsTo(x => x.Title);
         })
         .WithValidators(b => {
            b.EnableParentValidation(x => x.Title);
         })
         .Build();


      public CustomerWithBaseVM()
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

   public sealed class CustomerWithBaseVMDescriptor : BaseVMDescriptor {
      public IVMPropertyDescriptor<string> Title { get; set; }
   }
}
