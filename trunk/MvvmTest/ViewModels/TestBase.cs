namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class TestBase {
      public const string ArbitraryString = "Arbitrary string!";
      public const string AnotherArbitraryString = "Another arbitrary string!";

      protected static T Mock<T>() where T : class {
         return new Mock<T>().Object;
      }

      protected class ViewModelBehaviorContextHelper {
         public ViewModelBehaviorContextHelper() {
            var fields = new FieldDefinitionCollection();

            InitializationContext = new BehaviorInitializationContext(new VMDescriptorStub()); // Is this correct?

            var fieldValues = new Lazy<FieldValueHolder>(() =>
               fields.CreateValueHolder()
            );

            ContextMock = new Mock<IBehaviorContext>();
            ContextMock
               .Setup(x => x.FieldValues)
               .Returns(() => fieldValues.Value);

            Context = ContextMock.Object;
            VM = Context.VM;
         }

         public IViewModel VM { get; private set; }

         public Mock<IBehaviorContext> ContextMock { get; private set; }

         public IBehaviorContext Context { get; private set; }

         public BehaviorInitializationContext InitializationContext { get; private set; }
      }

      protected class ContextTestHelper {
         public ContextTestHelper(IVMProperty property = null) {
            if (property == null) {
               property = new Mock<IVMProperty>().Object;
            }

            var descriptor = new VMDescriptorStub();

            InitializationContext = new BehaviorInitializationContext(
               descriptor,
               property
            );

            var fieldValues = new Lazy<FieldValueHolder>(() =>
               descriptor.Fields.CreateValueHolder()
            );

            VM = new ViewModelStub();

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

         public IVMProperty Property { get; private set; }

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