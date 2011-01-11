namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;

   internal sealed class GroupVM : ViewModel<GroupVMDescriptor>, IHasSourceObject<Group> {
      public static readonly GroupVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<GroupVMDescriptor>()
         .For<GroupVM>()
         .WithProperties((d, c) => {
            var vm = c.GetPropertyBuilder();
            var k = c.GetPropertyBuilder(x => x.GroupSource);

            d.Name = k.Property.MapsTo(x => x.Name);
         })
         .Build();

      public GroupVM()
         : base(ClassDescriptor) {
      }

      public Group GroupSource { get; private set; }

      public string Name {
         get { return GetValue(Descriptor.Name); }
      }

      public void InitializeFrom(Group source) {
         GroupSource = source;
      }

      Group IHasSourceObject<Group>.Source {
         get { return GroupSource; }
         set { GroupSource = value; }
      }
   }

   public sealed class GroupVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Name { get; set; }
   }
}
