namespace Inspiring.MvvmTest.ViewModels.Core.Common.Paths {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public abstract class StepFixture : PathFixture {
      protected EmployeeVM VM { get; set; }

      protected EmployeeVMDescriptor Descriptor {
         get { return EmployeeVM.ClassDescriptor; }
      }

      [TestInitialize]
      public void Setup() {
         VM = new EmployeeVM();
         var projectVM = new ProjectVM();
         VM.GetValue(x => x.Projects).Add(projectVM);
         VM.SetValue(x => x.SelectedProject, projectVM);
      }
   }
}
