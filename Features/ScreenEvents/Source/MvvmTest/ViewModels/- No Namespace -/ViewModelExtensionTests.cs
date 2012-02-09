namespace Inspiring.MvvmTest.ViewModels {
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public sealed class ViewModelExtensionTests {

      [TestMethod]
      public void GetAnchestors_ReturnsAnchestors() {
         GrandparentVM grandparent = new GrandparentVM();
         ParentVM parent = new ParentVM();
         ChildVM child = new ChildVM();

         grandparent.SetValue(x => x.Child, parent);
         parent.SetValue(x => x.Child, child);

         var expectedAnchestors = new IViewModel[] { parent, grandparent };

         CollectionAssert.AreEqual(expectedAnchestors, child.GetAnchestors().ToList());
      }

      private class GrandparentVM : ViewModel<GrandparentVMDescriptor> {
         public static readonly GrandparentVMDescriptor ClassDescriptor = VMDescriptorBuilder
           .OfType<GrandparentVMDescriptor>()
           .For<GrandparentVM>()
           .WithProperties((d, b) => {
              var v = b.GetPropertyBuilder();
              d.Child = v.VM.Of<ParentVM>();
           })
           .Build();

         public GrandparentVM()
            : base(ClassDescriptor) {

         }
      }

      private class GrandparentVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ParentVM> Child { get; set; }
      }

      private class ParentVM : ViewModel<ParentVMDescriptor> {
         public static readonly ParentVMDescriptor ClassDescriptor = VMDescriptorBuilder
           .OfType<ParentVMDescriptor>()
           .For<ParentVM>()
           .WithProperties((d, b) => {
              var v = b.GetPropertyBuilder();
              d.Child = v.VM.Of<ChildVM>();
           })
           .Build();

         public ParentVM()
            : base(ClassDescriptor) {

         }
      }

      private class ParentVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ChildVM> Child { get; set; }
      }

      private class ChildVM : ViewModel<ChildVMDescriptor> {
         public static readonly ChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
           .OfType<ChildVMDescriptor>()
           .For<ChildVM>()
           .WithProperties((d, b) => {
           })
           .Build();

         public ChildVM()
            : base(ClassDescriptor) {
         }
      }

      private class ChildVMDescriptor : VMDescriptor {

      }
   }
}
