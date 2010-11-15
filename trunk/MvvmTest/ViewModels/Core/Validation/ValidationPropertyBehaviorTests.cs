namespace Inspiring.MvvmTest.ViewModels.Core.Validation {
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class ValidationPropertyBehaviorTests : TestBase {
      private PropertyBehaviorContextTestHelper _context;
      private PropertyValidationBehavior<string> _behavior;

      [TestInitialize]
      public void Setup() {
         _context = new PropertyBehaviorContextTestHelper();
         _behavior = new PropertyValidationBehavior<string>();
         _behavior.Initialize(_context.InitializationContext);
      }

      [TestMethod]
      public void Validate_WithStateParameter_CallsBehaviorContext() {
         _behavior.Validate(_context.Context, new ValidationContext());

         _context
            .ContextMock
            .Verify(
               x => x.NotifyPropertyValidating(_context.Property, It.IsAny<ValidationState>()),
               Times.Once()
            );
      }

      [TestMethod]
      public void Validate_CallsBehaviorContext() {

      }

      [TestMethod]
      public void GetValidationState_Initially_ReturnsValidState() {
         var state = _behavior.GetValidationState(_context.Context);
         Assert.AreSame(ValidationState.Valid, state);
      }

      [TestMethod]
      public void GetValidationState_AfterValidateWithoutErrors_ReturnsValidState() {

      }

      [TestMethod]
      public void GetValidationState_AfterValidateWithErrors_ReturnsInvalidState() {

      }
   }
}