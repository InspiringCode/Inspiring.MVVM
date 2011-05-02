namespace Inspiring.MvvmTest.ApiTests.ViewModels.Undo {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public sealed class EmployeeVM : DefaultViewModelWithSourceBase<EmployeeVMDescriptor, Employee> {

      private static readonly Action<EmployeeVM, ChangeArgs> NoChangeHandling = (vm, args) => { };

      public EmployeeVM()
         : base(CreateDescriptor(ProjectVM.ClassDescriptorWithoutUndoRoot, NoChangeHandling)) {
      }

      public EmployeeVM(ProjectVMDescriptor projectVMDescriptor)
         : base(CreateDescriptor(projectVMDescriptor, NoChangeHandling)) {

      }

      public EmployeeVM(
         Action<EmployeeVM, ChangeArgs> handleChangeAction
      )
         : base(CreateDescriptor(ProjectVM.ClassDescriptorWithoutUndoRoot, handleChangeAction)) {

      }

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

      internal new EmployeeVMDescriptor Descriptor {
         get { return base.Descriptor; }
      }

      internal void Revalidate() {
         Kernel.Revalidate(ValidationScope.SelfAndAllDescendants, ValidationMode.CommitValidValues);
      }

      internal void SetSource(Employee emp, Action action) {
         base.SetSource(emp);
         action();
      }

      private static EmployeeVMDescriptor CreateDescriptor(
         ProjectVMDescriptor projectVMDescriptor,
         Action<EmployeeVM, ChangeArgs> handleChangeAction
      ) {
         return VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, c) => {
               var b = c.GetPropertyBuilder(x => x.Source);

               d.Name = b.Property.MapsTo(x => x.Name);
               d.Projects = b.Collection
                  .Wraps(x => x.Projects)
                  .With<ProjectVM>(projectVMDescriptor);
               d.SelectedProject = b.Property.Of<ProjectVM>();
            })
            .WithValidators(b => {
               b.Check(x => x.Name)
                  .HasValue(String.Empty);
               b.CheckCollection(x => x.Projects, x => x.Title)
                  .IsUnique(string.Empty);
            })
            .WithViewModelBehaviors(b => {
               b.IsUndoRoot();
               b.AddChangeHandler((vm, changeArgs) => {
                  handleChangeAction(vm, changeArgs);
               });
            })
            .Build();
      }
   }

   public sealed class EmployeeVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Name { get; set; }
      public IVMPropertyDescriptor<ProjectVM> SelectedProject { get; set; }
      public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
   }
}