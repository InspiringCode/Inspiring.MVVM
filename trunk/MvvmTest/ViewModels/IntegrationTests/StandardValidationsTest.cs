using System;
using System.Collections.Generic;
using System.ComponentModel;
using Inspiring.Mvvm.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Inspiring.Mvvm.ViewModels.Core;

namespace Inspiring.MvvmTest.ViewModels.IntegrationTests {
   [TestClass]
   public class StandardValidationsTest {
      [TestMethod]
      public void CheckUnique() {
         ParentVM vm = new ParentVM();

         // TODO: Validation works only after vm were added to parent!
         ChildVM child1 = new ChildVM() { StringProperty = "Val1" };
         ChildVM child2 = new ChildVM() { StringProperty = "Val2" };
         ChildVM child3 = new ChildVM() { StringProperty = "Val3" };

         vm.Children.Add(child1);
         vm.Children.Add(child2);
         vm.Children.Add(child3);

         child1.StringProperty = "Val1";
         child2.StringProperty = "Val2";
         child3.StringProperty = "Val2";

         IDataErrorInfo errorInfo1 = child1;
         IDataErrorInfo errorInfo2 = child2;
         IDataErrorInfo errorInfo3 = child3;

         Assert.IsNull(errorInfo1.Error);
         Assert.IsNull(errorInfo2.Error);
         Assert.IsNull(errorInfo3.Error);

         Assert.IsNull(errorInfo1["StringProperty"]);
         //Assert.AreEqual("Duplicate value", errorInfo2["StringProperty"]);
         Assert.AreEqual("Duplicate value", errorInfo3["StringProperty"]);
      }

      [TestMethod]
      public void ChildValidation_OneChildInvalid_ParentHasError() {
         ParentVM vm = new ParentVM();
         IDataErrorInfo info = vm;

         ChildVM child1 = new ChildVM() { StringProperty = "Val1" };
         ChildVM child2 = new ChildVM() { StringProperty = "Val2" };

         vm.Children.Add(child1);
         vm.Children.Add(child2);

         Assert.IsTrue(vm.IsValid);
         Assert.IsNull(info.Error);

         child2.StringProperty = "";
         Assert.AreEqual("Child invalid", info.Error);
      }

      [TestMethod]
      public void CheckHasValue() {
         ChildVM vm = new ChildVM();
         vm.StringProperty = String.Empty;

         IDataErrorInfo errorInfo = vm;
         Assert.IsNull(errorInfo.Error);
         Assert.AreEqual("No value", errorInfo["StringProperty"]);
      }

      [TestMethod]
      public void CheckLength() {
         ChildVM vm = new ChildVM();
         vm.StringProperty = "Wert";

         IDataErrorInfo errorInfo = vm;
         Assert.IsNull(errorInfo.Error);
         Assert.IsNull(errorInfo["StringProperty"]);

         vm.StringProperty = "Wert!";

         Assert.IsNull(errorInfo.Error);
         Assert.AreEqual("Max length 4", errorInfo["StringProperty"]);
      }


      private class ParentVM : ViewModel<ParentVMDescriptor> {
         public static readonly ParentVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ParentVMDescriptor>()
            .For<ParentVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               d.Children = v.Collection.Wraps(x => x.Source).With<ChildVM>(ChildVM.ClassDescriptor);
            })
            .WithValidators(c => {
               c.CheckCollection(x => x.Children, x => x.StringProperty).IsUnique(
                  StringComparison.CurrentCultureIgnoreCase,
                  "Duplicate value"
               );
               c.PropagateChildErrors("Child invalid");
            })
            .Build();

         public ParentVM()
            : base(ClassDescriptor) {
            Source = new List<string>();
         }

         public IVMCollection<ChildVM> Children {
            get { return GetValue(Descriptor.Children); }
            set { SetValue(Descriptor.Children, value); }
         }

         private List<string> Source { get; set; }

         public bool IsValid {
            get { return Kernel.GetValidationState().IsValid; }
         }
      }

      private class ChildVM : ViewModel<ChildVMDescriptor>, ICanInitializeFrom<string>, IHasSourceObject<string> {
         public static readonly ChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ChildVMDescriptor>()
            .For<ChildVM>()
            .WithProperties((d, c) => {
               var v = c.GetPropertyBuilder();
               d.StringProperty = v.Property.Of<string>();
            })
            .WithValidators(c => {
               c.Check(x => x.StringProperty).HasValue("No value");
               c.Check(x => x.StringProperty).Length(4, "Max length {0}");
            })
            .Build();

         public ChildVM()
            : base(ClassDescriptor) {
         }

         public string StringProperty {
            get { return GetValue(Descriptor.StringProperty); }
            set { SetValue(Descriptor.StringProperty, value); }
         }

         public void InitializeFrom(string source) {
            StringProperty = source;
         }

         public string Source {
            get { return StringProperty; }
         }

         public bool IsValid {
            get { return Kernel.GetValidationState().IsValid; }
         }
      }

      private class ParentVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ChildVM>> Children { get; set; }
      }

      private class ChildVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> StringProperty { get; set; }
      }
   }
}
