namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System.Text;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class LoadOrderTests {
      private const string SourcePropertyDelegateLog = "SourcePropertyDelegate ";
      private const string CollectionPropertyDelegateLog = "CollectionPropertyDelegateLog ";

      private TestVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new TestVM();
      }

      [TestMethod]
      public void GetValueOfSourceProperty_LoadsRequiredPropertyBeforeSourceProperty() {
         VM.Load(x => x.SourceProperty);
         Assert.IsTrue(VM.IsLoaded(x => x.CollectionRequiredBySourceProperty));
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

         public TestVM()
            : base(ClassDescriptor) {
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