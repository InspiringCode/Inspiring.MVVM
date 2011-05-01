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
            })
            .Build();

         public ChildVM()
            : base(ClassDescriptor) {
         }

         public bool WasRefreshed {
            get { return GetDetector().WasCalled; }
            set { GetDetector().WasCalled = value; }
         }

         private RefreshDetectorBehavior GetDetector() {
            return Descriptor
               .RefreshDetectionProperty
               .Behaviors
               .GetNextBehavior<RefreshDetectorBehavior>();
         }

         private static BehaviorChainConfiguration CreateRefreshDetectionPropertyChain() {
            var key = new BehaviorKey("RefreshDetector");
            var b = new BehaviorChainConfiguration();
            b.Append(key, new RefreshDetectorBehavior());
            b.Enable(key);
            return b;
         }
      }

      protected class ChildVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<object> RefreshDetectionProperty { get; set; }
      }

      private class RefreshDetectorBehavior : Behavior, IRefreshBehavior {
         public bool WasCalled { get; set; }

         public void Refresh(IBehaviorContext context) {
            WasCalled = true;
         }
      }
   }
}