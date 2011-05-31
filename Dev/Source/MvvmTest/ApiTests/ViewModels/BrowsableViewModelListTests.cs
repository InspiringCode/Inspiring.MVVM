namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class BrowsableViewModelListTests {
      [TestMethod]
      public void Constructor_WhenListTypeDoesNotSpecifyClassDescriptorAttributes_ThrowsException() {
         AssertHelper.Throws<ArgumentException>(() =>
            new BrowsableViewModelList<ViewModelWithoutClassDescriptorVM>()
         )
         .WithMessage(EViewModels.BrowsableListRequiresClassDescriptorAttribute);
      }

      [TestMethod]
      public void GetItemProperties_WhenListAccessorsIsNull_ReturnsPropertyDescriptorsOfClassDescriptor() {
         var list = new BrowsableViewModelList<EmployeeVM>();
         var actualProperties = list.GetItemProperties(listAccessors: null);

         var expectedProperties = EmployeeVM.ClassDescriptor.Properties;
         CollectionAssert.AreEqual(GetExpectations(expectedProperties), GetExpectations(actualProperties));
      }

      [TestMethod]
      public void GetItemProperties_WhenListAccessorsContainsDescendantPropertyDescriptor_ReturnsPropertyDescriptorsOfDescendantClassDescriptor() {
         var list = new BrowsableViewModelList<EmployeeVM>();
         var actualProperties = list.GetItemProperties(listAccessors: GetListAccessorsForManagerAddress(list));

         var expectedProperties = AddressVM.ClassDescriptor.Properties;
         CollectionAssert.AreEqual(GetExpectations(expectedProperties), GetExpectations(actualProperties));
      }

      [TestMethod]
      public void GetItemProperties_WhenListAccessorsContainsCollectionPropertyDescriptor_ReturnsPropertyDescriptorsOfItemClassDescriptor() {
         var list = new BrowsableViewModelList<EmployeeVM>();
         var actualProperties = list.GetItemProperties(listAccessors: GetListAccessorsForAddresses(list));

         var expectedProperties = AddressVM.ClassDescriptor.Properties;
         CollectionAssert.AreEqual(GetExpectations(expectedProperties), GetExpectations(actualProperties));
      }

      [TestMethod]
      public void GetItemProperties_WhenListAccessorsContainsSimpleType_ReturnsPropertyDescriptorsOfThatType() {
         var list = new BrowsableViewModelList<EmployeeVM>();
         var actualProperties = list.GetItemProperties(listAccessors: GetListAccessorsForSimpleTypeProperty(list));

         var expectedProperties = TypeDescriptor.GetProperties(typeof(SimpleType));
         CollectionAssert.AreEqual(GetExpectations(expectedProperties), GetExpectations(actualProperties));
      }

      [TestMethod]
      public void GetItemProperties_WhenListAccessorsIsNull_AlwaysReturnsSameInstance() {
         var list = new BrowsableViewModelList<EmployeeVM>();
         var first = list.GetItemProperties(listAccessors: null);
         var second = list.GetItemProperties(listAccessors: null);

         Assert.AreSame(first, second);
      }

      [TestMethod]
      public void GetItemProperties_WhenListAccessorsContainsDescendantPropertyAccessor_AlwaysReturnsSameInstance() {
         var list = new BrowsableViewModelList<EmployeeVM>();

         var first = list.GetItemProperties(GetListAccessorsForManagerAddress(list));
         var second = list.GetItemProperties(GetListAccessorsForManagerAddress(list));

         Assert.AreSame(first, second);
      }

      // PLEASE clean us up. Maybe make a nice generic method for 

      [TestMethod]
      public void GetItemProperties_ForAViewModelCollectionPropertyOfAnObjectWithoutClassDescriptorAttribute_ReturnsViewModelProperties() {
         var list = new BrowsableViewModelList<ViewModelWithClassDescriptorVM>();

         PropertyDescriptorCollection withDescProperties = list.GetItemProperties(null);
         PropertyDescriptor withoutAttributePropertyDescriptor = withDescProperties["ViewModelWithoutAttribute"];

         PropertyDescriptor withAttributePropertyDescriptor = list
            .GetItemProperties(new[] { withoutAttributePropertyDescriptor })["ViewModelCollectionWithAttribute"];


         PropertyDescriptorCollection withAttributeProperties = list
            .GetItemProperties(new[] { withoutAttributePropertyDescriptor, withAttributePropertyDescriptor });

         var expectedProperties = ViewModelWithClassDescriptorVM.ClassDescriptor.Properties;
         CollectionAssert.AreEqual(GetExpectations(expectedProperties), GetExpectations(withAttributeProperties));
      }

      [TestMethod]
      public void GetItemProperties_ForAViewModelPropertyOfAnObjectWithoutClassDescriptorAttribute_ReturnsViewModelProperties() {
         var list = new BrowsableViewModelList<ViewModelWithClassDescriptorVM>();

         PropertyDescriptorCollection withDescProperties = list.GetItemProperties(null);
         PropertyDescriptor withoutAttributePropertyDescriptor = withDescProperties["ViewModelWithoutAttribute"];

         PropertyDescriptor withAttributePropertyDescriptor = list
            .GetItemProperties(new[] { withoutAttributePropertyDescriptor })["ViewModelWithAttribute"];


         PropertyDescriptorCollection withAttributeProperties = list
            .GetItemProperties(new[] { withoutAttributePropertyDescriptor, withAttributePropertyDescriptor });

         var expectedProperties = ViewModelWithClassDescriptorVM.ClassDescriptor.Properties;
         CollectionAssert.AreEqual(GetExpectations(expectedProperties), GetExpectations(withAttributeProperties));
      }

      [TestMethod]
      public void Constructor_WithItemArray_PopulatesCollection() {
         var item1 = new EmployeeVM();
         var item2 = new EmployeeVM();

         var list = new BrowsableViewModelList<EmployeeVM>(item1, item2);
         CollectionAssert.AreEqual(new[] { item1, item2 }, list);
      }

      [TestMethod]
      public void Add_OnEmptyCollection_AddsItem() {
         var list = new BrowsableViewModelList<EmployeeVM>();
         var newItem = new EmployeeVM();
         list.Add(newItem);

         CollectionAssert.AreEqual(new[] { newItem }, list);
      }

      [TestMethod]
      public void Add_OnPopulatedCollection_AddsItem() {
         var initialItem = new EmployeeVM();
         var list = new BrowsableViewModelList<EmployeeVM>(initialItem);
         var newItem = new EmployeeVM();
         list.Add(newItem);

         CollectionAssert.AreEqual(new[] { initialItem, newItem }, list);
      }

      [TestMethod]
      public void Constructor_WithEnumerable_PopulatesCollection() {
         IEnumerable<EmployeeVM> source = new[] { new EmployeeVM(), new EmployeeVM() };
         var list = new BrowsableViewModelList<EmployeeVM>(source);
         CollectionAssert.AreEqual(source.ToArray(), list);
      }

      private static PropertyDescriptor[] GetListAccessorsForSimpleTypeProperty(BrowsableViewModelList<EmployeeVM> list) {
         var employeeProperties = list.GetItemProperties(null);
         var simpleTypePropertyDescriptor = employeeProperties["SimpleTypeProperty"];
         return new[] { simpleTypePropertyDescriptor };
      }

      private static PropertyDescriptor[] GetListAccessorsForManagerAddress(BrowsableViewModelList<EmployeeVM> list) {
         PropertyDescriptorCollection employeeProperties = list.GetItemProperties(null);
         PropertyDescriptor managerPropertyDescriptor = employeeProperties["Manager"];

         PropertyDescriptorCollection managerProperties = list
            .GetItemProperties(new[] { managerPropertyDescriptor });
         PropertyDescriptor addressPropertyDescriptor = managerProperties["Address"];

         return new[] { managerPropertyDescriptor, addressPropertyDescriptor };
      }

      private static PropertyDescriptor[] GetListAccessorsForAddresses(BrowsableViewModelList<EmployeeVM> list) {
         PropertyDescriptorCollection employeeProperties = list.GetItemProperties(null);
         PropertyDescriptor addressesPropertyDescriptor = employeeProperties["Addresses"];

         return new[] { addressesPropertyDescriptor };
      }

      private PropertyDescriptorExpectation[] GetExpectations(PropertyDescriptorCollection collection) {
         return collection
            .Cast<PropertyDescriptor>()
            .Select(x => new PropertyDescriptorExpectation {
               Name = x.Name,
               PropertyType = x.PropertyType
            })
            .ToArray();
      }

      private PropertyDescriptorExpectation[] GetExpectations(IEnumerable<IVMPropertyDescriptor> properties) {
         return properties
            .Select(x => new PropertyDescriptorExpectation {
               Name = x.PropertyName,
               PropertyType = x.PropertyType
            })
            .ToArray();
      }

      private struct PropertyDescriptorExpectation {
         public string Name { get; set; }
         public Type PropertyType { get; set; }

         public override string ToString() {
            return String.Format(
               "{{PropertyDescriptor Name={0}, PropertyType={1}}}",
               Name,
               PropertyType
            );
         }
      }


      private class ViewModelWithClassDescriptorVM : ViewModel<ViewModelWithClassDescriptorVMDescriptor> {
         [ClassDescriptor]
         public static readonly ViewModelWithClassDescriptorVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ViewModelWithClassDescriptorVMDescriptor>()
            .For<ViewModelWithClassDescriptorVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.ViewModelWithoutAttribute = v.VM.Of<ViewModelWithoutClassDescriptorVM>();
            })
            .Build();

         public ViewModelWithClassDescriptorVM()
            : base(ClassDescriptor) {
         }
      }

      private class ViewModelWithClassDescriptorVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ViewModelWithoutClassDescriptorVM> ViewModelWithoutAttribute { get; set; }
      }

      private class ViewModelWithoutClassDescriptorVM : ViewModel<ViewModelWithoutClassDescriptorVMDescriptor> {
         public static readonly ViewModelWithoutClassDescriptorVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ViewModelWithoutClassDescriptorVMDescriptor>()
            .For<ViewModelWithoutClassDescriptorVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.ViewModelCollectionWithAttribute = v
                  .Collection
                  .Of<ViewModelWithClassDescriptorVM>(ViewModelWithClassDescriptorVM.ClassDescriptor);

               d.ViewModelWithAttribute = v.VM.Of<ViewModelWithClassDescriptorVM>();
            })
            .Build();

         public ViewModelWithoutClassDescriptorVM()
            : base(ClassDescriptor) {
         }

         public ViewModelWithClassDescriptorVM ViewModelWithAttribute {
            get { return GetValue(Descriptor.ViewModelWithAttribute); }
         }

         public IVMCollection<ViewModelWithClassDescriptorVM> ViewModelCollectionWithAttribute {
            get { return GetValue(Descriptor.ViewModelCollectionWithAttribute); }
         }
      }

      private class ViewModelWithoutClassDescriptorVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ViewModelWithClassDescriptorVM>> ViewModelCollectionWithAttribute { get; set; }
         public IVMPropertyDescriptor<ViewModelWithClassDescriptorVM> ViewModelWithAttribute { get; set; }
      }

      private class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         [ClassDescriptor]
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Name = v.Property.Of<string>();
               d.Manager = v.Property.Of<EmployeeVM>();
               d.Address = v.VM.Of<AddressVM>();
               d.Addresses = v.Collection.Of<AddressVM>(AddressVM.ClassDescriptor);
               d.SimpleTypeProperty = v.Property.Of<SimpleType>();
            })
            .Build();

         public EmployeeVM()
            : base(ClassDescriptor) {
         }
      }

      private class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<AddressVM> Address { get; set; }
         public IVMPropertyDescriptor<IVMCollection<AddressVM>> Addresses { get; set; }
         public IVMPropertyDescriptor<EmployeeVM> Manager { get; set; }
         public IVMPropertyDescriptor<SimpleType> SimpleTypeProperty { get; set; }
      }

      private class AddressVM : ViewModel<AddressVMDescriptor> {
         [ClassDescriptor]
         public static readonly AddressVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<AddressVMDescriptor>()
            .For<AddressVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Street = v.Property.Of<string>();
            })
            .Build();

         public AddressVM()
            : base(ClassDescriptor) {
         }
      }

      private class AddressVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Street { get; set; }
      }

      private class SimpleType {
         public string SimpleProperty { get; set; }
      }
   }
}