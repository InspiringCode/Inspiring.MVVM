namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class HierarchicalValidationTests : HierarchicalValidationFixture {
      [TestMethod]
      public void Revalidate_ExecutesAncestorPropertyValidators() {
         Setup.ForOwner(Child).SetupPropertyError(Child, ChildDescriptor.ChildProperty);
         Setup.ForOwner(Parent).SetupPropertyError(Child, ChildDescriptor.ChildProperty);
         Setup.ForOwner(Grandparent).SetupPropertyError(Child, ChildDescriptor.ChildProperty);

         Setup.ForOwner(Child).ExpectPropertyValidatorInvocation(Child, ChildDescriptor.ChildProperty);
         Setup.ForOwner(Parent).ExpectPropertyValidatorInvocation(Child, ChildDescriptor.ChildProperty);
         Setup.ForOwner(Grandparent).ExpectPropertyValidatorInvocation(Child, ChildDescriptor.ChildProperty);

         // The view model validation is triggered by the validation state change
         Setup.ForOwner(Child).ExpectViewModelValidatorInvocation(Child);
         Setup.ForOwner(Parent).ExpectViewModelValidatorInvocation(Child);
         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Child);

         Setup.ForOwner(Parent).ExpectViewModelValidatorInvocation(Parent);
         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Parent);

         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Grandparent);

         Child.Revalidate(ChildDescriptor.ChildProperty);
         Setup.VerifyAllStrict();
      }

      [TestMethod]
      public void Revalidate_ExecutesAncestorViewModelValidators() {
         Setup.ForOwner(Child).SetupViewModelError(Child);
         Setup.ForOwner(Parent).SetupViewModelError(Child);
         Setup.ForOwner(Grandparent).SetupViewModelError(Child);

         Setup.ForOwner(Child).ExpectViewModelValidatorInvocation(Child);
         Setup.ForOwner(Parent).ExpectViewModelValidatorInvocation(Child);
         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Child);

         // The second executions are triggered by the validation state change
         Setup.ForOwner(Child).ExpectViewModelValidatorInvocation(Child);
         Setup.ForOwner(Parent).ExpectViewModelValidatorInvocation(Child);
         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Child);

         Setup.ForOwner(Parent).ExpectViewModelValidatorInvocation(Parent);
         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Parent);

         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Grandparent);


         Child.RevalidateViewModelValidations();
         Setup.VerifyAllStrict();
      }

      [TestMethod]
      public void Revalidate_ExecutesAncestorCollectionPropertyValidators() {
         Setup.ForOwner(Child).SetupCollectionPropertyError(Item, ItemDescriptor.ItemProperty);
         Setup.ForOwner(Parent).SetupCollectionPropertyError(Item, ItemDescriptor.ItemProperty);
         Setup.ForOwner(Grandparent).SetupCollectionPropertyError(Item, ItemDescriptor.ItemProperty);

         Setup.ForOwner(Item).ExpectPropertyValidatorInvocation(Item, ItemDescriptor.ItemProperty);
         Setup.ForOwner(Child).ExpectPropertyValidatorInvocation(Item, ItemDescriptor.ItemProperty);
         Setup.ForOwner(Parent).ExpectPropertyValidatorInvocation(Item, ItemDescriptor.ItemProperty);
         Setup.ForOwner(Grandparent).ExpectPropertyValidatorInvocation(Item, ItemDescriptor.ItemProperty);

         Setup.ForOwner(Child).ExpectCollectionPropertyInvocation(Item, ItemDescriptor.ItemProperty);
         Setup.ForOwner(Parent).ExpectCollectionPropertyInvocation(Item, ItemDescriptor.ItemProperty);
         Setup.ForOwner(Grandparent).ExpectCollectionPropertyInvocation(Item, ItemDescriptor.ItemProperty);

         // The view model validation is triggered by the validation state change
         Setup.ForOwner(Item).ExpectViewModelValidatorInvocation(Item);
         Setup.ForOwner(Child).ExpectViewModelValidatorInvocation(Item);
         Setup.ForOwner(Parent).ExpectViewModelValidatorInvocation(Item);
         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Item);

         Setup.ForOwner(Child).ExpectCollectionViewModelValidatorInvocation(Item);
         Setup.ForOwner(Parent).ExpectCollectionViewModelValidatorInvocation(Item);
         Setup.ForOwner(Grandparent).ExpectCollectionViewModelValidatorInvocation(Item);

         Setup.ForOwner(Child).ExpectViewModelValidatorInvocation(Child);
         Setup.ForOwner(Parent).ExpectViewModelValidatorInvocation(Child);
         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Child);

         Setup.ForOwner(Parent).ExpectViewModelValidatorInvocation(Parent);
         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Parent);

         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Grandparent);

         Item.Revalidate(ItemDescriptor.ItemProperty);
         Setup.VerifyAllStrict();
      }

      [TestMethod]
      public void Revalidate_ExecutesAncestorCollectionViewModelValidators() {
         Setup.ForOwner(Child).SetupCollectionViewModelError(Item);
         Setup.ForOwner(Parent).SetupCollectionViewModelError(Item);
         Setup.ForOwner(Grandparent).SetupCollectionViewModelError(Item);

         Setup.ForOwner(Item).ExpectViewModelValidatorInvocation(Item);
         Setup.ForOwner(Child).ExpectViewModelValidatorInvocation(Item);
         Setup.ForOwner(Parent).ExpectViewModelValidatorInvocation(Item);
         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Item);

         Setup.ForOwner(Child).ExpectCollectionViewModelValidatorInvocation(Item);
         Setup.ForOwner(Parent).ExpectCollectionViewModelValidatorInvocation(Item);
         Setup.ForOwner(Grandparent).ExpectCollectionViewModelValidatorInvocation(Item);

         // The second executions are triggered by the validation state change
         Setup.ForOwner(Item).ExpectViewModelValidatorInvocation(Item);
         Setup.ForOwner(Child).ExpectViewModelValidatorInvocation(Item);
         Setup.ForOwner(Parent).ExpectViewModelValidatorInvocation(Item);
         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Item);

         Setup.ForOwner(Child).ExpectCollectionViewModelValidatorInvocation(Item);
         Setup.ForOwner(Parent).ExpectCollectionViewModelValidatorInvocation(Item);
         Setup.ForOwner(Grandparent).ExpectCollectionViewModelValidatorInvocation(Item);

         Setup.ForOwner(Child).ExpectViewModelValidatorInvocation(Child);
         Setup.ForOwner(Parent).ExpectViewModelValidatorInvocation(Child);
         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Child);

         Setup.ForOwner(Parent).ExpectViewModelValidatorInvocation(Parent);
         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Parent);

         Setup.ForOwner(Grandparent).ExpectViewModelValidatorInvocation(Grandparent);

         Item.RevalidateViewModelValidations();
         Setup.VerifyAllStrict();
      }
   }
}