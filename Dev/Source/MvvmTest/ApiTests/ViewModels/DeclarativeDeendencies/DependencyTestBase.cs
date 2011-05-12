namespace Inspiring.MvvmTest.ApiTests.ViewModels.DeclarativeDeendencies {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public abstract class DependencyTestBase {

      internal sealed class EmployeeVM : TestViewModel<EmployeeVMDescriptor> {
         public EmployeeVM(
            Action<IVMDependencyBuilder<EmployeeVM, EmployeeVMDescriptor>> dependencyConfigurationAction,
            ProjectVMDescriptor projectVMDescriptor,
            bool useMockBehaviors
         )
            : this(
               dependencyConfigurationAction,
               projectVMDescriptor,
               new RefreshControllerBehaviorMock(),
               new DescendantValidationBehavioMock(),
               new ViewModelRevalidationBehaviorMock(),
               new PropertyRevalidationBehaviorMock(),
               useMockBehaviors
            ) {
         }

         private EmployeeVM(
               Action<IVMDependencyBuilder<EmployeeVM, EmployeeVMDescriptor>> dependencyConfigurationAction,
               ProjectVMDescriptor projectVMDescriptor,
               RefreshControllerBehaviorMock refreshControllerBehavior,
               DescendantValidationBehavioMock descendantValidationBehavior,
               ViewModelRevalidationBehaviorMock viewModelRevalidationBehavior,
               PropertyRevalidationBehaviorMock propertyRevalidationBehavior,
               bool useMockBehaviors
         )
            : base(
               CreateDescriptor(
                  dependencyConfigurationAction,
                  projectVMDescriptor,
                  refreshControllerBehavior,
                  descendantValidationBehavior,
                  viewModelRevalidationBehavior,
                  propertyRevalidationBehavior,
                  useMockBehaviors
                )
            ) {
            RefreshControllerBehaviorMock = refreshControllerBehavior;
            DescendantValidationBehavioMock = descendantValidationBehavior;
            ViewModelRevalidationBehaviorMock = viewModelRevalidationBehavior;
            PropertyRevalidationBehaviorMock = propertyRevalidationBehavior;
         }

         internal RefreshControllerBehaviorMock RefreshControllerBehaviorMock { get; private set; }
         internal DescendantValidationBehavioMock DescendantValidationBehavioMock { get; private set; }
         internal ViewModelRevalidationBehaviorMock ViewModelRevalidationBehaviorMock { get; private set; }
         internal PropertyRevalidationBehaviorMock PropertyRevalidationBehaviorMock { get; private set; }

         internal string Name {
            get { return GetValue(Descriptor.Name); }
            set { SetValue(Descriptor.Name, value); }
         }

         internal ProjectVM SelectedProject {
            get { return GetValue(Descriptor.SelectedProject); }
            set { SetValue(Descriptor.SelectedProject, value); }
         }

         internal IVMCollection<ProjectVM> Projects {
            get { return GetValue(Descriptor.Projects); }
            set { SetValue(Descriptor.Projects, value); }
         }

         private static EmployeeVMDescriptor CreateDescriptor(
            Action<IVMDependencyBuilder<EmployeeVM, EmployeeVMDescriptor>> dependencyConfigurationAction,
            ProjectVMDescriptor projectVMDescriptor,
            RefreshControllerBehaviorMock refreshControllerBehavior,
            DescendantValidationBehavioMock descendantValidationBehavior,
            ViewModelRevalidationBehaviorMock viewModelRevalidationBehavior,
            PropertyRevalidationBehaviorMock propertyRevalidationBehavior,
            bool useMockBehaviors
         ) {
            return VMDescriptorBuilder
               .OfType<EmployeeVMDescriptor>()
               .For<EmployeeVM>()
               .WithProperties((d, c) => {
                  var b = c.GetPropertyBuilder();

                  d.Name = b.Property.Of<string>();
                  d.Projects = b.Collection.Of<ProjectVM>(projectVMDescriptor);
                  d.SelectedProject = b.VM.Of<ProjectVM>();
               })
               .WithValidators(b => {
                  b.Check(x => x.Name)
                     .HasValue(String.Empty);
                  b.CheckCollection(x => x.Projects, x => x.Title)
                     .IsUnique(string.Empty);
               })
               .WithDependencies(
                  dependencyConfigurationAction
               )
               .WithBehaviors(b => {
                  if (useMockBehaviors) {
                     b.AddBehavior(refreshControllerBehavior);
                  }
               })
               .Build();
         }
      }

      internal sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<ProjectVM> SelectedProject { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
      }

      internal sealed class ProjectVM : TestViewModel<ProjectVMDescriptor> {
         public ProjectVM(bool useMockBehaviors)
            : this(
               new RefreshControllerBehaviorMock(),
               new DescendantValidationBehavioMock(),
               new ViewModelRevalidationBehaviorMock(),
               new PropertyRevalidationBehaviorMock(),
               useMockBehaviors
            ) {
         }

         private ProjectVM(
            RefreshControllerBehaviorMock refreshControllerBehavior,
            DescendantValidationBehavioMock descendantValidationBehavior,
            ViewModelRevalidationBehaviorMock viewModelRevalidationBehavior,
            PropertyRevalidationBehaviorMock propertyRevalidationBehavior,
            bool useMockBehaviors
         )
            : base(
               CreateDescriptor(
                  refreshControllerBehavior,
                  descendantValidationBehavior,
                  viewModelRevalidationBehavior,
                  propertyRevalidationBehavior,
                  useMockBehaviors
               )
            ) {
            RefreshControllerBehaviorMock = refreshControllerBehavior;
            DescendantValidationBehavioMock = descendantValidationBehavior;
            ViewModelRevalidationBehaviorMock = viewModelRevalidationBehavior;
            PropertyRevalidationBehaviorMock = propertyRevalidationBehavior;
         }

         internal RefreshControllerBehaviorMock RefreshControllerBehaviorMock { get; private set; }
         internal DescendantValidationBehavioMock DescendantValidationBehavioMock { get; private set; }
         internal ViewModelRevalidationBehaviorMock ViewModelRevalidationBehaviorMock { get; private set; }
         internal PropertyRevalidationBehaviorMock PropertyRevalidationBehaviorMock { get; private set; }

         internal string Title {
            get { return GetValue(Descriptor.Title); }
            set { SetValue(Descriptor.Title, value); }
         }

         internal CustomerVM Customer {
            get { return GetValue(Descriptor.Customer); }
            set { SetValue(Descriptor.Customer, value); }
         }

         internal static ProjectVMDescriptor CreateDescriptor(
            RefreshControllerBehaviorMock refreshControllerBehavior,
            DescendantValidationBehavioMock descendantValidationBehavior,
            ViewModelRevalidationBehaviorMock viewModelRevalidationBehavior,
            PropertyRevalidationBehaviorMock propertyRevalidationBehavior,
            bool useMockBehaviors
         ) {
            return VMDescriptorBuilder
               .OfType<ProjectVMDescriptor>()
               .For<ProjectVM>()
               .WithProperties((d, c) => {
                  var b = c.GetPropertyBuilder();

                  d.Title = b.Property.Of<string>();
                  d.Customer = b.VM.Of<CustomerVM>();
               })
               .WithValidators(b => {
                  b.EnableParentValidation(x => x.Title);
               })
               .WithBehaviors(b => {
                  if (useMockBehaviors) {
                     b.AddBehavior(refreshControllerBehavior);
                  }
               })
               .Build();
         }
      }

      internal sealed class ProjectVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Title { get; set; }
         public IVMPropertyDescriptor<CustomerVM> Customer { get; set; }
      }

      internal sealed class CustomerVM : TestViewModel<CustomerVMDescriptor> {
         public CustomerVM(bool useMockBehaviors)
            : this(
               new RefreshControllerBehaviorMock(),
               new DescendantValidationBehavioMock(),
               new ViewModelRevalidationBehaviorMock(),
               new PropertyRevalidationBehaviorMock(),
               useMockBehaviors) {
         }

         private CustomerVM(
            RefreshControllerBehaviorMock refreshControllerBehavior,
            DescendantValidationBehavioMock descendantValidationBehavior,
            ViewModelRevalidationBehaviorMock viewModelRevalidationBehavior,
            PropertyRevalidationBehaviorMock propertyRevalidationBehavior,
            bool useMockBehaviors)
            : base(
               CreateDescriptor(
                  refreshControllerBehavior,
                  descendantValidationBehavior,
                  viewModelRevalidationBehavior,
                  propertyRevalidationBehavior,
                  useMockBehaviors
               )
            ) {
            RefreshControllerBehaviorMock = refreshControllerBehavior;
            DescendantValidationBehavioMock = descendantValidationBehavior;
            ViewModelRevalidationBehaviorMock = viewModelRevalidationBehavior;
            PropertyRevalidationBehaviorMock = propertyRevalidationBehavior;
         }

         internal RefreshControllerBehaviorMock RefreshControllerBehaviorMock { get; private set; }
         internal DescendantValidationBehavioMock DescendantValidationBehavioMock { get; private set; }
         internal ViewModelRevalidationBehaviorMock ViewModelRevalidationBehaviorMock { get; private set; }
         internal PropertyRevalidationBehaviorMock PropertyRevalidationBehaviorMock { get; private set; }

         internal string Name {
            get { return GetValue(Descriptor.Name); }
            set { SetValue(Descriptor.Name, value); }
         }

         internal int Rating {
            get { return GetValue(Descriptor.Rating); }
            set { SetValue(Descriptor.Rating, value); }
         }

         internal string Address {
            get { return GetValue(Descriptor.Address); }
            set { SetValue(Descriptor.Address, value); }
         }

         private static CustomerVMDescriptor CreateDescriptor(
            RefreshControllerBehaviorMock refreshControllerBehavior,
            DescendantValidationBehavioMock descendantValidationBehavior,
            ViewModelRevalidationBehaviorMock viewModelRevalidationBehavior,
            PropertyRevalidationBehaviorMock propertyRevalidationBehavior,
            bool useMockBehaviors
         ) {
            return VMDescriptorBuilder
               .OfType<CustomerVMDescriptor>()
               .For<CustomerVM>()
               .WithProperties((d, c) => {
                  var b = c.GetPropertyBuilder();

                  d.Name = b.Property.Of<string>();
                  d.Address = b.Property.Of<string>();
                  d.Rating = b.Property.Of<int>();
               })
               .WithValidators(b => {
                  b.Check(x => x.Name)
                     .HasValue(String.Empty);
               })
               .WithBehaviors(b => {
                  if (useMockBehaviors) {
                     b.AddBehavior(refreshControllerBehavior);
                  }
               })
               .Build();
         }
      }

      internal sealed class CustomerVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<int> Rating { get; set; }
         public IVMPropertyDescriptor<string> Address { get; set; }
      }


      internal class RefreshControllerBehaviorMock :
         Behavior,
         IRefreshControllerBehavior {

         private IViewModel _refreshedViewModel;

         internal IViewModel RefreshedViewModel { get { return _refreshedViewModel; } }

         public void Refresh(IBehaviorContext context) {
            _refreshedViewModel = context.VM;
         }

         public void Refresh(IBehaviorContext context, IVMPropertyDescriptor property) {

         }
      }

      internal class DescendantValidationBehavioMock :
         Behavior,
         IDescendantValidationBehavior {

         public void RevalidateDescendants(IBehaviorContext context, ValidationScope scope) {
            throw new NotImplementedException();
         }
      }

      internal class ViewModelRevalidationBehaviorMock :
         Behavior,
         IViewModelRevalidationBehavior {

         public void Revalidate(IBehaviorContext context, ValidationController controller) {
            throw new NotImplementedException();
         }
      }

      internal class PropertyRevalidationBehaviorMock :
         Behavior,
         IPropertyRevalidationBehavior {

         public void BeginValidation(IBehaviorContext context, ValidationController controller) {
            throw new NotImplementedException();
         }

         public void Revalidate(IBehaviorContext context) {
            throw new NotImplementedException();
         }

         public void EndValidation(IBehaviorContext context) {
            throw new NotImplementedException();
         }
      }

   }
}