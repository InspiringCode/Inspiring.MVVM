namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System;
   using System.ComponentModel;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class TypeDescriptorTests : TestBase {
      private const string ArbitraryCommentValue = "Test";
      private const int ArbitraryRatingValue = 10;

      private MovieReviewVM _viewModel;

      [TestInitialize]
      public void Setup() {
         _viewModel = new MovieReviewVM();
      }

      [TestMethod]
      public void GetProperties_ReturnsOnlyVMDescriptorProperties() {
         // Act
         PropertyDescriptorCollection propertyDescriptors = TypeDescriptor.GetProperties(_viewModel);

         // Assert
         var actual = propertyDescriptors
            .Cast<PropertyDescriptor>()
            .Select(pd => new PropertyDescriptorCO(pd))
            .ToArray();

         var expected = new PropertyDescriptorCO[] { 
            new PropertyDescriptorCO("Rating", typeof(int)),
            new PropertyDescriptorCO("Comment", typeof(string)),
            new PropertyDescriptorCO("PropertySupportingConversion", typeof(object)),
            new PropertyDescriptorCO("PropertyNotSupportingConversion", typeof(string))
         };

         CollectionAssert.AreEquivalent(expected, actual);
      }

      [TestMethod]
      public void GetValue_WhereNoClrPropertyExists_ReturnsValue() {
         _viewModel.RatingTestAccessor = ArbitraryRatingValue;

         PropertyDescriptor pd = GetPropertyDescriptor("Rating");
         object propertyValue = pd.GetValue(_viewModel);

         Assert.AreEqual(ArbitraryRatingValue, propertyValue);
      }

      [TestMethod]
      public void GetValue_WhereClrPropretyWithSameNameExists_ClrPropertyIsNotInvoked() {
         _viewModel.CommentTestAccessor = ArbitraryCommentValue;

         PropertyDescriptor pd = GetPropertyDescriptor("Comment");
         object propertyValue = pd.GetValue(_viewModel);

         Assert.AreEqual(ArbitraryCommentValue, propertyValue);
      }

      [TestMethod]
      public void SetValue_SetsVMProperty() {
         PropertyDescriptor pd = GetPropertyDescriptor("Comment");
         pd.SetValue(_viewModel, ArbitraryCommentValue);

         Assert.AreEqual(ArbitraryCommentValue, _viewModel.CommentTestAccessor);
      }

      [TestMethod]
      public void AddValueChanged_WhenPropertyChanges_CallbackIsInvoked() {
         int callbackInvocationCount = 0;
         object actualSender = null;
         string actualValueInCallback = null;

         EventHandler callback = (s, _) => {
            callbackInvocationCount++;
            actualSender = s;
            actualValueInCallback = _viewModel.CommentTestAccessor;
         };

         PropertyDescriptor pd = GetPropertyDescriptor("Comment");
         pd.AddValueChanged(_viewModel, callback);

         var newPropertyValue = ArbitraryCommentValue;
         _viewModel.CommentTestAccessor = newPropertyValue;

         Assert.AreNotEqual(0, callbackInvocationCount, "The callback was not invoked.");
         Assert.AreEqual(1, callbackInvocationCount, "The callback should not be invoked more than once.");
         Assert.AreSame(_viewModel, actualSender, "The sender of the event handler should be the view model.");
         Assert.AreEqual(newPropertyValue, actualValueInCallback, "GetValue should already return the new property value in the callback.");
      }

      [TestMethod]
      public void GetProperties_WhenViewModelTypeWithClassDescriptorAttributeIsPassed_ReturnsProperties() {
         var actualProperties = TypeDescriptor.GetProperties(typeof(ViewModelWithClassDescriptorAttributeVM));

         ICustomTypeDescriptor customTypeDescriptor = new ViewModelWithClassDescriptorAttributeVM();
         var expectedProperties = customTypeDescriptor.GetProperties();

         Assert.AreSame(expectedProperties, actualProperties);
      }

      [TestMethod]
      public void GetProperties_WhenViewModelTypeWithoutClassDescriptorAttributeIsPassed_ReturnsClrProperties() {
         var actualProperties = TypeDescriptor.GetProperties(typeof(MovieReviewVM));

         var propertyNames = actualProperties
            .Cast<PropertyDescriptor>()
            .Select(x => x.Name)
            .ToArray();

         CollectionAssert.Contains(propertyNames, "ClrProperty");
      }

      [TestMethod]
      public void PropertyType_WhenDispalyValueConversionIsEnabled_ReturnsObject() {
         PropertyDescriptor pd = GetPropertyDescriptor("PropertySupportingConversion");
         Assert.AreEqual(typeof(object), pd.PropertyType);
      }

      [TestMethod]
      public void PropertyType_WhenDispalyValueConversionIsDisabled_ReturnsActualType() {
         PropertyDescriptor pd = GetPropertyDescriptor("PropertyNotSupportingConversion");
         Assert.AreEqual(typeof(string), pd.PropertyType);
      }

      /// <summary>
      ///   Calls 'TypeDescriptor.GetProperties' and returns the 'PropertyDescriptor' 
      ///   with the specified 'propertyName'.
      /// </summary>
      private PropertyDescriptor GetPropertyDescriptor(string propertyName) {
         PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(_viewModel)[propertyName];

         Assert.IsNotNull(
            propertyDescriptor,
            "The TypeDescriptor does not contain the property '{0}'.",
            propertyName
         );

         return propertyDescriptor;
      }

      /// <summary>
      ///   Compares the name and type of a property descriptor.
      /// </summary>
      private class PropertyDescriptorCO : ComparisonObject {
         public PropertyDescriptorCO(string propertyName, Type propertyType) {
            SetEqualityProperties(() => propertyName, () => propertyType);
         }

         public PropertyDescriptorCO(PropertyDescriptor descriptor)
            : this(descriptor.Name, descriptor.PropertyType) {
         }
      }

      /// <summary>
      ///   Example VM.
      /// </summary>
      private sealed class MovieReviewVM : ViewModel<MovieReviewVMDescriptor> {
         public static readonly MovieReviewVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<MovieReviewVMDescriptor>()
            .For<MovieReviewVM>()
            .WithProperties((d, c) => {
               var vm = c.GetPropertyBuilder();

               d.Rating = vm.Property.Of<int>();
               d.Comment = vm.Property.Of<string>();

               d.PropertySupportingConversion = vm.Property.Of<string>();
               d.PropertyNotSupportingConversion = vm.Property.Of<string>();
            })
            .WithBehaviors(b => {
               b.Property(x => x.PropertySupportingConversion).SupportsDisplayValueConversion();
               b.Property(x => x.PropertyNotSupportingConversion).SupportsDisplayValueConversion(false);
            })
            .Build();

         public MovieReviewVM()
            : base(ClassDescriptor) {
         }

         public string Comment {
            get {
               Assert.Fail("A type descriptor should never access a CLR property.");
               return default(string);
            }
            set {
               Assert.Fail("A type descriptor should never access a CLR property.");
            }
         }

         public int RatingTestAccessor {
            get { return GetValue(Descriptor.Rating); }
            set { SetValue(Descriptor.Rating, value); }
         }

         public string CommentTestAccessor {
            get { return GetValue(Descriptor.Comment); }
            set { SetValue(Descriptor.Comment, value); }
         }

         public string ClrProperty { get; set; }
      }

      private sealed class MovieReviewVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<int> Rating { get; set; }
         public IVMPropertyDescriptor<string> Comment { get; set; }

         public IVMPropertyDescriptor<string> PropertySupportingConversion { get; set; }
         public IVMPropertyDescriptor<string> PropertyNotSupportingConversion { get; set; }
      }

      [TypeDescriptionProvider(typeof(ViewModelTypeDescriptionProvider))]
      private class ViewModelWithClassDescriptorAttributeVM : ViewModel<ViewModelWithClassDescriptorAttributeVMDescriptor> {
         [ClassDescriptor]
         public static readonly ViewModelWithClassDescriptorAttributeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ViewModelWithClassDescriptorAttributeVMDescriptor>()
            .For<ViewModelWithClassDescriptorAttributeVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.DummyProperty = v.Property.Of<string>();
            })
            .Build();

         public ViewModelWithClassDescriptorAttributeVM()
            : base(ClassDescriptor) {
         }
      }

      private class ViewModelWithClassDescriptorAttributeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> DummyProperty { get; set; }
      }
   }
}