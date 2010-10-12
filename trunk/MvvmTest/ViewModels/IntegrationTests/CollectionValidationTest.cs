namespace Inspiring.MvvmTest.ViewModels.IntegrationTests {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionValidationTest {
      [TestMethod]
      public void CheckChangeNotification() {
         var firstProject = new ProjectVM { Name = "X" };
         var secondProject = new ProjectVM { Name = "Y" };

         var firstProjectCounter = new PropertyChangedCounter(firstProject, "Item[]");
         var secondProjectCounter = new PropertyChangedCounter(secondProject, "Item[]");

         // The two projects are not contained by a collection yet
         firstProject.Name = "Z";
         secondProject.Name = "Z";
         firstProjectCounter.AssertNoRaise();
         secondProjectCounter.AssertNoRaise();

         EmployeeVM emp = new EmployeeVM();

         // Only one item is in the collection - no validation error
         //    Values: [Z]
         emp.Projects.Add(firstProject);
         firstProjectCounter.AssertNoRaise();

         // The second duplicate is added - both become invalid
         //    Values: [Z, Z]
         emp.Projects.Add(secondProject);
         firstProjectCounter.AssertOneRaise();
         secondProjectCounter.AssertOneRaise();

         // The first duplicate is renamed - both become valid
         //    Values [A, Z]
         firstProject.NameDisplayValue = "A";
         firstProjectCounter.AssertOneRaise();
         secondProjectCounter.AssertOneRaise();

         // The second is renamed too - nothing changes
         //    Values: [A, B]
         secondProject.NameDisplayValue = "B";
         firstProjectCounter.AssertNoRaise();
         secondProjectCounter.AssertNoRaise();

         // The second is being renamed to a duplicate name - the value is not
         // commited to the underlying property but cached as invalid display
         // value.
         //    Values: [A, B]
         //    Display values: [A, A]
         secondProject.NameDisplayValue = "A";
         firstProjectCounter.AssertNoRaise();
         secondProjectCounter.AssertOneRaise();

         // Change the first project to a new name - making both valid again
         //    Values: [C, A]
         firstProject.NameDisplayValue = "C";
         firstProjectCounter.AssertNoRaise();
         secondProjectCounter.AssertOneRaise();

         Assert.AreEqual(
            "A",
            secondProject.Name,
            "Previously invalid display value was not commited to the underlying property."
         );

         // Change the name of the second project - nothing should change
         //    Values: [B, A]
         firstProject.NameDisplayValue = "B";
         firstProjectCounter.AssertNoRaise();
         secondProjectCounter.AssertNoRaise();
      }

      private class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static readonly EmployeeVMDescriptor Descriptor = VMDescriptorBuilder
            .For<EmployeeVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyFactory();

               return new EmployeeVMDescriptor {
                  Projects = v.Local<VMCollection<ProjectVM>>()
               };
            })
            .WithValidations((d, c) => {
               c.CheckCollection(d.Projects).Custom((project, projects, args) => {
                  if (projects.Any(x => x != project && x.Name == project.Name)) {
                     args.AddError("Duplicate");
                  }

                  args.AffectsOtherItems = true;
               });
               c.CheckCollection(d.test).Custom((project, projects, args) => {
                  if (projects.Any(x => x != project && x.Name == project.Name)) {
                     args.AddError("Duplicate");
                  }

                  args.AffectsOtherItems = true;
               });
            })
            .Build();

         public EmployeeVM()
            : base(Descriptor) {
            Projects = new VMCollection<ProjectVM>(this, ProjectVM.Descriptor);
         }

         public VMCollection<ProjectVM> Projects {
            get { return GetValue(Descriptor.Projects); }
            set { SetValue(Descriptor.Projects, value); }
         }
      }

      private class ProjectVM : ViewModel<ProjectVMDescriptor> {
         public static readonly ProjectVMDescriptor Descriptor = VMDescriptorBuilder
            .For<ProjectVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyFactory();

               return new ProjectVMDescriptor {
                  Name = v.Local<string>()
               };
            })
            .Build();

         public ProjectVM()
            : base(Descriptor) {
         }

         public string Name {
            get { return GetValue(Descriptor.Name); }
            set { SetValue(Descriptor.Name, value); }
         }

         public object NameDisplayValue {
            get { return Descriptor.Name.GetDisplayValue(this); }
            set { Descriptor.Name.SetDisplayValue(this, value); }
         }
      }

      private class EmployeeVMDescriptor : VMDescriptor {
         public VMProperty<VMCollection<ProjectVM>> Projects { get; set; }
         public VMCollectionProperty<ProjectVM> test { get; set; }
      }

      private class ProjectVMDescriptor : VMDescriptor {
         public VMProperty<string> Name { get; set; }
      }
   }
}