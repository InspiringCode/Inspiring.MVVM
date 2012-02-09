namespace Inspiring.MvvmTest.ViewModels.Core.Common {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class PropertySelectorTests {
      [TestMethod]
      public void PropertyName_WithSimpleProperty_ReturnsPropertyName() {
         var selector = new PropertySelector<TestVMDescriptor>(x => x.StringProperty);
         Assert.AreEqual("StringProperty", selector.PropertyName);
      }

      [TestMethod]
      public void PropertyName_WithCollectionProperty_ReturnsPropertyName() {
         var selector = new PropertySelector<TestVMDescriptor>(x => x.CollectionProperty);
         Assert.AreEqual("CollectionProperty", selector.PropertyName);
      }

      [TestMethod]
      public void ToString_ReturnsDescriptorTypeAndPropertyName() {
         var selector = new PropertySelector<TestVMDescriptor>(x => x.StringProperty);
         Assert.AreEqual("TestVMDescriptor.StringProperty", selector.ToString());
      }

      public sealed class TestVM : ViewModel<TestVMDescriptor> {
         [ClassDescriptor]
         public static readonly TestVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<TestVMDescriptor>()
            .For<TestVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.StringProperty = v.Property.Of<string>();
               d.CollectionProperty = v.Collection.Of<TestVM>(d);
            })
            .Build();

         public TestVM()
            : base(ClassDescriptor) {
         }
      }

      public sealed class TestVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> StringProperty { get; set; }
         public IVMPropertyDescriptor<IVMCollection<TestVM>> CollectionProperty { get; set; }
      }
   }
}