namespace Inspiring.MvvmTest.ViewModels {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public sealed class ViewModelTreeHelperTests {
      [TestMethod]
      public void GetAncestors_ReturnsAnchestors() {
         TestVM child = new TestVM();

         TestVM parent1 = new TestVM();
         TestVM parent2 = new TestVM();
         TestVM parent3 = new TestVM();

         TestVM grandParent12 = new TestVM();
         TestVM grandParent3a = new TestVM();
         TestVM grandParent3b = new TestVM();

         TestVM grandGrandParent3a1 = new TestVM();
         TestVM grandGrandParent3a2 = new TestVM();

         grandParent12.Children.Add(parent1);
         grandParent12.Children.Add(parent2);

         grandGrandParent3a1.Children.Add(grandParent3a);
         grandGrandParent3a2.Children.Add(grandParent3a);
         grandParent3a.Children.Add(parent3);
         grandParent3b.Children.Add(parent3);

         parent1.Children.Add(child);
         parent2.Children.Add(child);
         parent3.Children.Add(child);

         var expectedAnchestors = new IViewModel[] { 
            parent1,
            parent2,
            parent3,
            grandParent12,
            grandParent3a,
            grandParent3b,
            grandGrandParent3a1,
            grandGrandParent3a2
         };

         CollectionAssert.AreEqual(
            expectedAnchestors,
            ViewModelTreeHelper.GetAncestors(child).ToList()
         );
      }

      private class TestVM : ViewModel<TestVMDescriptor> {
         public static readonly TestVMDescriptor ClassDescriptor = VMDescriptorBuilder
           .OfType<TestVMDescriptor>()
           .For<TestVM>()
           .WithProperties((d, b) => {
              var v = b.GetPropertyBuilder();
              d.Children = v.Collection.Of<TestVM>(d);
           })
           .Build();

         public TestVM()
            : base(ClassDescriptor) {
         }

         public IVMCollection<TestVM> Children {
            get { return GetValue(Descriptor.Children); }
         }
      }

      private class TestVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<TestVM>> Children { get; set; }
      }
   }
}
