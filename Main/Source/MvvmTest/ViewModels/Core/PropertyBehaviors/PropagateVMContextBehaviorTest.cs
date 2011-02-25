namespace Inspiring.MvvmTest.ViewModels.Behaviors {

   //[TestClass]
   //public class PropagateVMContextBehaviorTest {
   //   [TestMethod]
   //   public void CreateInstance() {
   //      var vm = new PersonVM();

   //      var vmContext = new Mock<IVMContext>().Object;

   //      var behaviorContextMock = new Mock<IBehaviorContext>(MockBehavior.Strict);
   //      behaviorContextMock.Setup(x => x.VMContext).Returns(vmContext);

   //      var realFactoryMock = new Mock<IViewModelFactoryBehavior<PersonVM>>(MockBehavior.Strict);
   //      realFactoryMock.Setup(x => x.CreateInstance(behaviorContextMock.Object)).Returns(vm);

   //      Assert.IsNull(vm.VMContext);

   //      IViewModelFactoryBehavior<PersonVM> propagator = new PropagateVMContextBehavior<PersonVM>();
   //      propagator.Successor = realFactoryMock.Object;

   //      propagator.CreateInstance(behaviorContextMock.Object);
   //      Assert.AreSame(vmContext, vm.VMContext);
   //   }
   //}
}