namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.SingleSelection;

   internal sealed class UserVM : ViewModel<UserVMDescriptor>, ICanInitializeFrom<User> {
      public UserVM(UserVMDescriptor descriptor, IServiceLocator serviceLocator = null)
         : base(descriptor, serviceLocator) {
      }

      public User UserSource { get; private set; }

      public void InitializeFrom(User source) {
         UserSource = source;
      }

      public string Name {
         get { return GetValue(DescriptorBase.Name); }
      }

      public MultiSelectionVM<Group, GroupVM> Groups {
         get { return GetValue(DescriptorBase.Groups); }
      }

      public SingleSelectionVM<Department, DepartmentVM> Department {
         get { return GetValue(DescriptorBase.Department); }
      }
   }

   internal sealed class UserVMDescriptor : VMDescriptor {
      public VMProperty<string> Name { get; set; }
      public VMProperty<MultiSelectionVM<Group, GroupVM>> Groups { get; set; }
      public VMProperty<SingleSelectionVM<Department, DepartmentVM>> Department { get; set; }
   }
}
