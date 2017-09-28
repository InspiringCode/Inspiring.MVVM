namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.Views;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class InheritanceTests : TestBase {
      public void TestMethod1() {
         //EmployeeListView view = null;
         //ViewBinder.BindVM(view, b => {
         //   b.Collection<EmployeeVMDescriptor>(x => x.Employees);
         //});

         //EmployeeView employeeView = null;

         //ViewBinder.BindVM<EmployeeVMDescriptor>(employeeView, b => {
         //   b.Property(x => x.PersonalNumber);
         //});
      }

      [TestMethod]
      public void CollectionValidation_DefinedForDerivedItem_Success() {
         var listVM = new EmployeeListVM();
         listVM.Initialize();
      }

      [TestMethod]
      public void SubclassOfClassWithGenericDescriptor_CanBeCreated() {
         var vm = new StateEntryVM();
         vm.SetValue(x => x.Caption, ArbitraryString);
         vm.SetValue(x => x.Description, AnotherArbitraryString);

         Assert.AreEqual(ArbitraryString, vm.GetValue(x => x.Caption));
         Assert.AreEqual(AnotherArbitraryString, vm.GetValue(x => x.Description));
      }

      [TestMethod]
      public void AllowAssignmentOfSubClassToBaseClassVariable() {
         //ListOfValueEntryVM<ListOfValueEntryVMDescriptor, State> foo = null;
         //StateEntryVM bar = null;

         // TODO?
         //foo = bar;
         //bar = foo;
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
         public static readonly EmployeeListVMDescriptor ClassDescriptor = VMDescriptorBuilder
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

         public EmployeeListVM()
            : base(ClassDescriptor) {
         }

         private IVMCollection<EmployeeVM> Employees {
            get { return GetValue(Descriptor.Employees); }
         }

         public void Initialize() {
            Employees.Add(new EmployeeVM());
            Revalidate(ValidationScope.SelfAndAllDescendants);
         }
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
            .WithValidators(b => {
               b.EnableParentValidation(x => x.PersonalNumber);
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
         }

         private new EmployeeVMDescriptor Descriptor {
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

      public class ListOfValueEntryVM<TDescriptor, TSourceObject> :
         DefaultViewModelWithSourceBase<TDescriptor, TSourceObject>
         where TDescriptor : ListOfValueEntryVMDescriptor {

         //public static ListOfValueEntryVMDescriptor BaseDescriptor = null;
         public static ListOfValueEntryVMDescriptor BaseDescriptor = VMDescriptorBuilder
            .OfType<ListOfValueEntryVMDescriptor>()
            .For<ListOfValueEntryVM<TDescriptor, TSourceObject>>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Caption = v.Property.Of<string>();
            })
            .Build();

         public ListOfValueEntryVM(TDescriptor descriptor)
            : base(descriptor) {
         }
      }

      public class ListOfValueEntryVMDescriptor : PersonVMDescriptor {
         public IVMPropertyDescriptor<string> Caption { get; set; }
      }

      public sealed class StateEntryVM : ListOfValueEntryVM<StateEntryVMDescriptor, State> {
         public static readonly StateEntryVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .Inherits(BaseDescriptor)
            .OfType<StateEntryVMDescriptor>()
            .For<StateEntryVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Description = v.Property.Of<string>();
            })
            .Build();

         public StateEntryVM()
            : base(ClassDescriptor) {
         }
      }

      public sealed class StateEntryVMDescriptor : ListOfValueEntryVMDescriptor {
         public IVMPropertyDescriptor<string> Description { get; set; }
      }

      public class State {
      }
   }
}