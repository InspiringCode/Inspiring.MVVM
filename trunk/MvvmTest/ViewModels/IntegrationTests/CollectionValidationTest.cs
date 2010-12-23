//namespace Inspiring.MvvmTest.ViewModels.IntegrationTests {
//   using System.Collections.Generic;
//   using System.ComponentModel;
//   using System.Linq;
//   using Inspiring.Mvvm.ViewModels;
//   using Microsoft.VisualStudio.TestTools.UnitTesting;

//   [TestClass]
//   public class CollectionValidationTest {
//      [TestMethod]
//      public void CheckChangeNotification() {
//         var firstProject = new ProjectVM { Name = "X" };
//         var secondProject = new ProjectVM { Name = "Y" };

//         var firstProjectCounter = new PropertyChangedCounter(firstProject, "Item[]");
//         var secondProjectCounter = new PropertyChangedCounter(secondProject, "Item[]");

//         // The two projects are not contained by a collection yet
//         firstProject.Name = "Z";
//         secondProject.Name = "Z";
//         firstProjectCounter.AssertNoRaise();
//         secondProjectCounter.AssertNoRaise();

//         EmployeeVM emp = new EmployeeVM();

//         // Only one item is in the collection - no validation error
//         //    Values: [Z]
//         emp.Projects.Add(firstProject);
//         firstProjectCounter.AssertNoRaise();

//         // The second duplicate is added - both become invalid
//         //    Values: [Z, Z]
//         emp.Projects.Add(secondProject);
//         firstProjectCounter.AssertOneRaise();
//         secondProjectCounter.AssertOneRaise();

//         // The first duplicate is renamed - both become valid
//         //    Values [A, Z]
//         firstProject.NameDisplayValue = "A";
//         firstProjectCounter.AssertOneRaise();
//         secondProjectCounter.AssertOneRaise();

//         // The second is renamed too - nothing changes
//         //    Values: [A, B]
//         secondProject.NameDisplayValue = "B";
//         firstProjectCounter.AssertNoRaise();
//         secondProjectCounter.AssertNoRaise();

//         // The second is being renamed to a duplicate name - the value is not
//         // commited to the underlying property but cached as invalid display
//         // value.
//         //    Values: [A, B]
//         //    Display values: [A, A]
//         secondProject.NameDisplayValue = "A";
//         firstProjectCounter.AssertNoRaise();
//         secondProjectCounter.AssertOneRaise();

//         // Change the first project to a new name - making both valid again
//         //    Values: [C, A]
//         firstProject.NameDisplayValue = "C";
//         firstProjectCounter.AssertNoRaise();
//         secondProjectCounter.AssertOneRaise();

//         Assert.AreEqual(
//            "A",
//            secondProject.Name,
//            "Previously invalid display value was not commited to the underlying property."
//         );

//         // Change the name of the second project - nothing should change
//         //    Values: [B, A]
//         firstProject.NameDisplayValue = "B";
//         firstProjectCounter.AssertNoRaise();
//         secondProjectCounter.AssertNoRaise();
//      }

//      [TestMethod]
//      public void CheckCustomWithProperty() {
//         EmployeeVM vm = new EmployeeVM();

//         // TODO: Validation works only after vm were added to parent!
//         ProjectVM child1 = new ProjectVM() { Name = "Value 1" };
//         ProjectVM child2 = new ProjectVM() { Name = "Value 2" };
//         ProjectVM child3 = new ProjectVM() { Name = "Value 3" };

//         vm.SpareTimeProjects.Add(child1);
//         vm.SpareTimeProjects.Add(child2);
//         vm.SpareTimeProjects.Add(child3);

//         IDataErrorInfo errorInfo1 = child1;
//         IDataErrorInfo errorInfo2 = child2;
//         IDataErrorInfo errorInfo3 = child3;

//         Assert.IsNull(errorInfo1["Name"]);
//         Assert.IsNull(errorInfo2["Name"]);
//         Assert.IsNull(errorInfo3["Name"]);

//         child3.NameDisplayValue = "Value 2";

//         Assert.IsNull(errorInfo1.Error);
//         Assert.IsNull(errorInfo2.Error);
//         Assert.IsNull(errorInfo3.Error);

//         Assert.IsNull(errorInfo1["Name"]);
//         Assert.IsNull(errorInfo2["Name"]);
//         Assert.AreEqual("Duplicate", errorInfo3["Name"]);
//      }

//      private class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
//         public static readonly EmployeeVMDescriptor Descriptor = VMDescriptorBuilder
//            .OfType<>().For<EmployeeVM>()
//            .WithProperties((d, c) => {
//               var v = c.GetPropertyFactory();

//               return new EmployeeVMDescriptor {
//                  Projects = v.MappedCollection(x => x.ProjectsSource).Of<ProjectVM>(ProjectVM.Descriptor),
//                  SpareTimeProjects = v.MappedCollection(x => x.SpareTimeProjectsSource).Of<ProjectVM>(ProjectVM.Descriptor)
//               };
//            })
//            .WithValidators(c => {
//               c.CheckCollection(d.Projects).Custom((project, projects, args) => {
//                  if (projects.Any(x => x != project && x.Name == (string)args.PropertyValue)) {
//                     args.AddError("Duplicate");
//                  }

//                  args.AffectsOtherItems = true;
//               });
//               c.CheckCollection(d.SpareTimeProjects).Check<string>(ProjectVM.Descriptor.Name).Custom(args => {
//                  if (args.AllItems.Any(i => i.VM != args.Item.VM && i.Value == args.Item.Value)) {
//                     args.AddError("Duplicate");
//                  }

//                  args.AffectsOtherItems = true;
//               });
//            })
//            .Build();

//         public EmployeeVM()
//            : base(Descriptor) {
//            ProjectsSource = new List<string>();
//         }

//         public VMCollection<ProjectVM> Projects {
//            get { return GetValue(Descriptor.Projects); }
//         }

//         public VMCollection<ProjectVM> SpareTimeProjects {
//            get { return GetValue(Descriptor.SpareTimeProjects); }
//         }

//         private List<string> ProjectsSource { get; set; }

//         private List<string> SpareTimeProjectsSource { get; set; }
//      }

//      private class ProjectVM : ViewModel<ProjectVMDescriptor>, ICanInitializeFrom<string>, IVMCollectionItem<string> {
//         public static readonly ProjectVMDescriptor Descriptor = VMDescriptorBuilder
//            .OfType<>().For<ProjectVM>()
//            .WithProperties((d, c) => {
//               var v = c.GetPropertyFactory();

//               return new ProjectVMDescriptor {
//                  Name = v.Local<string>()
//               };
//            })
//            .WithValidators(c => {
//               c.Check(d.Name); // HACK: Enable validation
//            })
//            .Build();

//         public ProjectVM()
//            : base() {
//         }

//         public string Name {
//            get { return GetValue(Descriptor.Name); }
//            set { SetValue(Descriptor.Name, value); }
//         }

//         public object NameDisplayValue {
//            get { return Descriptor.Name.GetDisplayValue(this); }
//            set { Descriptor.Name.SetDisplayValue(this, value); }
//         }

//         public void InitializeFrom(string source) {
//            Name = source;
//         }

//         public string Source {
//            get { return Name; }
//         }
//      }

//      private class EmployeeVMDescriptor : VMDescriptor {
//         public VMCollectionProperty<ProjectVM> Projects { get; set; }
//         public VMCollectionProperty<ProjectVM> SpareTimeProjects { get; set; }
//      }

//      private class ProjectVMDescriptor : VMDescriptor {
//         public VMProperty<string> Name { get; set; }
//      }
//   }
//}