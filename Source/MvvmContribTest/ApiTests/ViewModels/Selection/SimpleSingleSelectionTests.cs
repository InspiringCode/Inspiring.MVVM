﻿namespace Inspiring.MvvmContribTest.ApiTests.ViewModels.Selection {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class SimpleSingleSelectionTests : TestBase {
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

      [TestMethod]
      public void UpdateFromSource() {
         UserVM vm = new UserVM();
         vm.Refresh(UserVM.ClassDescriptor.Department);
      }

      [TestMethod]
      public void EnableValidations_EnablesParentValidation() {
         Department department = new Department("First department");

         UserVM vm = new UserVM(new[] { department });

         var lazyLoadTrigger = vm.Department.SelectedItem;

         vm.Revalidate(ValidationScope.SelfAndAllDescendants);

         Assert.IsFalse(vm.IsValid);
      }

      [TestMethod]
      public void EnableUndo_EnablesUndoSetValueBehavior() {
         UserVM vm = new UserVM();

         IViewModel department = vm.GetValue(x => x.Department);

         foreach (var property in department.Descriptor.Properties) {
            bool found = false;
            for (IBehavior b = property.Behaviors; b != null; b = b.Successor) {
               if (b.GetType().Name.Contains("UndoSetValueBehavior") ||
                   b.GetType().Name.Contains("UndoCollectionModifcationBehavior")) {
                  found = true;
                  break;
               }
            }
            Assert.IsTrue(found);
         }
      }

      internal sealed class UserVM : DefaultViewModelWithSourceBase<UserVMDescriptor, User> {
         public static UserVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<UserVMDescriptor>()
            .For<UserVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               var s = c.GetPropertyBuilder(x => x.Source);

               d.Name = s.Property.MapsTo(x => x.Name);
               d.Department = v
                  .SingleSelection(x => x.Source.Department)
                  .EnableValidations()
                  .EnableUndo()
                  .WithItems(x => x.AllSourceDepartments)
                  .WithFilter((vm, x) => x.IsActive)
                  .WithCaption(x => x.Name);
            })
            .WithValidators(b => {
               b.ValidateDescendant(x => x.Department)
                  .Check(x => x.SelectedItem)
                  .HasValue(string.Empty);
            })
            .Build();

         public UserVM(IEnumerable<Department> allSourceDepartments = null)
            : base(ClassDescriptor) {
            AllSourceDepartments = allSourceDepartments;
            SetSource(new User());
         }

         public IEnumerable<Department> AllSourceDepartments { get; set; }

         public string Name {
            get { return GetValue(Descriptor.Name); }
         }

         public SingleSelectionVM<Department> Department {
            get { return GetValue(Descriptor.Department); }
         }

         public void Refresh(IVMPropertyDescriptor property) {
            base.Refresh(property);
         }

         public new void Revalidate(ValidationScope scope) {
            base.Revalidate(scope);
         }

         public IRollbackPoint GetRollbackPoint() {
            return Kernel.UndoManager.GetRollbackPoint();
         }

         public void RollbackTo(IRollbackPoint point) {
            Kernel.UndoManager.RollbackTo(point);
         }
      }

      internal sealed class UserVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<SingleSelectionVM<Department>> Department { get; set; }
      }
   }
}