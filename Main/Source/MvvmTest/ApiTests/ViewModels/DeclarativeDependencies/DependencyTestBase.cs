namespace Inspiring.MvvmTest.ApiTests.ViewModels.DeclarativeDeendencies {
   using System;
   using System.Collections.Generic;
   using System.Collections.ObjectModel;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public abstract class DependencyTestBase {
      protected ValidatorMockConfigurationFluent Results { get; private set; }

      [TestInitialize]
      public void InitilaizeTest() {
         Results = new ValidatorMockConfigurationFluent();
      }

      internal sealed class EmployeeVM : TestViewModel<EmployeeVMDescriptor> {
         public EmployeeVM(
            Action<IVMDependencyBuilder<EmployeeVM, EmployeeVMDescriptor>> dependencyConfigurationAction,
            ProjectVMDescriptor projectVMDescriptor,
            bool useMockBehaviors,
            ValidatorMockConfigurationFluent results,
            RefreshControllerBehaviorMock refreshControllerBehavior
         )
            : base(
               CreateDescriptor(
                  dependencyConfigurationAction,
                  projectVMDescriptor,
                  refreshControllerBehavior,
                  useMockBehaviors
                )
            ) {

            RefreshControllerBehaviorMock = refreshControllerBehavior;
            Results = results;
         }

         internal RefreshControllerBehaviorMock RefreshControllerBehaviorMock { get; private set; }

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

         internal bool InvalidateNameProperty { get; set; }

         private ValidatorMockConfigurationFluent Results { get; set; }

         private static EmployeeVMDescriptor CreateDescriptor(
            Action<IVMDependencyBuilder<EmployeeVM, EmployeeVMDescriptor>> dependencyConfigurationAction,
            ProjectVMDescriptor projectVMDescriptor,
            RefreshControllerBehaviorMock refreshControllerBehavior,
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
                     .Custom((args) => {
                        if (args.Owner.InvalidateNameProperty) {
                           args.AddError("Test triggered failure");
                        }
                     });
                  b.Check(x => x.SelectedProject)
                     .Custom(args => args.Owner.Results.PerformValidation(args));
               })
               .WithDependencies(
                  dependencyConfigurationAction
               )
               .WithBehaviors(b => {
                  if (useMockBehaviors) {
                     if (refreshControllerBehavior != null) {
                        b.AddBehavior(refreshControllerBehavior);
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
         public ProjectVM(bool useMockBehaviors, ValidatorMockConfigurationFluent results)
            : this(
               new RefreshControllerBehaviorMock(),
               useMockBehaviors
            ) {
            Results = results;
         }

         private ProjectVM(
            RefreshControllerBehaviorMock refreshControllerBehavior,
            bool useMockBehaviors
         )
            : base(
               CreateDescriptor(
                  refreshControllerBehavior,
                  useMockBehaviors
               )
            ) {
            RefreshControllerBehaviorMock = refreshControllerBehavior;
         }

         internal RefreshControllerBehaviorMock RefreshControllerBehaviorMock { get; private set; }

         internal string Title {
            get { return GetValue(Descriptor.Title); }
            set { SetValue(Descriptor.Title, value); }
         }

         internal CustomerVM Customer {
            get { return GetValue(Descriptor.Customer); }
            set { SetValue(Descriptor.Customer, value); }
         }

         private ValidatorMockConfigurationFluent Results { get; set; }

         internal static ProjectVMDescriptor CreateDescriptor(
            RefreshControllerBehaviorMock refreshControllerBehavior,
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
                  b.Check(x => x.Title)
                     .Custom(args => args.Owner.Results.PerformValidation(args));
                  b.Check(x => x.Customer)
                     .Custom(args => args.Owner.Results.PerformValidation(args));

               })
               .WithBehaviors(b => {
                  if (useMockBehaviors) {
                     if (refreshControllerBehavior != null) {
                        b.AddBehavior(refreshControllerBehavior);
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
         public CustomerVM(bool useMockBehaviors, ValidatorMockConfigurationFluent results)
            : this(
               new RefreshControllerBehaviorMock(),
               useMockBehaviors) {
            Results = results;
         }

         private CustomerVM(
            RefreshControllerBehaviorMock refreshControllerBehavior,
            bool useMockBehaviors)
            : base(
               CreateDescriptor(
                  refreshControllerBehavior,
                  useMockBehaviors
               )
            ) {
            RefreshControllerBehaviorMock = refreshControllerBehavior;
         }

         internal RefreshControllerBehaviorMock RefreshControllerBehaviorMock { get; private set; }

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

         private ValidatorMockConfigurationFluent Results { get; set; }

         private static CustomerVMDescriptor CreateDescriptor(
            RefreshControllerBehaviorMock refreshControllerBehavior,
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
                    .Custom(args => args.Owner.Results.PerformValidation(args));
                  b.Check(x => x.Rating)
                    .Custom(args => args.Owner.Results.PerformValidation(args));

               })
               .WithBehaviors(b => {
                  if (useMockBehaviors) {
                     if (refreshControllerBehavior != null) {
                        b.AddBehavior(refreshControllerBehavior);
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

         public void Refresh(IBehaviorContext context, bool executeRefreshDependencies) {
            _refreshedViewModels.Add(context.VM);
         }

         public void Refresh(IBehaviorContext context, IVMPropertyDescriptor property, bool executeRefreshDependencies) {
            _refreshedProperties.Add(property);
         }
      }
   }
}