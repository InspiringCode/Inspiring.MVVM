using System;
using System.ComponentModel;
using Inspiring.Mvvm.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inspiring.MvvmTest.ViewModels.IntegrationTests {
   [TestClass]
   public class StandardValidationsTest {
      [TestMethod]
      public void CheckUnique() {
         ParentVM vm = new ParentVM();

         // TODO: Validation works only after vm were added to parent!
         ChildVM child1 = new ChildVM() { StringProperty = "Value 1" };
         ChildVM child2 = new ChildVM() { StringProperty = "Value 2" };
         ChildVM child3 = new ChildVM() { StringProperty = "Value 3" };

         vm.Children.Add(child1);
         vm.Children.Add(child2);
         vm.Children.Add(child3);

         child1.StringProperty = "Value 1";
         child2.StringProperty = "Value 2";
         child3.StringProperty = "Value 2";

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
      public void CheckHasValue() {
         ChildVM vm = new ChildVM();
         vm.StringProperty = String.Empty;

         IDataErrorInfo errorInfo = vm;
         Assert.IsNull(errorInfo.Error);
         Assert.AreEqual("No value", errorInfo["StringProperty"]);
      }


      private class ParentVM : ViewModel<ParentVMDescriptor> {
         public static readonly ParentVMDescriptor Descriptor = VMDescriptorBuilder
            .For<ParentVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyFactory();

               return new ParentVMDescriptor {
                  Children = v.Local<VMCollection<ChildVM>>()
               };
            })
            .Build();

         public ParentVM()
            : base(Descriptor) {
            Children = new VMCollection<ChildVM>(this, ChildVM.Descriptor);
         }

         public VMCollection<ChildVM> Children {
            get { return GetValue(Descriptor.Children); }
            set { SetValue(Descriptor.Children, value); }
         }
      }

      private class ChildVM : ViewModel<ChildVMDescriptor> {
         public static readonly ChildVMDescriptor Descriptor = VMDescriptorBuilder
            .For<ChildVM>()
            .CreateDescriptor(c => {
               var v = c.GetPropertyFactory();

               return new ChildVMDescriptor {
                  StringProperty = v.Local<string>()
               };
            })
            .WithValidations((d, c) => {
               c.Check(d.StringProperty).HasValue("No value");
               c.Check(d.StringProperty).WithParent<ParentVM>().IsUnique(x => x.Children, x => x.StringProperty, "Duplicate value");
            })
            .Build();

         public ChildVM()
            : base(Descriptor) {
         }

         public string StringProperty {
            get { return GetValue(Descriptor.StringProperty); }
            set { SetValue(Descriptor.StringProperty, value); }
         }
      }

      private class ParentVMDescriptor : VMDescriptor {
         public VMProperty<VMCollection<ChildVM>> Children { get; set; }
      }

      private class ChildVMDescriptor : VMDescriptor {
         public VMProperty<string> StringProperty { get; set; }
      }
   }
}
