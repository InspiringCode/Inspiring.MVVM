namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class TestBase {
      public const string ArbitraryString = "Arbitrary string!";
      public const string AnotherArbitraryString = "Another arbitrary string!";
      public const int ArbitraryInt = 42;

      protected static T Mock<T>() where T : class {
         return new Mock<T>().Object;
      }

      [TestInitialize]
      public void SetupBase() {
         ServiceLocator.SetServiceLocator(new ReflectionServiceLocator());
      }

      protected class ViewModelBehaviorContextHelper {
         public ViewModelBehaviorContextHelper() {
            //var fields = new FieldDefinitionCollection();

            var descriptor = new DescriptorStub();

            InitializationContext = new BehaviorInitializationContext(descriptor); // Is this correct?

            //var fieldValues = new Lazy<FieldValueHolder>(() =>
            //   fields.CreateValueHolder()
            //);

            VM = new ViewModelStub(descriptor);

            IBehaviorContext ctx = VM.Kernel;

            ContextMock = new Mock<IBehaviorContext>();
            ContextMock
               .Setup(x => x.FieldValues)
               .Returns(ctx.FieldValues);

            ContextMock
               .Setup(x => x.VM)
               .Returns(VM);

            Context = ContextMock.Object;
         }

         public IViewModel VM { get; private set; }

         public Mock<IBehaviorContext> ContextMock { get; private set; }

         public IBehaviorContext Context { get; private set; }

         public BehaviorInitializationContext InitializationContext { get; private set; }
      }

      protected class ContextTestHelper {
         public ContextTestHelper(IViewModel viewModel = null, IVMPropertyDescriptor property = null) {
            if (property == null) {
               property = new Mock<IVMPropertyDescriptor>().Object;
            }

            var descriptor = new DescriptorStub();

            InitializationContext = new BehaviorInitializationContext(
               descriptor,
               property
            );

            var fieldValues = new Lazy<FieldValueHolder>(() =>
               descriptor.Fields.CreateValueHolder()
            );

            VM = viewModel ?? new ViewModelStub();

            ContextMock = new Mock<IBehaviorContext>();
            ContextMock
               .Setup(x => x.FieldValues)
               .Returns(() => fieldValues.Value);

            ContextMock
               .Setup(x => x.VM)
               .Returns(VM);

            Context = ContextMock.Object;

            Property = property;
         }

         public IViewModel VM { get; private set; }

         public IVMPropertyDescriptor Property { get; private set; }

         public Mock<IBehaviorContext> ContextMock { get; private set; }

         public IBehaviorContext Context { get; private set; }

         public BehaviorInitializationContext InitializationContext { get; private set; }

         public void InitializeBehaviors(Behavior behavior) {
            // If we call 'TryCall' directly on 'behavior', 'behavior' itself
            // is ignored, even if it implements the behavior interface correctly.
            var chain = new BehaviorChain { Successor = behavior };

            chain.TryCall<IBehaviorInitializationBehavior>(
               x => x.Initialize(InitializationContext)
            );
         }
      }
   }
}