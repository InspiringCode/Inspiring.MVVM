namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidatorInteractionTests : TestBase {
      private const bool OK = true;
      private const bool NOK = false;
      private const bool Occured = true;
      private const bool HasNotOccured = false;

      private EmployeeListVM VM { get; set; }

      private EmployeeVM TargetItem { get; set; }
      private EmployeeVM OtherItem { get; set; }

      private List<EmployeeVM> InvalidItemsOfCollectionValidator { get; set; }
      private List<EmployeeVM> InvalidItemsOfSecondCollectionValidator { get; set; }
      private List<EmployeeVM> InvalidItemsOfPropertyValidator { get; set; }
      private List<EmployeeVM> InvalidItemsOfSecondPropertyValidator { get; set; }

      [TestInitialize]
      public void Setup() {
         InvalidItemsOfCollectionValidator = new List<EmployeeVM>();
         InvalidItemsOfSecondCollectionValidator = new List<EmployeeVM>();
         InvalidItemsOfPropertyValidator = new List<EmployeeVM>();
         InvalidItemsOfSecondPropertyValidator = new List<EmployeeVM>();

         VM = new EmployeeListVM(this);
         TargetItem = new EmployeeVM(this, "Target employee");
         OtherItem = new EmployeeVM(this, "Other employee");

         VM.GetValue(x => x.Employees).Add(TargetItem);
         VM.GetValue(x => x.Employees).Add(OtherItem);
      }

      [TestMethod]
      public void CollectionValidatorResultOfTargetItemChanges_OtherValidatorsReturnOK() {
         AfterCollectionValidatorResultOf(TargetItem, changedFrom: OK, to: OK, itemIs: OK, andValidationStateEvent: HasNotOccured);
         AfterCollectionValidatorResultOf(TargetItem, changedFrom: NOK, to: NOK, itemIs: NOK, andValidationStateEvent: HasNotOccured);
         AfterCollectionValidatorResultOf(TargetItem, changedFrom: OK, to: NOK, itemIs: NOK, andValidationStateEvent: Occured);
         AfterCollectionValidatorResultOf(TargetItem, changedFrom: NOK, to: OK, itemIs: OK, andValidationStateEvent: Occured);
      }

      [TestMethod]
      public void CollectionValidatorResultOfOtherItemChanges_OtherValidatorsReturnOK() {
         AfterCollectionValidatorResultOf(OtherItem, changedFrom: OK, to: OK, itemIs: OK, andValidationStateEvent: HasNotOccured);
         AfterCollectionValidatorResultOf(OtherItem, changedFrom: NOK, to: NOK, itemIs: NOK, andValidationStateEvent: HasNotOccured);
         AfterCollectionValidatorResultOf(OtherItem, changedFrom: OK, to: NOK, itemIs: NOK, andValidationStateEvent: Occured);
         AfterCollectionValidatorResultOf(OtherItem, changedFrom: NOK, to: OK, itemIs: OK, andValidationStateEvent: Occured);
      }


      [TestMethod]
      public void CollectionValidatorResultOfTargetItemChanges_PropertyValidatorReturnsNOK() {
         InvalidItemsOfPropertyValidator.Add(TargetItem);
         TargetItem.Revalidate();

         AfterCollectionValidatorResultOf(TargetItem, changedFrom: OK, to: OK, itemIs: NOK, andValidationStateEvent: HasNotOccured);
         AfterCollectionValidatorResultOf(TargetItem, changedFrom: NOK, to: NOK, itemIs: NOK, andValidationStateEvent: HasNotOccured);
         AfterCollectionValidatorResultOf(TargetItem, changedFrom: OK, to: NOK, itemIs: NOK, andValidationStateEvent: Occured);
         AfterCollectionValidatorResultOf(TargetItem, changedFrom: NOK, to: OK, itemIs: NOK, andValidationStateEvent: Occured);
      }

      [TestMethod]
      public void CollectionValidatorResultOfOtherItemChanges_PropertyValidatorReturnsNOK() {
         InvalidItemsOfPropertyValidator.Add(OtherItem);
         OtherItem.Revalidate();

         AfterCollectionValidatorResultOf(OtherItem, changedFrom: OK, to: OK, itemIs: NOK, andValidationStateEvent: HasNotOccured);
         AfterCollectionValidatorResultOf(OtherItem, changedFrom: NOK, to: NOK, itemIs: NOK, andValidationStateEvent: HasNotOccured);
         AfterCollectionValidatorResultOf(OtherItem, changedFrom: OK, to: NOK, itemIs: NOK, andValidationStateEvent: Occured);
         AfterCollectionValidatorResultOf(OtherItem, changedFrom: NOK, to: OK, itemIs: NOK, andValidationStateEvent: Occured);
      }

      [TestMethod]
      public void CollectionValidatorResultOfTargetItemChanges_SecondCollectionValidatorReturnsNOK() {
         InvalidItemsOfSecondCollectionValidator.Add(TargetItem);
         TargetItem.Revalidate();

         AfterCollectionValidatorResultOf(TargetItem, changedFrom: OK, to: OK, itemIs: NOK, andValidationStateEvent: HasNotOccured);
         AfterCollectionValidatorResultOf(TargetItem, changedFrom: NOK, to: NOK, itemIs: NOK, andValidationStateEvent: HasNotOccured);
         AfterCollectionValidatorResultOf(TargetItem, changedFrom: OK, to: NOK, itemIs: NOK, andValidationStateEvent: Occured);
         AfterCollectionValidatorResultOf(TargetItem, changedFrom: NOK, to: OK, itemIs: NOK, andValidationStateEvent: Occured);
      }

      [TestMethod]
      public void CollectionValidatorResultOfOtherItemChanges_SecondCollectionValidatorReturnsNOK() {
         InvalidItemsOfSecondCollectionValidator.Add(OtherItem);
         OtherItem.Revalidate();

         AfterCollectionValidatorResultOf(OtherItem, changedFrom: OK, to: OK, itemIs: NOK, andValidationStateEvent: HasNotOccured);
         AfterCollectionValidatorResultOf(OtherItem, changedFrom: NOK, to: NOK, itemIs: NOK, andValidationStateEvent: HasNotOccured);
         AfterCollectionValidatorResultOf(OtherItem, changedFrom: OK, to: NOK, itemIs: NOK, andValidationStateEvent: Occured);
         AfterCollectionValidatorResultOf(OtherItem, changedFrom: NOK, to: OK, itemIs: NOK, andValidationStateEvent: Occured);
      }

      [TestMethod]
      public void PropertyValidatorResultChanges_OtherValidatorsReturnsOK() {
         AfterPropertyValidatorResultOf(TargetItem, changedFrom: OK, to: OK, itemIs: OK, andValidationStateEvent: HasNotOccured);
         AfterPropertyValidatorResultOf(TargetItem, changedFrom: NOK, to: NOK, itemIs: NOK, andValidationStateEvent: HasNotOccured);
         AfterPropertyValidatorResultOf(TargetItem, changedFrom: OK, to: NOK, itemIs: NOK, andValidationStateEvent: Occured);
         AfterPropertyValidatorResultOf(TargetItem, changedFrom: NOK, to: OK, itemIs: OK, andValidationStateEvent: Occured);
      }

      [TestMethod]
      public void PropertyValidatorResultChanges_CollectionValidatorReturnsNOK() {
         InvalidItemsOfCollectionValidator.Add(TargetItem);
         TargetItem.Revalidate();

         AfterPropertyValidatorResultOf(TargetItem, changedFrom: OK, to: OK, itemIs: NOK, andValidationStateEvent: HasNotOccured);
         AfterPropertyValidatorResultOf(TargetItem, changedFrom: NOK, to: NOK, itemIs: NOK, andValidationStateEvent: HasNotOccured);
         AfterPropertyValidatorResultOf(TargetItem, changedFrom: OK, to: NOK, itemIs: NOK, andValidationStateEvent: Occured);
         AfterPropertyValidatorResultOf(TargetItem, changedFrom: NOK, to: OK, itemIs: NOK, andValidationStateEvent: Occured);
      }

      [TestMethod]
      public void PropertyValidatorResultChanges_SecondPropertyValidatorReturnsNOK() {
         InvalidItemsOfSecondPropertyValidator.Add(TargetItem);
         TargetItem.Revalidate();

         AfterPropertyValidatorResultOf(TargetItem, changedFrom: OK, to: OK, itemIs: NOK, andValidationStateEvent: HasNotOccured);
         AfterPropertyValidatorResultOf(TargetItem, changedFrom: NOK, to: NOK, itemIs: NOK, andValidationStateEvent: HasNotOccured);
         AfterPropertyValidatorResultOf(TargetItem, changedFrom: OK, to: NOK, itemIs: NOK, andValidationStateEvent: Occured);
         AfterPropertyValidatorResultOf(TargetItem, changedFrom: NOK, to: OK, itemIs: NOK, andValidationStateEvent: Occured);
      }


      //
      // HELPERS
      //

      private void AfterCollectionValidatorResultOf(EmployeeVM item, bool changedFrom, bool to, bool itemIs, bool andValidationStateEvent) {
         bool initiallyValidatorResultOK = (changedFrom == OK);
         bool finalValidatorResultOK = (to == OK);

         bool expectItemIsValid = (itemIs == OK);
         bool expectValidationStateChangedEvent = (andValidationStateEvent == Occured);

         ChangeValidatorResult(InvalidItemsOfCollectionValidator, item, initiallyValidatorResultOK);
         TargetItem.Revalidate();

         item.ValidationStateChangedEvents.Clear();
         ChangeValidatorResult(InvalidItemsOfCollectionValidator, item, finalValidatorResultOK);
         TargetItem.Revalidate();

         AssertValidationState(item, expectItemIsValid);
         AssertValidationStateChangedEvent(item, expectValidationStateChangedEvent);
      }

      private void AfterPropertyValidatorResultOf(EmployeeVM item, bool changedFrom, bool to, bool itemIs, bool andValidationStateEvent) {
         bool initiallyValidatorResultOK = (changedFrom == OK);
         bool finalValidatorResultOK = (to == OK);

         bool expectItemIsValid = (itemIs == OK);
         bool expectValidationStateChangedEvent = (andValidationStateEvent == Occured);

         ChangeValidatorResult(InvalidItemsOfPropertyValidator, item, initiallyValidatorResultOK);
         TargetItem.Revalidate();

         item.ValidationStateChangedEvents.Clear();
         ChangeValidatorResult(InvalidItemsOfPropertyValidator, item, finalValidatorResultOK);
         TargetItem.Revalidate();

         AssertValidationState(item, expectItemIsValid);
         AssertValidationStateChangedEvent(item, expectValidationStateChangedEvent);
      }


      private void ChangeValidatorResult(IList<EmployeeVM> invalidItems, EmployeeVM item, bool isValid) {
         invalidItems.Remove(item);
         if (!isValid) {
            invalidItems.Add(item);
         }
      }

      private void AssertValidationState(EmployeeVM item, bool isValid) {
         if (isValid) {
            Assert.IsTrue(item.IsValid, "Expected item to be valid.");
         } else {
            Assert.IsFalse(item.IsValid, "Expected item to be invalid.");
         }
      }

      private void AssertValidationStateChangedEvent(EmployeeVM item, bool assertEvent) {
         var eventCount = item.ValidationStateChangedEvents.Count;
         var eventProperty = item.ValidationStateChangedEvents.FirstOrDefault();

         if (assertEvent) {
            Assert.AreEqual(1, eventCount, "Expected exactly one validation state changed event.");
            Assert.AreEqual(EmployeeVM.ClassDescriptor.Name, eventProperty);
         } else {
            Assert.AreEqual(0, eventCount, "Expected no validation state changed event.");
         }
      }


      //
      // VIEW MODELS
      //

      private sealed class EmployeeListVM : ViewModel<EmployeeListVMDescriptor> {
         public static readonly EmployeeListVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeListVMDescriptor>()
            .For<EmployeeListVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Employees = v.Collection.Of<EmployeeVM>(EmployeeVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.CheckCollection(x => x.Employees, x => x.Name).Custom<EmployeeVM>(FirstCollectionValidator);
               b.CheckCollection(x => x.Employees, x => x.Name).Custom<EmployeeVM>(SecondCollectionValidator);
            })
            .Build();

         public EmployeeListVM(ValidatorInteractionTests test)
            : base(ClassDescriptor) {
            Test = test;
         }

         private ValidatorInteractionTests Test { get; set; }

         private static void FirstCollectionValidator(
            CollectionValidationArgs<EmployeeListVM, EmployeeVM, string> args
         ) {
            var owner = args.Owner;
            var invalidItems = owner.Test.InvalidItemsOfCollectionValidator.ToList();

            foreach (var item in invalidItems) {
               args.AddError(item, "Collection validator error");
            }
         }

         private static void SecondCollectionValidator(
            CollectionValidationArgs<EmployeeListVM, EmployeeVM, string> args
         ) {
            var owner = args.Owner;
            var invalidItems = owner.Test.InvalidItemsOfSecondCollectionValidator.ToList();

            foreach (var item in invalidItems) {
               args.AddError(item, "Second collection validator error");
            }
         }
      }


      private sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> {
         public static readonly EmployeeVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Name = v.Property.Of<string>();
               d.Salary = v.Property.Of<decimal>();
            })
            .WithValidators(b => {
               b.Check(x => x.Name).Custom(FirstPropertyValidator);
               b.Check(x => x.Name).Custom(SecondPropertyValidator);
            })
            .Build();

         public EmployeeVM(ValidatorInteractionTests test, string name)
            : base(ClassDescriptor) {

            ValidationStateChangedEvents = new List<IVMPropertyDescriptor>();

            Test = test;
            SetValue(Descriptor.Name, name);
         }

         public List<IVMPropertyDescriptor> ValidationStateChangedEvents { get; private set; }

         private ValidatorInteractionTests Test { get; set; }

         public void Revalidate() {
            base.Revalidate();
         }

         public override string ToString() {
            return "{" + GetValue(Descriptor.Name) + "}";
         }

         protected override void OnValidationStateChanged(IVMPropertyDescriptor property) {
            base.OnValidationStateChanged(property);
            ValidationStateChangedEvents.Add(property);
         }

         private static void FirstPropertyValidator(PropertyValidationArgs<EmployeeVM, EmployeeVM, string> args) {
            if (args.Owner.Test.InvalidItemsOfPropertyValidator.Contains(args.Target)) {
               args.AddError("Property validator error");
            }
         }

         private static void SecondPropertyValidator(PropertyValidationArgs<EmployeeVM, EmployeeVM, string> args) {
            if (args.Owner.Test.InvalidItemsOfSecondPropertyValidator.Contains(args.Target)) {
               args.AddError("Second property validator error");
            }
         }
      }

      private sealed class EmployeeListVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<EmployeeVM>> Employees { get; set; }
      }

      private sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<decimal> Salary { get; set; }
      }
   }
}