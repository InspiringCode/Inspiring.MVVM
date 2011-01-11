namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;

   internal sealed class UserVM : ViewModel<UserVMDescriptor>, IHasSourceObject<User> {
      public UserVM(UserVMDescriptor descriptor, IServiceLocator serviceLocator = null)
         : base(descriptor, serviceLocator) {
      }

      public User UserSource { get; private set; }

      public void InitializeFrom(User source) {
         UserSource = source;
      }

      public string Name {
         get { return GetValue(Descriptor.Name); }
      }

      public MultiSelectionVM<Group, GroupVM> Groups {
         get { return GetValue(Descriptor.Groups); }
      }

      public SingleSelectionVM<Department, DepartmentVM> Department {
         get { return GetValue(Descriptor.Department); }
      }

      public void UpdateGroupsFromSource() {
         CopyFromSource(Descriptor.Groups);
      }

      User IHasSourceObject<User>.Source {
         get { return UserSource; }
         set { UserSource = value; }
      }
   }

   internal sealed class UserVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Name { get; set; }
      public IVMPropertyDescriptor<MultiSelectionVM<Group, GroupVM>> Groups { get; set; }
      public IVMPropertyDescriptor<SingleSelectionVM<Department, DepartmentVM>> Department { get; set; }
   }
}
