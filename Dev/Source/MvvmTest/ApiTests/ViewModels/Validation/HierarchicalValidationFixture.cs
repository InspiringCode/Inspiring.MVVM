namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class HierarchicalValidationFixture {

      protected class GrandparentVM : ViewModel<GrandparentVMDescriptor> {
         public static readonly GrandparentVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<GrandparentVMDescriptor>()
            .For<GrandparentVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Children = v.Collection.Of<ParentVM>(ParentVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.ValidateDescendant(x => x.Children)
                  .ValidateDescendant(x => x.Child)
                  .Check(x => x.ChildProperty).Custom(args => { });

               b.ValidateDescendant(x => x.Children)
                  .ValidateDescendant(x => x.Child)
                  .CheckViewModel(args => { });

               b.ValidateDescendant(x => x.Children)
                  .ValidateDescendant(x => x.Child)
                  .CheckCollection(x => x.Items, x => x.ItemProperty)
                  .Custom<ItemVM>(args => { });

               b.ValidateDescendant(x => x.Children)
                  .ValidateDescendant(x => x.Child)
                  .CheckCollection(x => x.Items)
                  .Custom(args => { });
            })
            .Build();

         public GrandparentVM()
            : base(ClassDescriptor) {
         }
      }

      protected class GrandparentVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ParentVM>> Children { get; set; }
      }

      protected class ParentVM : ViewModel<ParentVMDescriptor> {
         public static readonly ParentVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ParentVMDescriptor>()
            .For<ParentVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Child = v.VM.Of<ChildVM>();
            })
            .WithValidators(b => {
               b.ValidateDescendant(x => x.Child)
                  .Check(x => x.ChildProperty).Custom(args => { });

               b.ValidateDescendant(x => x.Child)
                  .CheckViewModel(args => { });

               b.ValidateDescendant(x => x.Child)
                  .CheckCollection(x => x.Items, x => x.ItemProperty)
                  .Custom<ItemVM>(args => { });

               b.ValidateDescendant(x => x.Child)
                  .CheckCollection(x => x.Items)
                  .Custom(args => { });
            })
            .Build();

         public ParentVM()
            : base(ClassDescriptor) {
         }
      }

      protected class ParentVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ChildVM> Child { get; set; }
      }


      protected class ChildVM : ViewModel<ChildVMDescriptor> {
         public static readonly ChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ChildVMDescriptor>()
            .For<ChildVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.ChildProperty = v.Property.Of<string>();
               d.Items = v.Collection.Of<ItemVM>(ItemVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.Check(x => x.ChildProperty).Custom(args => { });

               b.CheckViewModel(args => { });

               b.CheckCollection(x => x.Items, x => x.ItemProperty)
                  .Custom<ItemVM>(args => { });

               b.CheckCollection(x => x.Items)
                  .Custom(args => { });
            })
            .Build();

         public ChildVM()
            : base(ClassDescriptor) {
         }
      }

      protected class ChildVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> ChildProperty { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ItemVM>> Items { get; set; }
      }

      protected class ItemVM : ViewModel<ItemVMDescriptor> {
         public static readonly ItemVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ItemVMDescriptor>()
            .For<ItemVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.ItemProperty = v.Property.Of<string>();
            })
            .Build();

         public ItemVM()
            : base(ClassDescriptor) {
         }
      }

      protected class ItemVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> ItemProperty { get; set; }
      }
   }
}