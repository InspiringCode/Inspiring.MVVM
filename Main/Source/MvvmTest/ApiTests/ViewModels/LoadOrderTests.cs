namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System.Text;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class LoadOrderTests {
      private const string SourcePropertyDelegateLog = "SourcePropertyDelegate ";
      private const string CollectionPropertyDelegateLog = "CollectionPropertyDelegateLog ";


      [TestMethod]
      public void GetValueOfSourceProperty_LoadsRequiredPropertyBeforeSourceProperty() {
         var vm = new TestVM(TestVM.ClassDescriptor);
         vm.Load(x => x.SourceProperty);
         Assert.IsTrue(vm.IsLoaded(x => x.CollectionRequiredBySourceProperty));
      }

      [TestMethod]
      public void RequiresLoadedPropertyFalse_DisablesLoadOrderDependency() {
         TestVMDescriptor descriptorWithoutDependency = VMDescriptorBuilder
            .Inherits(TestVM.ClassDescriptor)
            .OfType<TestVMDescriptor>()
            .For<TestVM>()
            .WithProperties((d, b) => { })
            .WithBehaviors(b => {
               b.Property(x => x.SourceProperty).RequiresLoadedProperty(x => x.CollectionRequiredBySourceProperty, false);
            })
            .Build();

         var vm = new TestVM(descriptorWithoutDependency);

         vm.Load(x => x.SourceProperty);
         Assert.IsFalse(vm.IsLoaded(x => x.CollectionRequiredBySourceProperty));
      }

      private class TestVM : TestViewModel<TestVMDescriptor> {
         public static readonly TestVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<TestVMDescriptor>()
            .For<TestVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.SourceProperty = v
                  .VM
                  .DelegatesTo(x => {
                     x.Log.Append(SourcePropertyDelegateLog);
                     return new ChildVM();
                  });
               d.CollectionRequiredBySourceProperty = v
                  .Collection
                  .PopulatedWith(x => {
                     x.Log.Append(CollectionPropertyDelegateLog);
                     return new[] { new ChildVM() };
                  })
                  .With(ChildVM.ClassDescriptor);
            })
            .WithBehaviors(b => {
               b.Property(x => x.SourceProperty).RequiresLoadedProperty(x => x.CollectionRequiredBySourceProperty);
            })
            .Build();

         public TestVM(TestVMDescriptor descriptor )
            : base(descriptor) {
            Log = new StringBuilder();
         }

         public StringBuilder Log { get; set; }
      }

      private class TestVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ChildVM> SourceProperty { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ChildVM>> CollectionRequiredBySourceProperty { get; set; }
      }

      private class ChildVM : TestViewModel<ChildVMDescriptor> {
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