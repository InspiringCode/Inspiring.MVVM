namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CustomCollectionValidationTests : TestBase {
      [TestMethod]
      public void ItemValidation_ExecutesCollectionCustomValidator() {
         var vm = new EmployeeVM();
         var p1 = new ProjectVM { IsSelected = true };
         var p2 = new ProjectVM();
         var p3 = new ProjectVM { IsSelected = true };

         vm.Projects.Add(p1);
         vm.Projects.Add(p2);
         vm.Projects.Add(p3);

         Assert.IsFalse(vm.GetValidationState(ValidationResultScope.Descendants).IsValid);
      }

      private sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, b) => {
               var vm = b.GetPropertyBuilder();

               d.Projects = vm
                  .Collection
                  .Of<ProjectVM>(ProjectVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.CheckCollection<ProjectVMDescriptor, bool>(x => x.Projects, x => x.IsSelected)
                  .Custom<ProjectVM>(args => {

                     var selectedItems = args
                        .Items
                        .Where(x => x.IsSelected)
                        .ToArray();

                     if (selectedItems.Count() > 1) {
                        foreach (ProjectVM p in selectedItems) {
                           args.AddError(p, "error");
                        }
                     }
                  });
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
         }

         public IVMCollection<ProjectVM> Projects {
            get { return GetValue(Descriptor.Projects); }
         }
      }

      private sealed class ProjectVM : ViewModel<ProjectVMDescriptor> {
         public static readonly ProjectVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ProjectVMDescriptor>()
            .For<ProjectVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.IsSelected = v.Property.Of<bool>();
            })
            .WithValidators(b => {
               b.EnableParentValidation(x => x.IsSelected);
            })
            .Build();

         public ProjectVM()
            : base(ClassDescriptor) {
         }

         public bool IsSelected { get; set; }
      }

      private sealed class ProjectVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<bool> IsSelected { get; set; }
      }

      private sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
      }
   }
}