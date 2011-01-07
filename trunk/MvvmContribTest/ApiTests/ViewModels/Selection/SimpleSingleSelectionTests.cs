namespace Inspiring.MvvmContribTest.ApiTests.ViewModels.Selection {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.SingleSelection;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class SimpleSingleSelectionTests {
      private Department Group1 { get; set; }
      private Department Group2 { get; set; }
      private Department InactiveGroup { get; set; }

      [TestInitialize]
      public void Setup() {
         Group1 = new Department("Department 1");
         Group2 = new Department("Department 2");
         InactiveGroup = new Department("Inactive Department", isActive: false);
      }

      [TestMethod]
      public void AllItems_WithFilter_ReturnsFilteredItems() {
         Department firstDepartment = new Department("First Department");
         Department inactiveDepartment = new Department("Inactive Department", isActive: false);

         UserVM vm = new UserVM(new[] { firstDepartment, inactiveDepartment });

         CollectionAssert.AreEqual(
            new[] { firstDepartment },
            vm.Department.AllItems.Select(x => x.Source).ToArray()
         );
      }

      [TestMethod]
      public void ItemVMs_HaveCorrectCaptions() {
         Department department = new Department("First department");

         UserVM vm = new UserVM(new[] { department });

         SelectionItemVM<Department> groupVM = vm.Department.AllItems.Single();

         Assert.AreEqual(department.Name, groupVM.Caption);
      }

      internal sealed class UserVM : ViewModel<UserVMDescriptor>, ICanInitializeFrom<User> {
         public static UserVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<UserVMDescriptor>()
            .For<UserVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               var s = c.GetPropertyBuilder(x => x.UserSource);

               d.Name = s.Property.MapsTo(x => x.Name);
               d.Department = v
                  .SingleSelection(x => x.UserSource.Department)
                  .WithItems(x => x.AllSourceDepartments)
                  .WithFilter(x => x.IsActive)
                  .WithCaption(x => x.Name);
            })
            .Build();

         public UserVM(IEnumerable<Department> allSourceDepartments = null)
            : base(ClassDescriptor) {
            AllSourceDepartments = allSourceDepartments;
            UserSource = new User();
         }

         public User UserSource { get; private set; }

         public IEnumerable<Department> AllSourceDepartments { get; set; }

         public void InitializeFrom(User source) {
            UserSource = source;
         }

         public string Name {
            get { return GetValue(Descriptor.Name); }
         }

         public SingleSelectionVM<Department> Department {
            get { return GetValue(Descriptor.Department); }
         }
      }

      internal sealed class UserVMDescriptor : VMDescriptor {
         public IVMProperty<string> Name { get; set; }
         public IVMProperty<SingleSelectionVM<Department>> Department { get; set; }
      }
   }
}