namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System.ComponentModel;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class TypedBindingListTests {
      [TestMethod]
      public void GetItemProperties_WhenListAccessorsIsNull_ReturnsPropertiesDefinedByItemTypeClassDescriptor() {
         var list = new TypedBindingList<EmployeeVM>();
         var actualProperties = list.GetItemProperties(listAccessors: null);

         var expectedProperties = EmployeeVM.ClassDescriptor.GetPropertyDescriptors();
         CollectionAssert.AreEqual(expectedProperties, actualProperties);
      }

      [TestMethod]
      public void GetItemProperties_WhenListAccessorsHasDescriptors_ReturnsPropertiesDefinedByClassDescriptorOfLastListAccessorType() {
         var list = new TypedBindingList<EmployeeVM>();

         PropertyDescriptorCollection employeePropertyDescriptors = EmployeeVM
            .ClassDescriptor
            .GetPropertyDescriptors();

         PropertyDescriptorCollection addressPropertyDescriptors = AddressVM
            .ClassDescriptor
            .GetPropertyDescriptors();

         PropertyDescriptor managerDescriptor = employeePropertyDescriptors["Manager"];
         PropertyDescriptor addressDescriptor = employeePropertyDescriptors["Address"];

         var listAccessors = new[] { managerDescriptor, addressDescriptor };
         var actualProperties = list.GetItemProperties(listAccessors);

         var expectedProperties = AddressVM.ClassDescriptor.GetPropertyDescriptors();
         CollectionAssert.AreEqual(expectedProperties, actualProperties);
      }

      public sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         [ClassDescriptor]
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Name = v.Property.Of<string>();
               d.Manager = v.Property.Of<EmployeeVM>();
               d.Address = v.VM.Of<AddressVM>();
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
         }
      }

      public sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<AddressVM> Address { get; set; }
         public IVMPropertyDescriptor<EmployeeVM> Manager { get; set; }
      }

      public sealed class AddressVM : ViewModel<AddressVMDescriptor> {
         [ClassDescriptor]
         public static readonly AddressVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<AddressVMDescriptor>()
            .For<AddressVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Street = v.Property.Of<string>();
            })
            .Build();

         public AddressVM()
            : base(ClassDescriptor) {
         }
      }

      public sealed class AddressVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Street { get; set; }
      }
   }
}