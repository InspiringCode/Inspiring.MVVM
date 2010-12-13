﻿namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;

   public sealed class CustomerVM : ViewModel<CustomerVMDescriptor>, ICanInitializeFrom<Customer> {
      public static readonly CustomerVMDescriptor Descriptor = VMDescriptorBuilder
         .For<CustomerVM>()
         .CreateDescriptor(c => {
            var cust = c.GetPropertyFactory(x => x.CustomerSource);

            return new CustomerVMDescriptor {
               Title = cust.Mapped(x => x.Title).Property()
            };
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
