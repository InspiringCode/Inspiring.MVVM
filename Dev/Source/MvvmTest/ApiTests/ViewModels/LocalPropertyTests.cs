namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class LocalPropertyTests : TestBase {
      public EmployeeVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new EmployeeVM();
      }

      [TestMethod]
      public void LocalCollection_Initially_ReturnsEmptyInstance() {
         Assert.IsNotNull(VM.Projects);
      }

      [TestMethod]
      public void LocalViewModel_Initially_ReturnsNull() {
         Assert.IsNull(VM.CurrentProject);
      }

      [TestMethod]
      public void SetViewModel_ParentIsSet() {
         var child = new ProjectVM();
         VM.CurrentProject = child;

         Assert.AreSame(VM, child.Parent);
      }

      [TestMethod]
      public void AddItemToCollection_ParentIsSet() {
         var child = new ProjectVM();
         VM.Projects.Add(child);

         Assert.AreSame(VM, child.Parent);
      }

      public sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, c) => {
               var vm = c.GetPropertyBuilder();

               d.CurrentProject = vm.VM.Of<ProjectVM>();
               d.Projects = vm.Collection.Of<ProjectVM>(ProjectVM.ClassDescriptor);
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
         }

         public ProjectVM CurrentProject {
            get { return GetValue(Descriptor.CurrentProject); }
            set { SetValue(Descriptor.CurrentProject, value); }
         }

         public IVMCollection<ProjectVM> Projects {
            get { return GetValue(Descriptor.Projects); }
         }
      }

      public sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ProjectVM> CurrentProject { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
      }
   }
}