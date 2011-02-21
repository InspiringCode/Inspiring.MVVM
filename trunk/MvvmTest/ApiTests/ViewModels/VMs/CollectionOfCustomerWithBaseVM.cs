namespace Inspiring.MvvmTest.ApiTests.ViewModels.VMs {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public sealed class CollectionOfCustomerWithBaseVM : ViewModel<CollectionOfCustomerWithBaseVMDescriptor>, IHasSourceObject<IEnumerable<Customer>> {

      public static readonly CollectionOfCustomerWithBaseVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<CollectionOfCustomerWithBaseVMDescriptor>()
         .For<CollectionOfCustomerWithBaseVM>()
         .WithProperties((d, b) => {
            var vm = b.GetPropertyBuilder();

            d.Customers = vm.Collection
               .Wraps(x => x.CustomersSource)
               .With<CustomerWithBaseVM>(CustomerWithBaseVM.ClassDescriptor);
            d.Children = vm.Collection.Of<BaseVM>(BaseVM.ClassDescriptor);
         })
         .WithValidators(b => {
            b.CheckCollection<CustomerWithBaseVMDescriptor, string>(x => x.Customers, x => x.Title)
               .IsUnique("Es exisitieren zwei Kunden mit derselben Bezeichnung!");
         })
         .Build();

      public CollectionOfCustomerWithBaseVM()
         : base(ClassDescriptor) {

      }
      public void InitializeFrom(IEnumerable<Customer> source) {
         CustomersSource = source;
         Revalidate(ValidationScope.FullSubtree);
      }

      public IEnumerable<Customer> CustomersSource { get; private set; }


      IEnumerable<Customer> IHasSourceObject<IEnumerable<Customer>>.Source {
         get { return CustomersSource; }
         set { CustomersSource = value; }
      }

      public IVMCollection<BaseVM> Children {
         get { return GetValue(ClassDescriptor.Children); }
         set { SetValue(ClassDescriptor.Children, value); }
      }

      public IVMCollection<CustomerWithBaseVM> Customers {
         get { return GetValue(Descriptor.Customers); }
      }
   }

   public sealed class CollectionOfCustomerWithBaseVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<IVMCollection<CustomerWithBaseVM>> Customers { get; set; }
      public IVMPropertyDescriptor<IVMCollection<BaseVM>> Children { get; set; }

   }
}
