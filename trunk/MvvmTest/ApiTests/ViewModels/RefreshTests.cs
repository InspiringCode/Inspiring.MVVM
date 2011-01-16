namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class RefreshTests {
      private TestVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new TestVM();
      }

      [TestMethod]
      public void Refresh_OfMappedProperty_ValidatesPropertyAndRaisesPropertyChanged() {
         VM.Refresh(TestVM.ClassDescriptor.MappedProperty);

         Assert.IsTrue(VM.MappedPropertyWasValidated, "Property was not validated.");
         Assert.IsTrue(VM.PropertyChangedWasRaisedForMappedProperty, "PropertyChanged was not raised.");
      }

      [TestMethod]
      public void Refresh_OfLocalProperty_DoesNothing() {
         VM.Refresh(TestVM.ClassDescriptor.LocalProperty);

         Assert.IsFalse(VM.LocalPropertyWasValidated, "Property was validated.");
         Assert.IsFalse(VM.PropertyChangedWasRaisedForLocalProperty, "PropertyChanged was raised.");
      }

      [TestMethod]
      public void Refresh_OfWrappingVM_UpdatesSourceAndRefreshesVM() {
         var newSource = new ChildSourceObject();
         VM.PreloadDescendants();
         VM.WrappingVMSource = newSource;
         VM.Refresh(TestVM.ClassDescriptor.WrappingVM);

         Assert.AreSame(newSource, VM.WrappingVM.Source, "Source of VM was not updated.");
         Assert.IsTrue(VM.WrappingVM.WasRefreshed, "VM was not refreshed.");
      }

      [TestMethod]
      public void Refresh_OfDelegatedVM_CallsGetterAndRevalidatesVM() {
         VM.PreloadDescendants();
         VM.Refresh(TestVM.ClassDescriptor.DelegatedVM);

         Assert.IsTrue(VM.DelegatedVMGetterWasCalled, "Getter was not called.");
         Assert.IsTrue(VM.DelegatedVM.WasValidated, "VM was not revalidated.");
         Assert.IsFalse(VM.DelegatedVM.WasRefreshed, "VM should not be refreshed (unnecessary).");
      }

      [TestMethod]
      public void Refresh_OfLocalVM_RefreshesVM() {
         VM.PreloadDescendants();
         VM.Refresh(TestVM.ClassDescriptor.LocalVM);

         Assert.IsTrue(VM.LocalVM.WasRefreshed, "VM was not refreshed.");
      }

      [TestMethod]
      public void Refresh_OfWrappingCollection_RepopulatesAndValidatesItems() {
         VM.PreloadDescendants();
         VM.WrappingCollectionSource.Add(new ChildSourceObject());
         VM.Refresh(TestVM.ClassDescriptor.WrappingCollection);

         Assert.AreEqual(1, VM.WrappingColletion.Count, "Collection was not repopulated.");

         ChildVM item = VM.WrappingColletion.Single();
         Assert.IsTrue(item.WasValidated, "Items were not validated.");
         Assert.IsFalse(item.WasRefreshed, "Items should not be refreshed (unnecessary).");
      }

      [TestMethod]
      public void Refresh_OfPopulatedColletion_RepopulatesAndValidatesItems() {
         var item = new ChildVM();
         VM.PreloadDescendants();
         VM.PopulatedColletion.Add(item);
         VM.Refresh(TestVM.ClassDescriptor.PopulatedCollection);

         Assert.IsTrue(VM.PopulatedCollectionWasRepopulated, "Collection was not repopulated.");
         Assert.IsTrue(item.WasValidated, "Items were not validated.");
         Assert.IsFalse(item.WasRefreshed, "Items should not be refreshed (unnecessary).");
      }

      [TestMethod]
      public void Refresh_OfLocalCollection_RefreshesAllItems() {
         var item = new ChildVM();
         VM.PreloadDescendants();
         VM.LocalCollection.Add(item);
         VM.Refresh(TestVM.ClassDescriptor.LocalCollection);

         Assert.IsTrue(item.WasRefreshed, "Items were not refreshed.");
      }

      [TestMethod]
      public void Refresh_PerformsViewModelValidation() {
         VM.PreloadDescendants();
         VM.Refresh();

         Assert.IsTrue(VM.ViewModelValidatorWasCalled);
      }

      [TestMethod]
      public void Refresh_DoesNotLoadWrappingVM() {
         VM.Refresh();
         Assert.IsFalse(VM.IsLoaded(TestVM.ClassDescriptor.WrappingVM));
      }

      [TestMethod]
      public void Refresh_DoesNotLoadDelegatedVM() {
         VM.Refresh();
         Assert.IsFalse(VM.IsLoaded(TestVM.ClassDescriptor.DelegatedVM));
      }

      [TestMethod]
      public void Refresh_DoesNotLoadWrappingCollection() {
         VM.Refresh();
         Assert.IsFalse(VM.IsLoaded(TestVM.ClassDescriptor.WrappingCollection));
      }

      [TestMethod]
      public void Refresh_DoesNotLoadPopulatedCollection() {
         VM.Refresh();
         Assert.IsFalse(VM.IsLoaded(TestVM.ClassDescriptor.PopulatedCollection));
      }

      public sealed class TestVM : ViewModel<TestVMDescriptor> {
         public static readonly TestVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<TestVMDescriptor>()
            .For<TestVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.MappedProperty = v.Property.MapsTo(x => x.MappedPropertySource);
               d.LocalProperty = v.Property.Of<string>();

               d.WrappingVM = v.VM.Wraps(x => x.WrappingVMSource).With<ChildVM>();
               d.DelegatedVM = v.VM.DelegatesTo(vm => {
                  vm.DelegatedVMGetterWasCalled = true;
                  return new ChildVM();
               });
               d.LocalVM = v.VM.Of<ChildVM>();

               d.WrappingCollection = v
                  .Collection
                  .Wraps(x => x.WrappingCollectionSource)
                  .With<ChildVM>(ChildVM.ClassDescriptor);

               d.PopulatedCollection = v
                  .Collection
                  .PopulatedWith(vm => {
                     vm.PopulatedCollectionWasRepopulated = true;
                     return new ChildVM[] { };
                  })
                  .With(ChildVM.ClassDescriptor);

               d.LocalCollection = v.Collection.Of<ChildVM>(ChildVM.ClassDescriptor);

            })
            .WithValidators(b => {
               b.Check(x => x.MappedProperty).Custom((vm, value, args) => {
                  vm.MappedPropertyWasValidated = true;
               });
               b.Check(x => x.LocalProperty).Custom((vm, value, args) => {
                  vm.LocalPropertyWasValidated = true;
               });

               b.CheckViewModel((vm, args) => {
                  if (args.ChangedProperty == null) {
                     vm.ViewModelValidatorWasCalled = true;
                  }
               });
            })
            .Build();

         public TestVM()
            : base(ClassDescriptor) {

            WrappingVMSource = new ChildSourceObject();
            WrappingCollectionSource = new List<ChildSourceObject>();

            SetValue(Descriptor.LocalVM, new ChildVM());
         }


         // FLAGS

         public bool MappedPropertyWasValidated { get; set; }

         public bool LocalPropertyWasValidated { get; set; }

         public bool PropertyChangedWasRaisedForMappedProperty { get; set; }

         public bool PropertyChangedWasRaisedForLocalProperty { get; set; }

         public bool DelegatedVMGetterWasCalled { get; set; }

         public bool PopulatedCollectionWasRepopulated { get; set; }

         public bool ViewModelValidatorWasCalled { get; set; }

         // WRAPPER PROPERTIES

         public ChildVM WrappingVM {
            get { return GetValue(Descriptor.WrappingVM); }
         }

         public ChildVM DelegatedVM {
            get { return GetValue(Descriptor.DelegatedVM); }
         }

         public ChildVM LocalVM {
            get { return GetValue(Descriptor.LocalVM); }
         }

         public IVMCollection<ChildVM> WrappingColletion {
            get { return GetValue(Descriptor.WrappingCollection); }
         }

         public IVMCollection<ChildVM> PopulatedColletion {
            get { return GetValue(Descriptor.PopulatedCollection); }
         }

         public IVMCollection<ChildVM> LocalCollection {
            get { return GetValue(Descriptor.LocalCollection); }
         }


         // SOURCE PROPERTIES

         public string MappedPropertySource { get; set; }

         public ChildSourceObject WrappingVMSource { get; set; }

         public List<ChildSourceObject> WrappingCollectionSource { get; set; }


         // METHODS

         public void Refresh(IVMPropertyDescriptor property) {
            base.Refresh(property, RefreshScope.SelfOnly);
         }

         public void PreloadDescendants() {
            Load(Descriptor.WrappingVM);
            Load(Descriptor.DelegatedVM);
            Load(Descriptor.LocalVM);

            Load(Descriptor.WrappingCollection);
            Load(Descriptor.PopulatedCollection);
            Load(Descriptor.LocalCollection);

            DelegatedVMGetterWasCalled = false;
            PopulatedCollectionWasRepopulated = false;
         }

         public void Refresh() {
            base.Refresh(RefreshScope.SelfOnly);
         }

         public bool IsLoaded(IVMPropertyDescriptor property) {
            return property.Behaviors.IsLoadedNext(GetContext());
         }

         protected override void OnPropertyChanged(IVMPropertyDescriptor property) {
            base.OnPropertyChanged(property);

            if (property == Descriptor.MappedProperty) {
               PropertyChangedWasRaisedForMappedProperty = true;
            }

            if (property == Descriptor.LocalProperty) {
               PropertyChangedWasRaisedForLocalProperty = true;
            }
         }
      }

      public sealed class ChildVM : ViewModel<ChildVMDescriptor>, IHasSourceObject<ChildSourceObject> {
         public static readonly ChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ChildVMDescriptor>()
            .For<ChildVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Property = v.Property.Of<string>();
            })
            .WithValidators(b => {
               b.Check(x => x.Property).Custom((vm, value, args) => {
                  vm.WasValidated = true;
               });
            })
            .Build();

         public ChildVM()
            : base(ClassDescriptor) {
         }

         public ChildSourceObject Source { get; set; }


         public bool WasValidated { get; set; }

         public bool WasRefreshed { get; set; }


         public void ResetFlags() {
            WasValidated = false;
            WasRefreshed = false;
         }

         protected override void OnPropertyChanged(IVMPropertyDescriptor property) {
            base.OnPropertyChanged(property);

            if (property == Descriptor.Property) {
               WasRefreshed = true;
            }
         }
      }

      public sealed class TestVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> MappedProperty { get; set; }
         public IVMPropertyDescriptor<string> LocalProperty { get; set; }

         public IVMPropertyDescriptor<ChildVM> WrappingVM { get; set; }
         public IVMPropertyDescriptor<ChildVM> DelegatedVM { get; set; }
         public IVMPropertyDescriptor<ChildVM> LocalVM { get; set; }

         public IVMPropertyDescriptor<IVMCollection<ChildVM>> WrappingCollection { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ChildVM>> PopulatedCollection { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ChildVM>> LocalCollection { get; set; }
      }

      public sealed class ChildVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Property { get; set; }
      }

      public class ChildSourceObject {
      }
   }
}