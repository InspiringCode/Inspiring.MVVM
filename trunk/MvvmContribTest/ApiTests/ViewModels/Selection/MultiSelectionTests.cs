namespace Inspiring.MvvmContribTest.ApiTests.ViewModels.Selection {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.Stubs;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class MultiSelectionTests : TestBase {

      private Group Group1 { get; set; }
      private Group Group2 { get; set; }
      private Group InactiveGroup { get; set; }

      [TestInitialize]
      public void Setup() {
         Group1 = new Group("Group 1");
         Group2 = new Group("Group 2");
         InactiveGroup = new Group("Group 3", isActive: false);
      }

      [TestMethod]
      public void AllItems_WithoutFilter_ReturnsAllSourceItems() {
         SetupServiceLocator(new[] { Group1, InactiveGroup });

         var vm = CreateUserVM();

         CollectionAssert.AreEquivalent(
            new[] { Group1, InactiveGroup },
            vm.Groups.AllItems.Select(x => x.GroupSource).ToArray()
         );
      }

      [TestMethod]
      public void AllItems_WithFilter_ReturnsFilteredItems() {
         SetupServiceLocator(new[] { Group1, InactiveGroup });

         var vm = CreateUserVM(x => x.IsActive);

         CollectionAssert.AreEquivalent(
            new[] { Group1 },
            vm.Groups.AllItems.Select(x => x.GroupSource).ToArray()
         );
      }

      private UserVM CreateUserVM(params Group[] groups) {
         UserVMDescriptor descriptor = VMDescriptorBuilder
            .For<UserVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyBuilder();

               return new UserVMDescriptor {
                  Name = v.Property.MapsTo(x => x.UserSource.Name),
                  Groups = v.MultiSelection(x => x.UserSource.Groups).Of<GroupVM>(GroupVM.Descriptor)
               };
            })
            .Build();

         var vm = new UserVM(descriptor);
         vm.InitializeFrom(new User(groups));
         return vm;
      }

      private UserVM CreateUserVM(Func<Group, bool> filter, params Group[] groups) {
         UserVMDescriptor descriptor = VMDescriptorBuilder
            .For<UserVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyBuilder();

               return new UserVMDescriptor {
                  Name = v.Property.MapsTo(x => x.UserSource.Name),
                  Groups = v
                     .MultiSelection(x => x.UserSource.Groups)
                     .WithFilter(x => x.IsActive)
                     .Of<GroupVM>(GroupVM.Descriptor)
               };
            })
            .Build();

         var vm = new UserVM(descriptor);
         vm.InitializeFrom(new User(groups));
         return vm;
      }

      private void SetupServiceLocator(IEnumerable<Group> sourceItemsToReturn) {
         var loc = new ServiceLocatorStub();
         loc.Register<IEnumerable<Group>>(sourceItemsToReturn);
         ServiceLocator.SetServiceLocator(loc);
      }
   }
}