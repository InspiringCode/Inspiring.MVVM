namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ApiTests.ViewModels.Domain;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ViewModelPropertyTests : TestBase {
      private RootVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new RootVM();
      }

      [TestMethod]
      public void GetValueOfDelegatingViewModelProperty_WhenGetterDelegateReturnsNull_ReturnsNull() {
         var vm = CreateVM(x => x.VM.DelegatesTo(v => (CustomerVM)null));
         Assert.IsNull(vm.Customer);
      }

      [TestMethod]
      public void GetValueOfWrapperProperty_WhenSourceValueIsNull_ReturnsNull() {
         var vm = CreateVM(x => x.VM.Wraps(v => (Customer)null, (v, val) => { }).With<CustomerVM>());
         Assert.IsNull(vm.Customer);
      }

      [TestMethod]
      public void SetValueOfWrapperProperty_ToNull_SetsSourceToNull() {
         VM.WrapperPropertySource = new ChildSource();
         VM.Load(x => x.WrapperProperty);

         VM.SetValue(x => x.WrapperProperty, null);
         Assert.IsNull(VM.WrapperPropertySource);
      }

      private ProjectVM CreateVM(
         Func<IVMPropertyBuilder<ProjectVM>, IVMPropertyDescriptor<CustomerVM>> propertyDefinitionAction
      ) {
         var descriptor = VMDescriptorBuilder
            .OfType<ProjectVMDescriptor>()
            .For<ProjectVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();

               d.Title = v.Property.Of<string>();
               d.Customer = propertyDefinitionAction(v);
            })
            .Build();

         return new ProjectVM(descriptor);
      }

      private sealed class RootVM : TestViewModel<RootVMDescriptor> {
         public static readonly RootVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<RootVMDescriptor>()
            .For<RootVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.InstanceProperty = v.VM.Of<ChildVM>();
               d.WrapperProperty = v.VM.Wraps(x => x.WrapperPropertySource).With<ChildVM>();
               d.DelegateProperty = v.VM.DelegatesTo(
                  x => x.DelegatePropertyResult,
                  (x, val) => x.DelegatePropertyResult = val
               );
            })
            .Build();

         public RootVM()
            : base(ClassDescriptor) {
         }

         public ChildSource WrapperPropertySource { get; set; }
         public ChildVM DelegatePropertyResult { get; set; }
      }

      private sealed class RootVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ChildVM> InstanceProperty { get; set; }
         public IVMPropertyDescriptor<ChildVM> WrapperProperty { get; set; }
         public IVMPropertyDescriptor<ChildVM> DelegateProperty { get; set; }
      }

      protected class ChildSource {
      }

      protected class ChildVM : DefaultViewModelWithSourceBase<ChildVMDescriptor, ChildSource> {
         public static readonly ChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ChildVMDescriptor>()
            .For<ChildVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
            })
            .Build();

         public ChildVM()
            : base(ClassDescriptor) {
         }
      }

      protected class ChildVMDescriptor : VMDescriptor {
      }


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
         public void Refresh_RecreatesViewModel() {
            Source.Customer = new Customer();
            VM.RefreshCustomer();
            Assert.AreEqual(Source.Customer, VM.Customer.CustomerSource);
         }

         [TestMethod]
         public void GetValue_WhenSourceIsNull_ReturnsNull() {
            var vm = CreateVM();
            vm.InitializeFrom(new Project { Customer = null });
            Assert.IsNull(vm.Customer);
         }

         protected abstract ProjectVM CreateVM();
      }

      [TestClass]
      public class MappedRootPropertyFactoryTests : BaseTests {
         protected override ProjectVM CreateVM() {
            var descriptor = VMDescriptorBuilder
               .OfType<ProjectVMDescriptor>()
               .For<ProjectVM>()
               .WithProperties((d, c) => {
                  var v = c.GetPropertyBuilder();

                  d.Title = v.Property.Of<string>();
                  d.Customer = v.VM.Wraps(x => x.Source.Customer).With<CustomerVM>();
               })
               .Build();

            return new ProjectVM(descriptor);
         }
      }

      [TestClass]
      public class CalculatedRootPropertyFactoryTests : BaseTests {
         protected override ProjectVM CreateVM() {
            var descriptor = VMDescriptorBuilder
               .OfType<ProjectVMDescriptor>()
               .For<ProjectVM>()
               .WithProperties((d, c) => {
                  var v = c.GetPropertyBuilder();

                  d.Title = v.Property.Of<string>();
                  d.Customer = v.VM.Wraps(
                     x => x.Source.Customer,
                     (x, val) => x.Source.Customer = val
                  ).With<CustomerVM>();
               })
               .Build();

            return new ProjectVM(descriptor);
         }
      }

      [TestClass]
      public class MappedRelativePropertyFactoryTests : BaseTests {
         protected override ProjectVM CreateVM() {
            var descriptor = VMDescriptorBuilder
               .OfType<ProjectVMDescriptor>().For<ProjectVM>()
               .WithProperties((d, c) => {
                  var p = c.GetPropertyBuilder(x => x.Source);

                  d.Title = p.Property.Of<string>();
                  d.Customer = p.VM.Wraps(x => x.Customer).With<CustomerVM>();
               })
               .Build();

            return new ProjectVM(descriptor);
         }
      }

      [TestClass]
      public class CalculatedRelativePropertyFactoryTests : BaseTests {
         protected override ProjectVM CreateVM() {
            var descriptor = VMDescriptorBuilder
               .OfType<ProjectVMDescriptor>().For<ProjectVM>()
               .WithProperties((d, c) => {
                  var p = c.GetPropertyBuilder(x => x.Source);

                  d.Title = p.Property.Of<string>();
                  d.Customer = p.VM.Wraps(
                     x => x.Customer,
                     (x, val) => x.Customer = val
                  ).With<CustomerVM>();
               })
               .Build();

            return new ProjectVM(descriptor);
         }
      }
   }
}