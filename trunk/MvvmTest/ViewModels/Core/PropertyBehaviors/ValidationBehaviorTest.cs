namespace Inspiring.MvvmTest.ViewModels.Behaviors {
   using System;
   using Inspiring.Mvvm;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ValidationBehaviorTest {
      private ValidationBehavior<string> _behavior;
      private Mock<IBehaviorContext> _contextMock;
      private VMProperty<string> _property;
      private ViewModel _vm;

      [TestInitialize]
      public void Setup() {
         _behavior = new ValidationBehavior<string>();
         _behavior.Add(args => {
            if (((string)args.PropertyValue).Contains("Invalid")) {
               args.AddError("Error 1");
            }

            args.AffectsOtherItems = true;
         });

         _behavior.Add(args => {
            if (((string)args.PropertyValue).Contains("Invalid")) {
               args.AddError("Error 2");
            }

            args.AffectsOtherItems = false;
         });

         _behavior.Add(args => { });

         _behavior.Successor = new Mock<IAccessPropertyBehavior<string>>().Object;

         _property = new Mock<VMProperty<string>>().Object;

         FieldDefinitionCollection fields = new FieldDefinitionCollection();

         ((IBehavior)_behavior).Initialize(
            new BehaviorInitializationContext(fields, _property)
         );

         _vm = new Mock<ViewModel>(ServiceLocator.Current).Object;

         _contextMock = new Mock<IBehaviorContext>();
         _contextMock.Setup(x => x.FieldValues).Returns(fields.CreateValueHolder());
         _contextMock.Setup(x => x.VM).Returns(_vm);
      }

      [TestMethod]
      public void SetValue() {
         Action<ValidationEventArgs> argsAssertions = e => {
            Assert.AreSame(_property, e.Property);
            Assert.AreEqual("Invalid", e.PropertyValue);
            Assert.AreSame(_vm, e.VM);
            Assert.IsTrue(e.AffectsOtherItems);
            Assert.IsTrue(e.Errors.Contains("Error 1"));
            Assert.IsTrue(e.Errors.Contains("Error 2"));
         };

         _contextMock
            .Setup(x => x.OnValidating(It.IsAny<ValidationEventArgs>()))
            .Callback<ValidationEventArgs>(argsAssertions);

         _contextMock
            .Setup(x => x.OnValidated(It.IsAny<ValidationEventArgs>()))
            .Callback<ValidationEventArgs>(argsAssertions);

         _behavior.SetValue(_contextMock.Object, "Invalid");

         _contextMock.Verify(
            x => x.OnValidating(It.IsAny<ValidationEventArgs>()),
            Times.Once()
         );

         _contextMock.Verify(
            x => x.OnValidated(It.IsAny<ValidationEventArgs>()),
            Times.Once()
         );
      }

      [TestMethod]
      public void SetValue_ValidationStateChanged() {
         _behavior.SetValue(_contextMock.Object, "Vaild 1");
         _behavior.SetValue(_contextMock.Object, "Valid 2");
         _contextMock.Verify(
            x => x.ValidationStateChanged(_property),
            Times.Never()
         );

         _behavior.SetValue(_contextMock.Object, "Invalid 1");
         _behavior.SetValue(_contextMock.Object, "Invalid 2");
         _contextMock.Verify(
            x => x.ValidationStateChanged(_property),
            Times.Once()
         );

         _behavior.SetValue(_contextMock.Object, "Valid 3");
         _behavior.SetValue(_contextMock.Object, "Valid 4");
         _contextMock.Verify(
            x => x.ValidationStateChanged(_property),
            Times.Exactly(2)
         );
      }
   }
}