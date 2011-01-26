namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Inspiring.Mvvm.Views;

   [TestClass]
   public class InheritanceTests {
      public void TestMethod1() {
         EmployeeListView view = null;
         ViewBinder.BindVM(view, b => {
            b.Collection<EmployeeVMDescriptor>(x => x.Employees);
         });

         EmployeeView employeeView = null;

         ViewBinder.BindVM<EmployeeVMDescriptor>(employeeView, b => {
            b.Property(x => x.PersonalNumber);
         });
      }

      public class EmployeeListView : IView<EmployeeListVM> {
         public EmployeeListVM Model {
            get;
            set;
         }
      }

      public class EmployeeView : IView<EmployeeVM> {
         public EmployeeVM Model {
            get;
            set;
         }
      }

      public class EmployeeListVM : ViewModel<EmployeeListVMDescriptor> {
         public static readonly PersonVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeListVMDescriptor>()
            .For<EmployeeListVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Employees = v.Collection.Of<EmployeeVM>(EmployeeVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.CheckCollection<EmployeeVMDescriptor, string>(x => x.Employees, x => x.FirstName)
                  .IsUnique("Not unique");
            })
            .Build();
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

      public sealed class EmployeeListVMDescriptor : PersonVMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<EmployeeVM>> Employees { get; set; }
      }
   }
}