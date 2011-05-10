namespace Inspiring.MvvmTest.ViewModels.Core.FluentDescriptorBuilder {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class DependencyBuilderTests {

      //[TestMethod]
      //public void MyTestMethod() {
      //   var d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Self
      //      .Execute(() => { })
      //   );
      //}

      //public void MyTestMethod() {
      //   var d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Properties(x => x.Name, x => x.LastName)
      //      .Refresh
      //      .Properties(x => x.Projects)
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Descendant(x => x.SelectedProject)
      //      .Refresh
      //      .Properties(x => x.Name)
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Descendant(x => x.Projects)
      //      .Descendant(x => x.Customer)
      //      .Properties(x => x.Rating)
      //      .Refresh
      //      .Properties(x => x.Name)
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Descendant(x => x.Projects)
      //      .Properties(x => x.Customer)
      //      .Revalidate
      //      .Descendant(x => x.SelectedProject)
      //      .Properties(x => x.EndDate)
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Descendant(x => x.Projects)
      //      .Properties(x => x.Customer)
      //      .Revalidate
      //      .Descendant(x => x.Projects)
      //      .Properties(x => x.EndDate)
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Self
      //      .Execute(() => { })
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Self
      //      .OrAnyDescendant
      //      .Execute(() => { })
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Collection(x => x.Projects)
      //      .Execute(() => { })
      //   );

      //   d = BuildDescriptor(b => b
      //      .OnChangeOf
      //      .Descendant(x => x.SelectedProject)
      //      .OrAnyDescendant
      //      .Execute(() => { })
      //   );
      //}

      private EmployeeVMDescriptor BuildDescriptor(
         Action<IVMDependencyBuilder<EmployeeVM, EmployeeVMDescriptor>> configurationAction
      ) {
         return VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Name = v.Property.Of<string>();
               d.LastName = v.Property.Of<string>();
               d.SelectedProject = v.VM.Of<ProjectVM>();
               d.Projects = v.Collection.Of<ProjectVM>(new ProjectVMDescriptor());
            })
            .WithDependencies(configurationAction)
            .Build();
      }


      private sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> { }

      private sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<string> LastName { get; set; }
         public IVMPropertyDescriptor<ProjectVM> SelectedProject { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
      }

      private sealed class ProjectVM : ViewModel<ProjectVMDescriptor> { }

      private sealed class ProjectVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<DateTime> EndDate { get; set; }
         public IVMPropertyDescriptor<CustomerVM> Customer { get; set; }
      }

      private sealed class CustomerVM : ViewModel<CustomerVMDescriptor> { }

      private sealed class CustomerVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<double> Rating { get; set; }
      }
   }
}