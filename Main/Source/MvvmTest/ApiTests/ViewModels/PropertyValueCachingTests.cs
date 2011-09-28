namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class PropertyValueCachingTests {
      private const string OldValue = "Old value";
      private const string NewValue = "New value";

      private TestVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new TestVM();
      }

      [TestMethod]
      public void GetValue_OfMappedProperty_AccessesSourceOnlyOnce() {
         VM.MappedPropertySource = OldValue;
         Assert.AreEqual(0, VM.MappedPropertyGetterCalls);

         string value = VM.MappedProperty;
         Assert.AreEqual(1, VM.MappedPropertyGetterCalls);

         value = VM.MappedProperty;
         Assert.AreEqual(1, VM.MappedPropertyGetterCalls);
      }

      [TestMethod]
      public void GetValue_OfDelegatingProperty_AccessesSourceOnlyOnce() {
         VM.DelegatingPropertySource = OldValue;
         Assert.AreEqual(0, VM.DelegatingPropertyGetterCalls);

         string value = VM.DelegatingProperty;
         Assert.AreEqual(1, VM.DelegatingPropertyGetterCalls);

         value = VM.DelegatingProperty;
         Assert.AreEqual(1, VM.DelegatingPropertyGetterCalls);
      }

      [TestMethod]
      public void SetValue_OfMappedProperty_SetsSource() {
         string value = VM.MappedProperty;
         VM.MappedProperty = NewValue;
         Assert.AreEqual(NewValue, VM.MappedPropertySource);
      }

      [TestMethod]
      public void SetValue_OfDelegatingProperty_SetsSource() {
         string value = VM.DelegatingProperty;
         VM.DelegatingProperty = NewValue;
         Assert.AreEqual(NewValue, VM.DelegatingPropertySource);
      }

      [TestMethod]
      public void GetAfterSet_OfMappedProperty_GetsValueFromSourceAgain() {
         string value = VM.MappedProperty;

         VM.MappedProperty = OldValue;
         Assert.AreEqual(1, VM.MappedPropertyGetterCalls);
         Assert.AreEqual(1, VM.MappedPropertySetterCalls);

         VM.MappedPropertySource = NewValue;
         Assert.AreEqual(1, VM.MappedPropertyGetterCalls);

         value = VM.MappedProperty;
         Assert.AreEqual(NewValue, value);
         Assert.AreEqual(2, VM.MappedPropertyGetterCalls);
      }

      [TestMethod]
      public void GetAfterSet_OfDelegatingProperty_GetsValueFromSourceAgain() {
         string value = VM.DelegatingProperty;

         VM.DelegatingProperty = OldValue;
         Assert.AreEqual(1, VM.DelegatingPropertyGetterCalls);
         Assert.AreEqual(1, VM.DelegatingPropertySetterCalls);

         VM.DelegatingPropertySource = NewValue;
         Assert.AreEqual(1, VM.DelegatingPropertyGetterCalls);

         value = VM.DelegatingProperty;
         Assert.AreEqual(NewValue, value);
         Assert.AreEqual(2, VM.DelegatingPropertyGetterCalls);
      }

      [TestMethod]
      public void GetAndSetValue_OfInstanceProperty_Works() {
         Assert.IsNull(VM.InstanceProperty);
         VM.InstanceProperty = NewValue;
         Assert.AreEqual(NewValue, VM.InstanceProperty);
      }

      [TestMethod]
      public void Refresh_OfDelegatingProperty_ClearsCache() {
         VM.DelegatingPropertySource = OldValue;
         string value = VM.DelegatingProperty;
         Assert.AreEqual(1, VM.DelegatingPropertyGetterCalls);

         VM.DelegatingPropertySource = NewValue;
         VM.Refresh(x => x.DelegatingProperty);
         Assert.AreEqual(1, VM.DelegatingPropertyGetterCalls);

         value = VM.DelegatingProperty;
         Assert.AreEqual(NewValue, value);
         Assert.AreEqual(2, VM.DelegatingPropertyGetterCalls);
      }

      [TestMethod]
      public void Refresh_OfMappedProperty_ClearsCache() {
         VM.MappedPropertySource = OldValue;
         string value = VM.MappedProperty;
         Assert.AreEqual(1, VM.MappedPropertyGetterCalls);

         VM.MappedPropertySource = NewValue;
         VM.Refresh(x => x.MappedProperty);
         Assert.AreEqual(1, VM.MappedPropertyGetterCalls);

         value = VM.MappedProperty;
         Assert.AreEqual(NewValue, value);
         Assert.AreEqual(2, VM.MappedPropertyGetterCalls);
      }

      private sealed class TestVM : TestViewModel<TestVMDescriptor> {
         public static readonly TestVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<TestVMDescriptor>()
            .For<TestVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.InstanceProperty = v.Property.Of<string>();
               d.DelegatingProperty = v.Property.DelegatesTo(
                  (vm) => vm.DelegatingPropertySourceCore,
                  (vm, val) => vm.DelegatingPropertySourceCore = val
               );
               d.MappedProperty = v.Property.MapsTo(x => x.MappedPropertySourceCore);
            })
            .WithBehaviors(b => {
               b.Property(x => x.MappedProperty).IsCached();
               b.Property(x => x.DelegatingProperty).IsCached();
               b.Property(x => x.InstanceProperty).IsCached();
            })
            .Build();

         public TestVM()
            : base(ClassDescriptor) {
         }

         public string InstanceProperty {
            get { return GetValue(Descriptor.InstanceProperty); }
            set { SetValue(Descriptor.InstanceProperty, value); }
         }

         public string DelegatingProperty {
            get { return GetValue(Descriptor.DelegatingProperty); }
            set { SetValue(Descriptor.DelegatingProperty, value); }
         }

         public string MappedProperty {
            get { return GetValue(Descriptor.MappedProperty); }
            set { SetValue(Descriptor.MappedProperty, value); }
         }

         public string DelegatingPropertySource { get; set; }
         public string MappedPropertySource { get; set; }

         public int DelegatingPropertyGetterCalls { get; set; }
         public int DelegatingPropertySetterCalls { get; set; }

         public int MappedPropertyGetterCalls { get; set; }
         public int MappedPropertySetterCalls { get; set; }

         private string MappedPropertySourceCore {
            get {
               MappedPropertyGetterCalls++;
               return MappedPropertySource;
            }
            set {
               MappedPropertySetterCalls++;
               MappedPropertySource = value;
            }
         }

         private string DelegatingPropertySourceCore {
            get {
               DelegatingPropertyGetterCalls++;
               return DelegatingPropertySource;
            }
            set {
               DelegatingPropertySetterCalls++;
               DelegatingPropertySource = value;
            }
         }
      }

      private sealed class TestVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> InstanceProperty { get; set; }
         public IVMPropertyDescriptor<string> DelegatingProperty { get; set; }
         public IVMPropertyDescriptor<string> MappedProperty { get; set; }
      }
   }
}