namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class TestBase {


      protected class PropertyBehaviorContextTestHelper {
         public PropertyBehaviorContextTestHelper(IVMProperty property = null) {
            if (property == null) {
               property = new Mock<IVMProperty>().Object;
            }

            var fields = new FieldDefinitionCollection();

            InitializationContext = new PropertyBehaviorInitializationContext(
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
         }

         public IVMProperty Property { get; private set; }

         public Mock<IPropertyBehaviorContext> ContextMock { get; private set; }

         public IPropertyBehaviorContext Context { get; private set; }

         public PropertyBehaviorInitializationContext InitializationContext { get; private set; }
      }
   }
}