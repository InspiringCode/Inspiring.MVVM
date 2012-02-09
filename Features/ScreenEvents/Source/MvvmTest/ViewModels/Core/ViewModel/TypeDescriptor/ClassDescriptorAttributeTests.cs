namespace Inspiring.MvvmTest.ViewModels.Core.ViewModel.TypeDescriptor {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ClassDescriptorAttributeTests {
      [TestMethod]
      public void GetClassDescriptor_ForTypeWithClassDescriptorAttribute_ReturnsClassDescriptor() {
         var actual = ClassDescriptorAttribute.GetClassDescriptorOf(typeof(ViewModelWithClassDescriptor));
         Assert.AreEqual(ViewModelWithClassDescriptor.ClazzDescriptor, actual);
      }

      [TestMethod]
      public void GetClassDescriptor_ForTypeWithoutClassDescriptorAttribute_ReturnsNull() {
         var actual = ClassDescriptorAttribute.GetClassDescriptorOf(typeof(ViewModelWithoutClassDescriptor));
         Assert.IsNull(actual);
      }

      [TestMethod]
      public void GetClassDescriptor_WhenOnlyBaseTypeHasAttribute_ReturnsClassDescriptor() {
         var actual = ClassDescriptorAttribute.GetClassDescriptorOf(typeof(DerivedViewModel));
         Assert.AreEqual(ViewModelWithClassDescriptor.ClazzDescriptor, actual);
      }

      private class ViewModelWithClassDescriptor : ViewModel<TestDescriptor> {
         [ClassDescriptor]
         public static readonly TestDescriptor ClazzDescriptor = new TestDescriptor();
      }

      private class DerivedViewModel : ViewModelWithClassDescriptor { }

      private class ViewModelWithoutClassDescriptor : ViewModel<TestDescriptor> {
         public static readonly TestDescriptor ClassDescriptor = new TestDescriptor();
      }

      private class TestDescriptor : VMDescriptor { }
   }
}