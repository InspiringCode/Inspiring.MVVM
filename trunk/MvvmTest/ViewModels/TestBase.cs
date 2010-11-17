﻿namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
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

            InitializationContext = new InitializationContext(fields);

            var fieldValues = new Lazy<FieldValueHolder>(() =>
               fields.CreateValueHolder()
            );

            ContextMock = new Mock<IBehaviorContext_>();
            ContextMock
               .Setup(x => x.FieldValues)
               .Returns(() => fieldValues.Value);

            Context = ContextMock.Object;
            VM = Context.VM;
         }

         public IViewModel VM { get; private set; }

         public Mock<IBehaviorContext_> ContextMock { get; private set; }

         public IBehaviorContext_ Context { get; private set; }

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

            ContextMock = new Mock<IBehaviorContext_>();
            ContextMock
               .Setup(x => x.FieldValues)
               .Returns(() => fieldValues.Value);

            Context = ContextMock.Object;

            Property = property;

            VM = Context.VM;
         }

         public IViewModel VM { get; private set; }

         public IVMProperty Property { get; private set; }

         public Mock<IBehaviorContext_> ContextMock { get; private set; }

         public IBehaviorContext_ Context { get; private set; }

         public InitializationContext InitializationContext { get; private set; }
      }
   }
}