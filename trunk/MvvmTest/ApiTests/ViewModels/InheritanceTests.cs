namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class InheritanceTests {
      [TestMethod]
      public void TestMethod1() {

      }

      public class PersonVM : ViewModel<PersonVMDescriptor> {
         public static readonly PersonVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<PersonVMDescriptor>()
            .For<PersonVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.FirstName = v.Property.Of<string>();
               d.LastName = v.Property.Of<string>();
            })
            .Build();

         public PersonVM()
            : this(ClassDescriptor) {
         }

         protected PersonVM(PersonVMDescriptor descriptor)
            : base(descriptor) {
         }
      }

      public sealed class EmployeeVM : PersonVM {
         public new static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .Inherits(PersonVM.ClassDescriptor)
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.PersonalNumber = v.Property.Of<decimal>();
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
         }

         protected new EmployeeVMDescriptor Descriptor {
            get { return (EmployeeVMDescriptor)base.Descriptor; }
         }

         public void Foobar() {
         }
      }


      public class PersonVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> FirstName { get; set; }
         public IVMPropertyDescriptor<string> LastName { get; set; }
      }

      public sealed class EmployeeVMDescriptor : PersonVMDescriptor {
         public IVMPropertyDescriptor<decimal> PersonalNumber { get; set; }
      }
   }
}