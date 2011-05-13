namespace Inspiring.MvvmTest.ApiTests.ViewModels.DeclarativeDeendencies {
   using System;
   using System.Collections.Generic;
   using System.Collections.ObjectModel;
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
               new DescendantValidationBehaviorMock(),
               new ViewModelRevalidationBehaviorMock(),
               new PropertyRevalidationBehaviorMock(),
               new PropertyRevalidationBehaviorMock(),
               new PropertyRevalidationBehaviorMock(),
               useMockBehaviors
            ) {
         }

         private EmployeeVM(
               Action<IVMDependencyBuilder<EmployeeVM, EmployeeVMDescriptor>> dependencyConfigurationAction,
               ProjectVMDescriptor projectVMDescriptor,
               RefreshControllerBehaviorMock refreshControllerBehavior,
               DescendantValidationBehaviorMock descendantValidationBehavior,
               ViewModelRevalidationBehaviorMock viewModelRevalidationBehavior,
               PropertyRevalidationBehaviorMock namePropertyRevalidationBehaviorMock,
               PropertyRevalidationBehaviorMock projectsPropertyRevalidationBehaviorMock,
               PropertyRevalidationBehaviorMock selectedProjectPropertyRevalidationBehaviorMock,
               bool useMockBehaviors
         )
            : base(
               CreateDescriptor(
                  dependencyConfigurationAction,
                  projectVMDescriptor,
                  refreshControllerBehavior,
                  descendantValidationBehavior,
                  viewModelRevalidationBehavior,
                  namePropertyRevalidationBehaviorMock,
                  projectsPropertyRevalidationBehaviorMock,
                  selectedProjectPropertyRevalidationBehaviorMock,
                  useMockBehaviors
                )
            ) {
            RefreshControllerBehaviorMock = refreshControllerBehavior;
            DescendantValidationBehavioMock = descendantValidationBehavior;
            ViewModelRevalidationBehaviorMock = viewModelRevalidationBehavior;
            NamePropertyRevalidationBehaviorMock = namePropertyRevalidationBehaviorMock;
            ProjectsPropertyRevalidationBehaviorMock = projectsPropertyRevalidationBehaviorMock;
            SelectedProjectPropertyRevalidationBehaviorMock = selectedProjectPropertyRevalidationBehaviorMock;
         }

         internal RefreshControllerBehaviorMock RefreshControllerBehaviorMock { get; private set; }
         internal DescendantValidationBehaviorMock DescendantValidationBehavioMock { get; private set; }
         internal ViewModelRevalidationBehaviorMock ViewModelRevalidationBehaviorMock { get; private set; }
         internal PropertyRevalidationBehaviorMock NamePropertyRevalidationBehaviorMock { get; private set; }
         internal PropertyRevalidationBehaviorMock ProjectsPropertyRevalidationBehaviorMock { get; private set; }
         internal PropertyRevalidationBehaviorMock SelectedProjectPropertyRevalidationBehaviorMock { get; private set; }

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
            DescendantValidationBehaviorMock descendantValidationBehavior,
            ViewModelRevalidationBehaviorMock viewModelRevalidationBehavior,
            PropertyRevalidationBehaviorMock namePropertyRevalidationBehaviorMock,
            PropertyRevalidationBehaviorMock projectsPropertyRevalidationBehaviorMock,
            PropertyRevalidationBehaviorMock selectedProjectPropertyRevalidationBehaviorMock,
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
                     if (refreshControllerBehavior != null) {
                        b.AddBehavior(refreshControllerBehavior);
                     }
                     if (viewModelRevalidationBehavior != null) {
                        b.AddBehavior(viewModelRevalidationBehavior);
                     }
                     if (descendantValidationBehavior != null) {
                        b.AddBehavior(descendantValidationBehavior);
                     }
                     if (namePropertyRevalidationBehaviorMock != null) {
                        b.Property(x => x.Name).AddBehavior(namePropertyRevalidationBehaviorMock);
                     }
                     if (projectsPropertyRevalidationBehaviorMock != null) {
                        b.Property(x => x.SelectedProject).AddBehavior(selectedProjectPropertyRevalidationBehaviorMock);
                     }
                     if (selectedProjectPropertyRevalidationBehaviorMock != null) {
                        b.Property(x => x.Projects).AddBehavior(projectsPropertyRevalidationBehaviorMock);
                     }
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
               new DescendantValidationBehaviorMock(),
               new ViewModelRevalidationBehaviorMock(),
               new PropertyRevalidationBehaviorMock(),
               new PropertyRevalidationBehaviorMock(),
               useMockBehaviors
            ) {
         }

         private ProjectVM(
            RefreshControllerBehaviorMock refreshControllerBehavior,
            DescendantValidationBehaviorMock descendantValidationBehavior,
            ViewModelRevalidationBehaviorMock viewModelRevalidationBehavior,
            PropertyRevalidationBehaviorMock titlePropertyRevalidationBehavior,
            PropertyRevalidationBehaviorMock customerPropertyRevalidationBehavior,
            bool useMockBehaviors
         )
            : base(
               CreateDescriptor(
                  refreshControllerBehavior,
                  descendantValidationBehavior,
                  viewModelRevalidationBehavior,
                  titlePropertyRevalidationBehavior,
                  customerPropertyRevalidationBehavior,
                  useMockBehaviors
               )
            ) {
            RefreshControllerBehaviorMock = refreshControllerBehavior;
            DescendantValidationBehavioMock = descendantValidationBehavior;
            ViewModelRevalidationBehaviorMock = viewModelRevalidationBehavior;
            TitelPropertyRevalidationBehaviorMock = titlePropertyRevalidationBehavior;
            CustomerPropertyRevalidationBehaviorMock = customerPropertyRevalidationBehavior;
         }

         internal RefreshControllerBehaviorMock RefreshControllerBehaviorMock { get; private set; }
         internal DescendantValidationBehaviorMock DescendantValidationBehavioMock { get; private set; }
         internal ViewModelRevalidationBehaviorMock ViewModelRevalidationBehaviorMock { get; private set; }
         internal PropertyRevalidationBehaviorMock TitelPropertyRevalidationBehaviorMock { get; private set; }
         internal PropertyRevalidationBehaviorMock CustomerPropertyRevalidationBehaviorMock { get; private set; }

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
            DescendantValidationBehaviorMock descendantValidationBehavior,
            ViewModelRevalidationBehaviorMock viewModelRevalidationBehavior,
            PropertyRevalidationBehaviorMock titlePropertyRevalidationBehavior,
            PropertyRevalidationBehaviorMock customerPropertyRevalidationBehavior,
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
                     if (refreshControllerBehavior != null) {
                        b.AddBehavior(refreshControllerBehavior);
                     }
                     if (viewModelRevalidationBehavior != null) {
                        b.AddBehavior(viewModelRevalidationBehavior);
                     }
                     if (descendantValidationBehavior != null) {
                        b.AddBehavior(descendantValidationBehavior);
                     }
                     if (titlePropertyRevalidationBehavior != null) {
                        b.Property(x => x.Title).AddBehavior(titlePropertyRevalidationBehavior);
                     }
                     if (customerPropertyRevalidationBehavior != null) {
                        b.Property(x => x.Customer).AddBehavior(customerPropertyRevalidationBehavior);
                     }
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
               new DescendantValidationBehaviorMock(),
               new ViewModelRevalidationBehaviorMock(),
               new PropertyRevalidationBehaviorMock(),
               new PropertyRevalidationBehaviorMock(),
               new PropertyRevalidationBehaviorMock(),
               useMockBehaviors) {
         }

         private CustomerVM(
            RefreshControllerBehaviorMock refreshControllerBehavior,
            DescendantValidationBehaviorMock descendantValidationBehavior,
            ViewModelRevalidationBehaviorMock viewModelRevalidationBehavior,
            PropertyRevalidationBehaviorMock namePopertyRevalidationBehavior,
            PropertyRevalidationBehaviorMock addressPropertyRevalidationBehavior,
            PropertyRevalidationBehaviorMock ratingPropertyRevalidationBehavior,
            bool useMockBehaviors)
            : base(
               CreateDescriptor(
                  refreshControllerBehavior,
                  descendantValidationBehavior,
                  viewModelRevalidationBehavior,
                  namePopertyRevalidationBehavior,
                  addressPropertyRevalidationBehavior,
                  ratingPropertyRevalidationBehavior,
                  useMockBehaviors
               )
            ) {
            RefreshControllerBehaviorMock = refreshControllerBehavior;
            DescendantValidationBehavioMock = descendantValidationBehavior;
            ViewModelRevalidationBehaviorMock = viewModelRevalidationBehavior;
            NamePropertyRevalidationBehaviorMock = namePopertyRevalidationBehavior;
            AddressPropertyRevalidationBehaviorMock = addressPropertyRevalidationBehavior;
            RatingPropertyRevalidationBehaviorMock = ratingPropertyRevalidationBehavior;
         }

         internal RefreshControllerBehaviorMock RefreshControllerBehaviorMock { get; private set; }
         internal DescendantValidationBehaviorMock DescendantValidationBehavioMock { get; private set; }
         internal ViewModelRevalidationBehaviorMock ViewModelRevalidationBehaviorMock { get; private set; }
         internal PropertyRevalidationBehaviorMock NamePropertyRevalidationBehaviorMock { get; private set; }
         internal PropertyRevalidationBehaviorMock AddressPropertyRevalidationBehaviorMock { get; private set; }
         internal PropertyRevalidationBehaviorMock RatingPropertyRevalidationBehaviorMock { get; private set; }


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
            DescendantValidationBehaviorMock descendantValidationBehavior,
            ViewModelRevalidationBehaviorMock viewModelRevalidationBehavior,
            PropertyRevalidationBehaviorMock namePopertyRevalidationBehavior,
            PropertyRevalidationBehaviorMock addressPropertyRevalidationBehavior,
            PropertyRevalidationBehaviorMock ratingPropertyRevalidationBehavior,
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
                     if (refreshControllerBehavior != null) {
                        b.AddBehavior(refreshControllerBehavior);
                     }
                     if (viewModelRevalidationBehavior != null) {
                        b.AddBehavior(viewModelRevalidationBehavior);
                     }
                     if (descendantValidationBehavior != null) {
                        b.AddBehavior(descendantValidationBehavior);
                     }
                     if (ratingPropertyRevalidationBehavior != null) {
                        b.Property(x => x.Name).AddBehavior(ratingPropertyRevalidationBehavior);
                     }
                     if (addressPropertyRevalidationBehavior != null) {
                        b.Property(x => x.Address).AddBehavior(addressPropertyRevalidationBehavior);
                     }
                     if (ratingPropertyRevalidationBehavior != null) {
                        b.Property(x => x.Rating).AddBehavior(ratingPropertyRevalidationBehavior);
                     }
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

         private List<IViewModel> _refreshedViewModels = new List<IViewModel>();
         private List<IVMPropertyDescriptor> _refreshedProperties = new List<IVMPropertyDescriptor>();

         internal ReadOnlyCollection<IViewModel> RefreshedViewModels {
            get { return _refreshedViewModels.AsReadOnly(); }
         }

         internal ReadOnlyCollection<IVMPropertyDescriptor> RefreshedProperties {
            get { return _refreshedProperties.AsReadOnly(); }
         }

         public void Refresh(IBehaviorContext context) {
            _refreshedViewModels.Add(context.VM);
         }

         public void Refresh(IBehaviorContext context, IVMPropertyDescriptor property) {
            _refreshedProperties.Add(property);
         }
      }

      internal class DescendantValidationBehaviorMock :
         Behavior,
         IDescendantValidationBehavior {

         public void RevalidateDescendants(IBehaviorContext context, ValidationScope scope) {

         }
      }

      internal class ViewModelRevalidationBehaviorMock :
         Behavior,
         IBehaviorInitializationBehavior,
         IViewModelRevalidationBehavior {

         public void Revalidate(IBehaviorContext context, ValidationController controller) {

         }

         public void Initialize(BehaviorInitializationContext context) {

         }
      }

      internal class PropertyRevalidationBehaviorMock :
         Behavior,
         IBehaviorInitializationBehavior,
         IPropertyRevalidationBehavior {
         private IVMPropertyDescriptor _property;

         public void Initialize(BehaviorInitializationContext context) {
            _property = context.Property;
         }

         internal bool WasRevalidated { get; private set; }

         public void BeginValidation(IBehaviorContext context, ValidationController controller) {
            WasRevalidated = true;
         }

         public void Revalidate(IBehaviorContext context) {

         }

         public void EndValidation(IBehaviorContext context) {

         }
      }
   }
}