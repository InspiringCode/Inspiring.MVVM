namespace Inspiring.MvvmTest.ApiTests.ViewModels.Refresh {
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Tracing;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionPropertyRefreshTests : RefreshFixture {
      private RootVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new RootVM();
         VM.Revalidate(ValidationScope.SelfAndLoadedDescendants); // Enable validation
      }

      [TestMethod]
      public void Refresh_OfInstanceCollectionProperty_CallsRefreshOnAllItems() {
         ChildVM item = new ChildVM();
         VM.InstanceProperty.Add(item);

         VM.Refresh(x => x.InstanceProperty);
         Assert.IsTrue(item.WasRefreshed);
      }

      [TestMethod]
      public void Refresh_OfInstanceCollectionProperty_DoesNotCallNotifyChangeForCollectionProperty() {
         VM.Load(x => x.InstanceProperty);
         Assert.IsFalse(VM.NotifyChangeInvocations.Any());
      }

      [TestMethod]
      public void Refresh_OfWrapperCollectionProperty_RepopulatesCollectionReusingItemVMsForSameSourceItems() {
         var sharedSource = new ChildSource();
         var reusedVM = new ChildVM(sharedSource);
         var previousVM = new ChildVM(new ChildSource());

         VM.WrapperProperty.Add(reusedVM);
         VM.WrapperProperty.Add(previousVM);

         var newSource = new ChildSource();
         VM.WrapperPropertySource = new List<ChildSource> { sharedSource, newSource };
         VM.Refresh(x => x.WrapperProperty);

         Assert.AreEqual(2, VM.WrapperProperty.Count);
         Assert.AreEqual(reusedVM, VM.WrapperProperty[0]);
         Assert.AreEqual(newSource, VM.WrapperProperty[1].Source);
      }

      [TestMethod]
      public void Refresh_OfWrapperCollectionProperty_RefreshesReusedItemVMs() {
         RefreshTrace.StartTrace();

         var sharedSource = new ChildSource();
         var reusedVM = new ChildVM(sharedSource);
         var previousVM = new ChildVM(new ChildSource());

         VM.WrapperProperty.Add(reusedVM);
         VM.WrapperProperty.Add(previousVM);

         var newSource = new ChildSource();
         VM.WrapperPropertySource = new List<ChildSource> { sharedSource, newSource };
         VM.Refresh(x => x.WrapperProperty);

         Assert.AreEqual(2, VM.WrapperProperty.Count);
         Assert.IsTrue(VM.WrapperProperty[0].WasRefreshed);
         Assert.IsFalse(VM.WrapperProperty[1].WasRefreshed);

         RefreshTrace.WriteToFile("D:\\trace.txt");
      }

      [TestMethod]
      public void Refresh_OfWrapperCollectionProperty_UsesReferenceEqualityComparisonOfSourceObjectsToDetermineReusability() {
         var oldSourceItem = new ChildSourceWithEquals("Identical ID");
         var newSourceItem = new ChildSourceWithEquals("Identical ID");

         VM.WrapperPropertySource = new[] { oldSourceItem };
         var oldItemVM = VM.WrapperProperty[0];

         VM.WrapperPropertySource = new[] { newSourceItem };
         VM.Refresh(x => x.WrapperProperty);

         var newItemVM = VM.WrapperProperty[0];
         Assert.AreNotSame(oldItemVM, newItemVM);
      }

      [TestMethod]
      public void Refresh_OfWrapperCollectionProperty_CallsNotifyChangeForCollectionPopulation() {
         VM.WrapperProperty.Add(new ChildVM(new ChildSource()));
         var oldItems = VM.WrapperProperty.ToArray();
         VM.NotifyChangeInvocations.Clear();

         VM.WrapperPropertySource = new List<ChildSource> { new ChildSource() };
         VM.Refresh(x => x.WrapperProperty);

         var newItems = VM.WrapperProperty.ToArray();

         var expectedChangeArgs = new[] {
            ChangeArgs
               .ItemsRemoved(VM.WrapperProperty, oldItems)
               .PrependViewModel(VM),
            ChangeArgs
               .CollectionPopulated(VM.WrapperProperty)
               .PrependViewModel(VM)
         };

         DomainAssert.AreEqual(expectedChangeArgs, VM.NotifyChangeInvocations);
      }

      [TestMethod]
      public void Refresh_OfUnloadedWrapperCollectionProperty_DoesNothing() {
         VM.Refresh(x => x.WrapperProperty);
         Assert.IsFalse(VM.IsLoaded(x => x.WrapperProperty));
      }

      [TestMethod]
      public void Refresh_OfPopulatedCollectionProperty_RepopulatesCollectionByCallingItemProviderDelegate() {
         VM.Load(x => x.PopulatedProperty);

         var newResult = new List<ChildVM> { new ChildVM() };
         VM.PopulatedPropertyResult = newResult;

         VM.Refresh(x => x.PopulatedProperty);
         CollectionAssert.AreEqual(newResult, VM.PopulatedProperty);
      }

      [TestMethod]
      public void Refresh_OfPopulatedCollectionProperty_CallsNotifyChangeForCollectionPopulation() {
         var oldItems = new[] { new ChildVM() };
         VM.PopulatedPropertyResult = oldItems;
         VM.Load(x => x.PopulatedProperty);
         VM.NotifyChangeInvocations.Clear();

         var newItems = new[] { new ChildVM() };
         VM.PopulatedPropertyResult = newItems;
         VM.Refresh(x => x.PopulatedProperty);

         var expectedChangeArgs = new[] {
            ChangeArgs
               .ItemsRemoved(VM.PopulatedProperty, oldItems)
               .PrependViewModel(VM),
            ChangeArgs
               .CollectionPopulated(VM.PopulatedProperty)
               .PrependViewModel(VM)
         };

         DomainAssert.AreEqual(expectedChangeArgs, VM.NotifyChangeInvocations);
      }

      [TestMethod]
      public void Refresh_OfPopulatedCollectionProperty_DoesNotRefreshReturnedItems() {
         var itemVM = new ChildVM();
         VM.PopulatedPropertyResult = new[] { itemVM };
         VM.Load(x => x.PopulatedProperty);

         VM.Refresh(x => x.PopulatedProperty);
         Assert.IsFalse(itemVM.WasRefreshed);
      }

      [TestMethod]
      public void Refresh_OfUnloadedPopulatedCollectionProperty_DoesNothing() {
         VM.Refresh(x => x.PopulatedProperty);
         Assert.IsFalse(VM.IsLoaded(x => x.PopulatedProperty));
      }

      [TestMethod]
      public void Refresh_OfCollectionProperty_RevalidatesItems() {
         ParameterizedTest
            .TestCase("InstanceProperty", new Func<RootVMDescriptor, IVMPropertyDescriptor<IVMCollection<ChildVM>>>(x => x.InstanceProperty))
            .TestCase("WrapperProperty", x => x.WrapperProperty)
            .TestCase("PopulatedProperty", x => x.PopulatedProperty)
            .Run(propertySelector => {
               var collection = VM.GetValue(propertySelector);
               var item = new ChildVM(new ChildSource());
               collection.Add(item);
               VM.PopulatedPropertyResult = new List<ChildVM> { item };

               VM.ValidatorResults.Reset();
               VM.Refresh(propertySelector);
               VM.ValidatorResults.VerifySetupValidationResults();

               VM.ValidatorResults.SetupFailing().CollectionPropertyValidation
                  .Targeting(item, x => x.ChildProperty)
                  .On(VM);
               VM.Refresh(propertySelector);
               VM.ValidatorResults.VerifySetupValidationResults();
            });
      }

      [TestMethod]
      public void Refresh_OfCollectionProperty_CallCollectionValidatorOnlyOnceForAllItems() {
         ParameterizedTest
            .TestCase("InstanceProperty", new Func<RootVMDescriptor, IVMPropertyDescriptor<IVMCollection<ChildVM>>>(x => x.InstanceProperty))
            .TestCase("WrapperProperty", x => x.WrapperProperty)
            .TestCase("PopulatedProperty", x => x.PopulatedProperty)
            .Run(propertySelector => {
               var collection = VM.GetValue(propertySelector);
               var firstItem = new ChildVM(new ChildSource());
               var secondItem = new ChildVM(new ChildSource());
               collection.Add(firstItem);
               collection.Add(secondItem);
               VM.PopulatedPropertyResult = new List<ChildVM> { firstItem, secondItem };

               VM.ValidatorResults.Reset();
               VM.ValidatorResults.ExpectInvocationOf.CollectionPropertyValidation
                  .Targeting(collection, x => x.ChildProperty)
                  .On(VM);
               VM.Refresh(propertySelector);
               VM.ValidatorResults.VerifyInvocationSequence();
            });
      }

      [TestMethod]
      public void RefreshContainer_OfWrapperCollection_DoesNotRefreshItems() {
         VM.Revalidate(ValidationScope.SelfAndAllDescendants);

         var child = new ChildVM(new ChildSource());
         VM.WrapperProperty.Add(child);

         VM.RefreshContainer(x => x.WrapperProperty);

         Assert.IsFalse(child.WasRefreshed);
      }

      [TestMethod]
      public void RefreshContainer_OfWrapperCollection_SynchronizesFromSourceItems() {
         var existing = new ChildVM(new ChildSource());
         var removed = new ChildVM(new ChildSource());
         var addedSource = new ChildSource();

         VM.WrapperProperty.Add(removed);
         VM.WrapperProperty.Add(existing);

         VM.WrapperPropertySource.Remove(removed.Source);
         VM.WrapperPropertySource.Insert(0, addedSource);

         VM.RefreshContainer(x => x.WrapperProperty);

         Assert.AreSame(addedSource, VM.WrapperProperty[0].Source);
         Assert.AreSame(existing, VM.WrapperProperty[1]);
      }

      [TestMethod]
      public void RefreshContainer_OfWrapperCollection_RevalidatesItems() {
         VM.Revalidate(ValidationScope.SelfAndAllDescendants);

         var child = new ChildVM(new ChildSource());
         VM.WrapperProperty.Add(child);
         VM.ValidatorResults.Reset();

         VM.ValidatorResults
            .ExpectInvocationOf.CollectionPropertyValidation
            .Targeting(VM.WrapperProperty, x => x.ChildProperty)
            .On(VM);

         VM.RefreshContainer(x => x.WrapperProperty);

         VM.ValidatorResults.VerifyInvocationSequence();
      }

      private class ChildSourceWithEquals : ChildSource {
         public ChildSourceWithEquals(string id) {
            Id = id;
         }

         public string Id { get; set; }

         public override int GetHashCode() {
            return (Id ?? String.Empty).GetHashCode();
         }

         public override bool Equals(object obj) {
            var other = obj as ChildSourceWithEquals;
            return other != null && Object.Equals(other.Id, Id);
         }
      }

      private sealed class RootVM : TestViewModel<RootVMDescriptor> {
         public static readonly RootVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<RootVMDescriptor>()
            .For<RootVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.InstanceProperty = v.Collection.Of<ChildVM>(ChildVM.ClassDescriptor);

               d.WrapperProperty = v
                  .Collection
                  .Wraps(x => x.WrapperPropertySource)
                  .With<ChildVM>(ChildVM.ClassDescriptor);

               d.PopulatedProperty = v
                  .Collection
                  .PopulatedWith(x => x.PopulatedPropertyResult)
                  .With(ChildVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.CheckCollection(x => x.InstanceProperty, x => x.ChildProperty).Custom<ChildVM>(args =>
                  args.Owner.ValidatorResults.PerformValidation(args)
               );

               b.CheckCollection(x => x.WrapperProperty, x => x.ChildProperty).Custom<ChildVM>(args =>
                  args.Owner.ValidatorResults.PerformValidation(args)
               );

               b.CheckCollection(x => x.PopulatedProperty, x => x.ChildProperty).Custom<ChildVM>(args =>
                  args.Owner.ValidatorResults.PerformValidation(args)
               );
            })
            .Build();

         public RootVM()
            : base(ClassDescriptor) {
            WrapperPropertySource = new List<ChildSource>();
            PopulatedPropertyResult = new List<ChildVM>();
            ValidatorResults = new ValidatorMockConfigurationFluent();
         }

         public IList<ChildSource> WrapperPropertySource { get; set; }
         public IList<ChildVM> PopulatedPropertyResult { get; set; }

         public ValidatorMockConfigurationFluent ValidatorResults { get; private set; }

         public IVMCollection<ChildVM> InstanceProperty {
            get { return GetValue(Descriptor.InstanceProperty); }
         }

         public IVMCollection<ChildVM> WrapperProperty {
            get { return GetValue(Descriptor.WrapperProperty); }
         }

         public IVMCollection<ChildVM> PopulatedProperty {
            get { return GetValue(Descriptor.PopulatedProperty); }
         }
      }

      private sealed class RootVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ChildVM>> InstanceProperty { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ChildVM>> WrapperProperty { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ChildVM>> PopulatedProperty { get; set; }
      }
   }
}