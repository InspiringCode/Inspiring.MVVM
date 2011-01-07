//namespace Inspiring.MvvmTest.ViewModels.TempTests {
//   using System;
//   using System.Linq;
//   using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Inspiring.Mvvm.ViewModels;

//   [TestClass]
//   public class CollectionPropertyTest {
//      [TestMethod]
//      public void TestMethod1() {
//         TestVM vm = new TestVM();
//         TestVMSource source = new TestVMSource();
//         source.AddChild(new ChildVMSource());
//         source.AddChild(new ChildVMSource());

//         vm.Source = source;

//         Assert.AreEqual(vm, vm.MappedParentedCollectionAccessor[0].Source.Parent);
//         Assert.AreEqual(vm, vm.MappedParentedCollectionAccessor[1].Source.Parent);

//         var child = new ParentedChildVM();
//         child.InitializeFrom(
//            new SourceWithParent<TestVM, ChildVMSource>(vm, new ChildVMSource())
//         );

//         vm.MappedParentedCollectionAccessor.Add(child);

//         Assert.AreEqual(vm, vm.MappedParentedCollectionAccessor[2].Source.Parent);
//      }


//   }
//}