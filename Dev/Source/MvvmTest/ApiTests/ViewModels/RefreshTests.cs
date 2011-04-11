namespace Inspiring.MvvmTest.ApiTests.ViewModels {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class RefreshTests : TestBase {
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
      public void Refresh_OfWrappingVM_RecreatesAndValidatesVM() {
         var newSource = new ChildSourceObject();
         VM.PreloadDescendants();
         VM.WrappingVMSource = newSource;
         VM.Refresh(TestVM.ClassDescriptor.WrappingVM);

         Assert.AreSame(newSource, VM.WrappingVM.Source, "Source of VM was not updated.");
         Assert.IsTrue(VM.WrappingVM.WasValidated, "VM was not refreshed.");
      }

      [TestMethod]
      public void Refresh_OfDelegatedVM_CallsGetterAndRevalidatesVM() {
         VM.PreloadDescendants();
         VM.Refresh(TestVM.ClassDescriptor.DelegatedVM);

         Assert.IsTrue(VM.DelegatedVMGetterWasCalled, "Getter was not called.");
         Assert.IsTrue(VM.DelegatedVM.WasValidated, "VM was not revalidated.");
         Assert.Inconclusive("Should VMs also be refreshed or not? Unnecessary?");
         //Assert.IsFalse(VM.DelegatedVM.WasRefreshed, "VM should not be refreshed (unnecessary).");
      }

      [TestMethod]
      public void Refresh_OfDelegatedVM_RaisesPropertyChangeEvent() {
         VM.PreloadDescendants();
         VM.Refresh(TestVM.ClassDescriptor.DelegatedVM);

         Assert.IsTrue(VM.PropertyChangedWasRaisedForDelegatedVMProperty);
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
         VM.PopulatedCollectionPopulationResult = new[] { item };
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

      // This does not work by design right now!
      //[TestMethod]
      //public void Refresh_DoesNotLoadDescendantsOfWrappingVM() {
      //   VM.Refresh();
      //   Assert.IsFalse(VM.WrappingVM.DescendantsWereLoaded);
      //}

      // This does not work by design right now!
      //[TestMethod]
      //public void Refresh_DoesNotLoadDescendantsOfDelegatedVM() {
      //   VM.Refresh();
      //   Assert.IsFalse(VM.DelegatedVM.DescendantsWereLoaded);
      //}

      [TestMethod]
      public void Refresh_DoesNotLoadDescendantsOfWrappingCollection() {
         VM.WrappingCollectionSource.Add(new ChildSourceObject());
         VM.Refresh();
         Assert.IsFalse(VM.WrappingColletion.First().DescendantsWereLoaded);
      }

      [TestMethod]
      public void Refresh_DoesNotLoadDescendantsOfPopulatedCollection() {
         VM.Refresh();
         Assert.IsFalse(VM.PopulatedColletion.First().DescendantsWereLoaded);
      }

      [TestMethod]
      public void Refresh_OfUnloadedDelegatedVMProperty_PerformsViewModelPropertyValidations() {
         VM.Refresh(TestVM.ClassDescriptor.DelegatedVM);
         Assert.IsTrue(VM.DelegatedVMPropertyWasValidated);
      }

      [TestMethod]
      public void Refresh_OfDelegatedVMPropertyThatReturnsNull_DoesNotThrow() {
         VM.DelegatedVMFactory = () => null;
         VM.Refresh(TestVM.ClassDescriptor.DelegatedVM);
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
                  return vm.DelegatedVMFactory();
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
                     return vm.PopulatedCollectionPopulationResult;
                  })
                  .With(ChildVM.ClassDescriptor);

               d.LocalCollection = v.Collection.Of<ChildVM>(ChildVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.Check(x => x.MappedProperty).Custom(args => {
                  args.Owner.MappedPropertyWasValidated = true;
               });
               b.Check(x => x.LocalProperty).Custom(args => {
                  args.Owner.LocalPropertyWasValidated = true;
               });
               b.Check(x => x.DelegatedVM).Custom(args => {
                  args.Owner.DelegatedVMPropertyWasValidated = true;
               });

               b.CheckViewModel(args => {
                  args.Owner.ViewModelValidatorWasCalled = true;
               });
            })
            .Build();

         public TestVM()
            : base(ClassDescriptor) {

            PopulatedCollectionPopulationResult = new[] { new ChildVM() };

            WrappingVMSource = new ChildSourceObject();
            WrappingCollectionSource = new List<ChildSourceObject>();

            DelegatedVMFactory = () => new ChildVM();

            SetValue(Descriptor.LocalVM, new ChildVM());
         }

         public IEnumerable<ChildVM> PopulatedCollectionPopulationResult { get; set; }

         // FLAGS

         public bool MappedPropertyWasValidated { get; set; }

         public bool LocalPropertyWasValidated { get; set; }

         public bool DelegatedVMPropertyWasValidated { get; set; }

         public bool PropertyChangedWasRaisedForMappedProperty { get; set; }

         public bool PropertyChangedWasRaisedForLocalProperty { get; set; }

         public bool PropertyChangedWasRaisedForDelegatedVMProperty { get; set; }

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

         public Func<ChildVM> DelegatedVMFactory { get; set; }

         // METHODS

         public new void Refresh(IVMPropertyDescriptor property) {
            base.Refresh(property);
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

         public new void Refresh() {
            base.Refresh();
         }

         protected override void OnPropertyChanged(IVMPropertyDescriptor property) {
            base.OnPropertyChanged(property);

            if (property == Descriptor.MappedProperty) {
               PropertyChangedWasRaisedForMappedProperty = true;
            }

            if (property == Descriptor.LocalProperty) {
               PropertyChangedWasRaisedForLocalProperty = true;
            }

            if (property == Descriptor.DelegatedVM) {
               PropertyChangedWasRaisedForDelegatedVMProperty = true;
            }
         }
      }

      public sealed class ChildVM : ViewModel<ChildVMDescriptor>, IHasSourceObject<ChildSourceObject> {
         public static readonly ChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ChildVMDescriptor>()
            .For<ChildVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Property = v.Property.MapsTo(x => x.PropertySource);
               d.GrandchildVM = v.VM.DelegatesTo(vm => new GrandchildVM());
            })
            .WithValidators(b => {
               b.Check(x => x.Property).Custom(args => {
                  args.Owner.WasValidated = true;
               });
            })
            .Build();

         public ChildVM()
            : base(ClassDescriptor) {
         }

         public ChildSourceObject Source { get; set; }

         public bool WasValidated { get; set; }

         public bool WasRefreshed { get; set; }

         public bool DescendantsWereLoaded {
            get {
               return Descriptor.GrandchildVM.Behaviors.IsLoadedNext(GetContext());
            }
         }

         private string PropertySource { get; set; }

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
         public IVMPropertyDescriptor<GrandchildVM> GrandchildVM { get; set; }
      }

      public class ChildSourceObject {
      }

      public sealed class GrandchildVM : ViewModel<GrandchildVMDescriptor> {
         public static readonly GrandchildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<GrandchildVMDescriptor>()
            .For<GrandchildVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Name = v.Property.Of<string>();
            })
            .Build();

         public GrandchildVM()
            : base(ClassDescriptor) {
         }
      }

      public sealed class GrandchildVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
      }
   }
}