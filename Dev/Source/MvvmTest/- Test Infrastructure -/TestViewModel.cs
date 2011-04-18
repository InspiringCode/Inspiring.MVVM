namespace Inspiring.MvvmTest {
   using Inspiring.Mvvm.ViewModels;

   public class TestViewModel<TDescriptor> :
      ViewModel<TDescriptor>
      where TDescriptor : VMDescriptorBase {

      public TestViewModel(TDescriptor descriptor)
         : base(descriptor) {
      }

      public new void Revalidate() {
         base.Revalidate();
      }
   }
}
