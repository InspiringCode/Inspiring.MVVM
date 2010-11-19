namespace Inspiring.MvvmTest.ViewModels.IntegrationTests {

   //[TestClass]
   //public class ValidationTest {
   //   private TestVMSource _source;
   //   private TestVM _vm;

   //   [TestInitialize]
   //   public void Setup() {
   //      _source = new TestVMSource();
   //      _source.MappedMutableValue = "Test";
   //      _source.ChildValue = new ChildVMSource { MappedMutableValue = "Childtest" };
   //      _source.AddChild(new ChildVMSource { MappedMutableValue = "Item 1" });
   //      _source.AddChild(new ChildVMSource { MappedMutableValue = "Item 2" });
   //      _source.SetCalculated(42);

   //      _vm = new TestVM();
   //      _vm.Source = _source;
   //   }
   //   [TestMethod]
   //   public void TestDefault() {
   //      Assert.IsTrue(_vm.IsValid(true));
   //   }

   //   [TestMethod]
   //   public void TestParentError() {
   //      _vm.ViewModelValidationResult = ValidationResult.Failure("Test");
   //      _vm.Revalidate();
   //      Assert.IsFalse(_vm.IsValid(true));
   //   }

   //   [TestMethod]
   //   public void TestParentPropertyError() {
   //      _vm.LocalPropertyValidationResult = ValidationResult.Failure("Test");
   //      Assert.IsFalse(_vm.IsValid(true));
   //   }

   //   [TestMethod]
   //   public void TestChildError() {
   //      _vm.MappedCollectionAccessor[0].MappedMutablePropertyValidationResult = ValidationResult.Failure("Test");
   //      Assert.IsFalse(_vm.IsValid(true));
   //   }

   //   [TestMethod]
   //   public void TestValidateParentOnly() {
   //      _vm.MappedCollectionAccessor[0].MappedMutablePropertyValidationResult = ValidationResult.Failure("Test");
   //      Assert.IsTrue(_vm.IsValid(false));
   //   }

   //   [TestMethod]
   //   public void Revalidate() {
   //      bool validationSuccessful = true;
   //      Action<ValidationEventArgs> validator = a => {
   //         if (!validationSuccessful) {
   //            a.AddError("Error");
   //         }
   //      };

   //      var config = BehaviorConfigurationFactory.CreateConfiguration();
   //      config.Enable(VMBehaviorKey.Validator);
   //      config.Configure(
   //         VMBehaviorKey.Validator,
   //         (ValidationBehavior<string> x) => x.Add(validator)
   //      );

   //      config.OverrideFactory(VMBehaviorKey.PropertyValueAcessor, new InstancePropertyBehavior<string>());


   //      VMDescriptor descriptor = new VMDescriptor();
   //      var contextMock = MockObjectFactory.MockBehaviorContext(descriptor);
   //      var property = MockObjectFactory.MockProperty<string>(config, descriptor);

   //      int invocationCount = 0;
   //      contextMock
   //         .Setup(x => x.RaiseValidationStateChanged(It.IsAny<VMPropertyBase<string>>()))
   //         .Callback(() => invocationCount++);

   //      var context = contextMock.Object;

   //      property.SetValue(context, "Wert");

   //      property.Revalidate(context);
   //      Assert.AreEqual(0, invocationCount);
   //      Assert.IsTrue(property.GetValidationResult(context).Successful);

   //      validationSuccessful = false;
   //      Assert.IsTrue(property.GetValidationResult(context).Successful);
   //      property.Revalidate(context);
   //      Assert.AreEqual(1, invocationCount);
   //      Assert.IsFalse(property.GetValidationResult(context).Successful);

   //      validationSuccessful = true;
   //      Assert.IsFalse(property.GetValidationResult(context).Successful);
   //      property.Revalidate(context);
   //      Assert.AreEqual(2, invocationCount);
   //      Assert.IsTrue(property.GetValidationResult(context).Successful);
   //   }
   //}
}