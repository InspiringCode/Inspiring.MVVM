namespace Inspiring.MvvmTest.ApiTests.ViewModels.Refresh {
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class RefreshFixture : TestBase {
      protected class ChildSource {
      }

      protected class ChildVM : DefaultViewModelWithSourceBase<ChildVMDescriptor, ChildSource> {
         public static readonly ChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ChildVMDescriptor>()
            .For<ChildVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.RefreshDetectionProperty = v.Custom.CustomProperty<object>(
                  CreateRefreshDetectionPropertyChain()
               );
               d.ChildProperty = v.Property.Of<string>();

               d.Grandchild = v.VM.DelegatesTo(_ => new GrandchildVM());
               d.Grandchildren = v.Collection
                  .PopulatedWith(_ => new[] { new GrandchildVM() })
                  .With(GrandchildVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.EnableParentValidation(x => x.ChildProperty);
            })
            .Build();

         public ChildVM()
            : base(ClassDescriptor) {
         }

         public ChildVM(ChildSource source)
            : base(ClassDescriptor) {
            InitializeFrom(source);
         }

         public bool WasRefreshed { get; set; }

         private static BehaviorChainConfiguration CreateRefreshDetectionPropertyChain() {
            var key = new BehaviorKey("RefreshDetector");
            var b = new BehaviorChainConfiguration();
            b.Append(key, new RefreshDetectorBehavior());
            b.Enable(key);
            return b;
         }

         public override string ToString() {
            return "ChildVM";
         }
      }

      protected class ChildVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<GrandchildVM> Grandchild { get; set; }
         public IVMPropertyDescriptor<IVMCollection<GrandchildVM>> Grandchildren { get; set; }
         public IVMPropertyDescriptor<object> RefreshDetectionProperty { get; set; }
         public IVMPropertyDescriptor<string> ChildProperty { get; set; }
      }

      protected class GrandchildVM : ViewModel<GrandchildVMDescriptor> {
         [ClassDescriptor]
         public static readonly GrandchildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<GrandchildVMDescriptor>()
            .For<GrandchildVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
            })
            .Build();

         public GrandchildVM()
            : base(ClassDescriptor) {
         }
      }

      protected class GrandchildVMDescriptor : VMDescriptor {
      }

      private class RefreshDetectorBehavior : Behavior, IRefreshBehavior {
         public bool WasCalled { get; set; }

         public void Refresh(IBehaviorContext context, RefreshOptions options) {
            var vm = (ChildVM)context.VM;
            vm.WasRefreshed = true;
         }
      }
   }
}