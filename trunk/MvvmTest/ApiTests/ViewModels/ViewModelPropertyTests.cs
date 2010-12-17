namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ViewModelPropertyTests {

      [TestClass]
      public abstract class BaseTests {
         private ProjectVM VM { get; set; }
         private Project Source { get; set; }

         [TestInitialize]
         public void Setup() {
            Source = new Project(null, new Customer());
            VM = CreateVM();
            VM.InitializeFrom(Source);
         }

         [TestMethod]
         public void GetValue_ReturnsInitializedViewModel() {
            CustomerVM vm = VM.Customer;
            Assert.IsNotNull(vm);
            Assert.AreEqual(Source.Customer, vm.CustomerSource);
         }

         [TestMethod]
         public void GetValue_Always_ReturnsSameInstance() {
            var first = VM.Customer;
            var second = VM.Customer;

            Assert.AreSame(first, second);
         }

         [TestMethod]
         public void SetValue_UpdatesSourceObject() {
            var newCustomer = new Customer();
            var newCustomerVM = new CustomerVM();
            newCustomerVM.InitializeFrom(newCustomer);

            VM.Customer = newCustomerVM;

            Assert.AreEqual(newCustomer, Source.Customer);
         }

         [TestMethod]
         public void UpdateFromSource_RecreatesViewModel() {
            Source.Customer = new Customer();
            VM.UpdateCustomerFromSource();
            Assert.AreEqual(Source.Customer, VM.Customer.CustomerSource);
         }

         protected abstract ProjectVM CreateVM();
      }

      [TestClass]
      public class MappedRootPropertyFactoryTests : BaseTests {
         protected override ProjectVM CreateVM() {
            var descriptor = VMDescriptorBuilder
               .For<ProjectVM>()
               .CreateDescriptor(c => {
                  var v = c.GetPropertyBuilder();

                  return new ProjectVMDescriptor {
                     Title = v.Local.Property<string>(),
                     Customer = v.Mapped(x => x.ProjectSource.Customer).VM<CustomerVM>()
                  };
               })
               .Build();

            return new ProjectVM(descriptor);
         }
      }

      [TestClass]
      public class CalculatedRootPropertyFactoryTests : BaseTests {
         protected override ProjectVM CreateVM() {
            var descriptor = VMDescriptorBuilder
               .For<ProjectVM>()
               .CreateDescriptor(c => {
                  var v = c.GetPropertyBuilder();

                  return new ProjectVMDescriptor {
                     Title = v.Local.Property<string>(),
                     Customer = v.Calculated(
                        x => x.ProjectSource.Customer,
                        (x, val) => x.ProjectSource.Customer = val
                     ).VM<CustomerVM>()
                  };
               })
               .Build();

            return new ProjectVM(descriptor);
         }
      }

      [TestClass]
      public class MappedRelativePropertyFactoryTests : BaseTests {
         protected override ProjectVM CreateVM() {
            var descriptor = VMDescriptorBuilder
               .For<ProjectVM>()
               .CreateDescriptor(c => {
                  var p = c.GetPropertyBuilder(x => x.ProjectSource);

                  return new ProjectVMDescriptor {
                     Title = p.Local.Property<string>(),
                     Customer = p.Mapped(x => x.Customer).VM<CustomerVM>()
                  };
               })
               .Build();

            return new ProjectVM(descriptor);
         }
      }

      [TestClass]
      public class CalculatedRelativePropertyFactoryTests : BaseTests {
         protected override ProjectVM CreateVM() {
            var descriptor = VMDescriptorBuilder
               .For<ProjectVM>()
               .CreateDescriptor(c => {
                  var p = c.GetPropertyBuilder(x => x.ProjectSource);

                  return new ProjectVMDescriptor {
                     Title = p.Local.Property<string>(),
                     Customer = p.Calculated(
                        x => x.Customer,
                        (x, val) => x.Customer = val
                     ).VM<CustomerVM>()
                  };
               })
               .Build();

            return new ProjectVM(descriptor);
         }
      }
   }
}