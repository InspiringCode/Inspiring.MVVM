namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.Stubs;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class TestBase {
      protected static T Mock<T>() where T : class {
         return new Mock<T>().Object;
      }

      protected class ViewModelBehaviorContextHelper {
         public ViewModelBehaviorContextHelper() {
            var fields = new FieldDefinitionCollection();

            InitializationContext = new BehaviorInitializationContext( new VMDescriptorStub()); // Is this correct?

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

      protected class PropertyBehaviorContextTestHelper {
         public PropertyBehaviorContextTestHelper(IVMProperty property = null) {
            if (property == null) {
               property = new Mock<IVMProperty>().Object;
            }

            var fields = new FieldDefinitionCollection();

            InitializationContext = new BehaviorInitializationContext(
               new VMDescriptorStub(), // TODO: Is this correct? Fields and so?
               property
            );

            var fieldValues = new Lazy<FieldValueHolder>(() =>
               fields.CreateValueHolder()
            );

            ContextMock = new Mock<IBehaviorContext>();
            ContextMock
               .Setup(x => x.FieldValues)
               .Returns(() => fieldValues.Value);

            Context = ContextMock.Object;

            Property = property;

            VM = Context.VM;
         }

         public IViewModel VM { get; private set; }

         public IVMProperty Property { get; private set; }

         public Mock<IBehaviorContext> ContextMock { get; private set; }

         public IBehaviorContext Context { get; private set; }

         public BehaviorInitializationContext InitializationContext { get; private set; }
      }
   }
}