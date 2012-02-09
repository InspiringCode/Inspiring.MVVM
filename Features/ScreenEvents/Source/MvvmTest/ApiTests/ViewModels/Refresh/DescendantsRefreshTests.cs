namespace Inspiring.MvvmTest.ApiTests.ViewModels.Refresh {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public sealed class DescendantsRefreshTests : RefreshFixture {
      [TestMethod]
      public void Refresh_RefresesSelectedDescendants() {
         var root = new RootVM();
         var child = new ChildVM();
         var item1 = new ChildVM();

         root.Child = child;
         root.Children.Add(item1);

         root.RefreshDescendants(b => {
            b.Descendant(x => x.Children)
               .Properties(x => x.RefreshDetectionProperty);
         });

         Assert.IsTrue(item1.WasRefreshed);
         Assert.IsFalse(child.WasRefreshed);
      }

      [TestMethod]
      public void RefreshDescendants_DoesNotLoadUnloadedProperties() {
         var root = new RootVM();
         
         root.RefreshDescendants(b => {
            b.Descendant(x => x.Children)
               .Properties(x => x.RefreshDetectionProperty);
         });

         ViewModelAssert.IsNotLoaded(root, x => x.Children);
      }


      private class RootVM : ViewModel<RootVMDescriptor> {
         [ClassDescriptor]
         public static readonly RootVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<RootVMDescriptor>()
            .For<RootVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Child = v.VM.Of<ChildVM>();
               d.Children = v.Collection
                  .Wraps(x => x.ChildrenSource)
                  .With<ChildVM>(ChildVM.ClassDescriptor);
            })
            .Build();

         public RootVM()
            : base(ClassDescriptor) {
            ChildrenSource = new List<ChildSource>();
         }

         public ChildVM Child {
            get { return GetValue(Descriptor.Child); }
            set { SetValue(Descriptor.Child, value); }
         }

         public IVMCollection<ChildVM> Children {
            get { return GetValue(Descriptor.Children); }
         }

         private List<ChildSource> ChildrenSource {
            get;
            set;
         }

         public void RefreshDescendants(Action<IPathDefinitionBuilder<RootVMDescriptor>> refreshTargetBuilder) {
            base.RefreshDescendants(refreshTargetBuilder);
         }
      }

      private class RootVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ChildVM> Child { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ChildVM>> Children { get; set; }
      }
   }
}
