namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.MvvmTest.ViewModels;

   [TestClass]
   public class InheritanceWithGenericsTests : TestBase {
      [TestMethod]
      public void AccessProperty_OfBaseClass_Success() {
         EmployeeVM vm = new EmployeeVM();
         vm.FirstName = ArbitraryString;
         Assert.AreEqual(ArbitraryString, vm.FirstName);
      }

      [TestMethod]
      public void AccessProperty_OfSubClass_Success() {
         EmployeeVM vm = new EmployeeVM();
         vm.PersonalNumber = ArbitraryInt;
         Assert.AreEqual(ArbitraryInt, vm.PersonalNumber);
      }

      public static class PersonVM {
         public static PersonVMDescriptor ClassDescriptor = PersonVM<PersonVMDescriptor>.ClassDescriptor;
      }

      public class PersonVM<TDescriptor> :
         ViewModel<TDescriptor>
         where TDescriptor : PersonVMDescriptor, new() {

         public static readonly TDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<TDescriptor>()
            .For<PersonVM<TDescriptor>>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.FirstName = v.Property.Of<string>();
               d.LastName = v.Property.Of<string>();
            })
            .Build();

         public PersonVM()
            : this(ClassDescriptor) {
         }

         protected PersonVM(TDescriptor descriptor)
            : base(descriptor) {
         }

         public string FirstName {
            get { return GetValue(Descriptor.FirstName); }
            set { SetValue(Descriptor.FirstName, value); }
         }
      }

      public sealed class EmployeeVM : PersonVM<EmployeeVMDescriptor> {
         public new static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .Inherits(PersonVM<PersonVMDescriptor>.ClassDescriptor)
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.PersonalNumber = v.Property.Of<int>();
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
         }

         public int PersonalNumber {
            get { return GetValue(Descriptor.PersonalNumber); }
            set { SetValue(Descriptor.PersonalNumber, value); }
         }
      }


      public class PersonVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> FirstName { get; set; }
         public IVMPropertyDescriptor<string> LastName { get; set; }
      }

      public sealed class EmployeeVMDescriptor : PersonVMDescriptor {
         public IVMPropertyDescriptor<int> PersonalNumber { get; set; }
      }
   }
}