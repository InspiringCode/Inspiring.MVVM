namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class TestBase {
      protected class ViewModelBehaviorContextHelper {
         public ViewModelBehaviorContextHelper() {
            var fields = new FieldDefinitionCollection();

            InitializationContext = new InitializationContext(fields);

            var fieldValues = new Lazy<FieldValueHolder>(() =>
               fields.CreateValueHolder()
            );

            ContextMock = new Mock<IViewModelBehaviorContext>();
            ContextMock
               .Setup(x => x.FieldValues)
               .Returns(() => fieldValues.Value);

            Context = ContextMock.Object;
            VM = Context.VM;
         }

         public IViewModel VM { get; private set; }

         public Mock<IViewModelBehaviorContext> ContextMock { get; private set; }

         public IViewModelBehaviorContext Context { get; private set; }

         public InitializationContext InitializationContext { get; private set; }
      }

      protected class PropertyBehaviorContextTestHelper {
         public PropertyBehaviorContextTestHelper(IVMProperty property = null) {
            if (property == null) {
               property = new Mock<IVMProperty>().Object;
            }

            var fields = new FieldDefinitionCollection();

            InitializationContext = new InitializationContext(
               fields,
               property
            );

            var fieldValues = new Lazy<FieldValueHolder>(() =>
               fields.CreateValueHolder()
            );

            ContextMock = new Mock<IPropertyBehaviorContext>();
            ContextMock
               .Setup(x => x.FieldValues)
               .Returns(() => fieldValues.Value);

            Context = ContextMock.Object;

            Property = property;

            VM = Context.VM;
         }

         public IViewModel VM { get; private set; }

         public IVMProperty Property { get; private set; }

         public Mock<IPropertyBehaviorContext> ContextMock { get; private set; }

         public IPropertyBehaviorContext Context { get; private set; }

         public InitializationContext InitializationContext { get; private set; }
      }
   }
}