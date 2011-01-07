namespace Inspiring.MvvmContribTest.ApiTests.ViewModels.Selection {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.SingleSelection;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class SimpleMultiSelectionTests {
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

      internal sealed class UserVM : ViewModel<UserVMDescriptor>, ICanInitializeFrom<User> {
         public static UserVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<UserVMDescriptor>()
            .For<UserVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               var s = c.GetPropertyBuilder(x => x.UserSource);

               d.Name = s.Property.MapsTo(x => x.Name);
               d.Groups = v
                  .MultiSelection(x => x.UserSource.Groups)
                  .WithItems(x => x.AllSourceGroups)
                  .WithFilter(x => x.IsActive)
                  .WithCaption(x => x.Name);
            })
            .Build();

         public UserVM(IEnumerable<Group> allSourceGroups)
            : base(ClassDescriptor) {
            AllSourceGroups = allSourceGroups;
            UserSource = new User();
         }

         public User UserSource { get; private set; }

         public IEnumerable<Group> AllSourceGroups { get; set; }

         public void InitializeFrom(User source) {
            UserSource = source;
         }

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
         public IVMProperty<string> Name { get; set; }
         public IVMProperty<MultiSelectionVM<Group>> Groups { get; set; }
         public IVMProperty<SingleSelectionVM<Department>> Department { get; set; }
      }
   }
}