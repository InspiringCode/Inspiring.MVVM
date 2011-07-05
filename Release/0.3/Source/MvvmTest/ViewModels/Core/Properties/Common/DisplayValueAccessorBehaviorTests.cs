namespace Inspiring.MvvmTest.ViewModels.Behaviors {
   using System;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;
   using Moq;

   [TestClass]
   public class DisplayValueAccessorBehaviorTests : TestBase {
      private DisplayValueAccessorBehavior<int> _behavior;
      private FieldDefinitionCollection _dynamicFields;
      private FieldValueHolder _fieldValues;
      private InnerAccessor _innerAccessor;
      private IBehaviorContext _context;

      [TestInitialize]
      public void Setup() {
         _dynamicFields = new FieldDefinitionCollection();
         _fieldValues = _dynamicFields.CreateValueHolder();

         _innerAccessor = new InnerAccessor();

         _behavior = new DisplayValueAccessorBehavior<int>();
         //((IBehavior)_behavior).Initialize(new BehaviorInitializationContext("Test", _dynamicFields));
         _behavior.Successor = _innerAccessor;

         var mock = new Mock<IBehaviorContext>();
         mock.Setup(x => x.FieldValues).Returns(_fieldValues);
         _context = mock.Object;
      }

      [TestMethod]
      public void CheckGetValue() {
         Assert.AreEqual(0, _behavior.GetDisplayValue(_context));

         _innerAccessor.Value = 42;
         Assert.AreEqual(42, _behavior.GetDisplayValue(_context));
      }

      [TestMethod]
      public void CheckSetValue() {
         _behavior.SetDisplayValue(_context, 42);
         Assert.AreEqual(42, _innerAccessor.Value);

         Assert.AreEqual(42, _behavior.GetDisplayValue(_context));
      }

      [TestMethod]
      public void CheckSetNullValue() {
         AssertHelper.Throws<ArgumentException>(() =>
            _behavior.SetDisplayValue(_context, null)
         ).Containing("type");
      }

      // [TestMethod] // TODO
      public void CheckSetWithChangeRequest() {
         //var mock = new Mock<IVMValueConverter<int>>(MockBehavior.Strict);
         //mock.Setup(x => x.ConvertBack("Fourtytwo")).Returns(ConversionResult.Success(42));
         //mock.Setup(x => x.ConvertBack("Invalid")).Returns(ConversionResult.Failure<int>("Test"));
         //mock.Setup(x => x.ConvertBack("Eleven")).Returns(ConversionResult.Success(11));

         //var r = new ChangeValueRequest<int>("Fourtytwo", mock.Object);
         //_behavior.SetDisplayValue(_context, r);
         //Assert.AreEqual(_innerAccessor.Value, 42);

         //r = new ChangeValueRequest<int>("Invalid", mock.Object);
         //_behavior.SetDisplayValue(_context, r);

         //ValidationResult result = _behavior.GetValidationResult(_context);
         //Assert.IsNotNull(result);
         //Assert.IsFalse(result.Successful);
         //Assert.AreEqual("Test", result.ErrorMessage);

         //Assert.AreEqual(42, _behavior.GetDisplayValue(_context));
         //Assert.AreEqual(42, _innerAccessor.Value);

         //r = new ChangeValueRequest<int>("Eleven", mock.Object);
         //_behavior.SetDisplayValue(_context, r);
         //Assert.AreEqual(11, _behavior.GetDisplayValue(_context));
         //Assert.AreEqual(11, _innerAccessor.Value);

         //result = _behavior.GetValidationResult(_context);
         //Assert.IsNotNull(result);
         //Assert.IsTrue(result.Successful);
         //Assert.AreEqual(null, result.ErrorMessage);
      }

      private class InnerAccessor : IValueAccessorBehavior<int> {
         public int Value { get; set; }

         public int GetValue(IBehaviorContext vm) {
            return Value;
         }

         public void SetValue(IBehaviorContext vm, int value) {
            Value = value;
         }

         public IBehavior Successor {
            get { return null; }
            set { }
         }


         public void Initialize(BehaviorInitializationContext context) {
         }
      }


   }
}
