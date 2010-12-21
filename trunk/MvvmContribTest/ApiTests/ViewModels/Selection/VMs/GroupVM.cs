namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;

   internal sealed class GroupVM : ViewModel<GroupVMDescriptor>, ICanInitializeFrom<Group>, IVMCollectionItem<Group> {
      public static readonly GroupVMDescriptor Descriptor = VMDescriptorBuilder
         .For<GroupVM>()
         .CreateDescriptor(c => {
            var vm = c.GetPropertyBuilder();
            var k = c.GetPropertyBuilder(x => x.GroupSource);

            return new GroupVMDescriptor {
               Name = k.Property.MapsTo(x => x.Name)
            };
         })
         .Build();

      public GroupVM()
         : base() {
      }

      public Group GroupSource { get; private set; }

      public string Name {
         get { return GetValue(DescriptorBase.Name); }
      }

      public void InitializeFrom(Group source) {
         GroupSource = source;
      }

      Group IVMCollectionItem<Group>.Source {
         get { return GroupSource; }
      }
   }

   public sealed class GroupVMDescriptor : VMDescriptor {
      public VMProperty<string> Name { get; set; }
   }
}
