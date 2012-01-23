namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest;

   internal sealed class UserVM : TestViewModel<UserVMDescriptor> {
      public UserVM(UserVMDescriptor descriptor, IServiceLocator serviceLocator = null)
         : base(descriptor, serviceLocator: serviceLocator) {
      }

      public User Source { get; set; }

      public string Name {
         get { return GetValue(Descriptor.Name); }
      }

      public MultiSelectionVM<Group, GroupVM> Groups {
         get { return GetValue(Descriptor.Groups); }
      }

      public SingleSelectionVM<Department, DepartmentVM> Department {
         get { return GetValue(Descriptor.Department); }
      }

      public void RefreshGroups() {
         Refresh(Descriptor.Groups);
      }

      public void RefreshDepartment() {
         Refresh(Descriptor.Department);
      }

      public void InitializeFrom(User source) {
         Source = source;
         Revalidate(ValidationScope.SelfAndLoadedDescendants);
      }
   }

   internal sealed class UserVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Name { get; set; }
      public IVMPropertyDescriptor<MultiSelectionVM<Group, GroupVM>> Groups { get; set; }
      public IVMPropertyDescriptor<SingleSelectionVM<Department, DepartmentVM>> Department { get; set; }
   }
}
