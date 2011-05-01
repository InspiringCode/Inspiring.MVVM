﻿namespace Inspiring.MvvmTest.ApiTests.ViewModels.Refresh {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class CollectionPropertyRefreshTests : RefreshFixture {
      private RootVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new RootVM();
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
               .ItemsAdded(VM.WrapperProperty, newItems)
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
               .ItemsAdded(VM.PopulatedProperty, newItems)
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
            .Build();

         public RootVM()
            : base(ClassDescriptor) {
            WrapperPropertySource = new List<ChildSource>();
            PopulatedPropertyResult = new List<ChildVM>();
         }

         public IList<ChildSource> WrapperPropertySource { get; set; }
         public IList<ChildVM> PopulatedPropertyResult { get; set; }

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