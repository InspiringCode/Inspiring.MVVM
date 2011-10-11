namespace Inspiring.MvvmContribTest.ApiTests.ViewModels.Selection {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class SimpleMultiSelectionTests : TestBase {
      [TestMethod]
      public void AllItems_WithFilter_ReturnsFilteredItems() {
         Group firstGroup = new Group("First group");
         Group inactiveGroup = new Group("Inactive group", isActive: false);

         UserVM vm = new UserVM(new[] { firstGroup, inactiveGroup });

         CollectionAssert.AreEqual(
            new[] { firstGroup },
            vm.Groups.AllItems.Select(x => x.Source).ToArray()
         );
      }

      [TestMethod]
      public void ItemVMs_HaveCorrectCaptions() {
         Group group = new Group("First group");

         UserVM vm = new UserVM(new[] { group });

         SelectionItemVM<Group> groupVM = vm.Groups.AllItems.Single();

         Assert.AreEqual(group.Name, groupVM.Caption);
      }

      [TestMethod]
      public void EnableUndo_EnablesUndoSetValueBehavior() {
         UserVM vm = new UserVM(null);

         IViewModel department = vm.GetValue(x => x.Groups);

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
               d.Groups = v
                  .MultiSelection(x => x.Source.Groups)
                  .EnableUndo()
                  .WithItems(x => x.AllSourceGroups)
                  .WithFilter((vm, x) => x.IsActive)
                  .WithCaption(x => x.Name);
            })
            .Build();

         public UserVM(IEnumerable<Group> allSourceGroups)
            : base(ClassDescriptor) {
            AllSourceGroups = allSourceGroups;
            SetSource(new User());
         }

         public IEnumerable<Group> AllSourceGroups { get; set; }

         public string Name {
            get { return GetValue(Descriptor.Name); }
         }

         public MultiSelectionVM<Group> Groups {
            get { return GetValue(Descriptor.Groups); }
         }

         public SingleSelectionVM<Department> Department {
            get { return GetValue(Descriptor.Department); }
         }
      }

      internal sealed class UserVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<MultiSelectionVM<Group>> Groups { get; set; }
         public IVMPropertyDescriptor<SingleSelectionVM<Department>> Department { get; set; }
      }
   }
}