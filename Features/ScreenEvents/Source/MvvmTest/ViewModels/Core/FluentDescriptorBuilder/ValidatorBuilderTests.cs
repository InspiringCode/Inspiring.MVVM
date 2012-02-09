namespace Inspiring.MvvmTest.ViewModels.Core.FluentDescriptorBuilder {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidatorBuilderTests {
      [TestMethod]
      public void PropertyValidator_AddsValidators() {
         Action<PropertyValidationArgs<EmployeeVM, EmployeeVM, string>> validationAction = (args) => { };

         var d = BuildDescriptor(b => b
            .Check(x => x.Name)
            .Custom(validationAction)
         );

         AssertStandardValidators(
            d,
            ValidationStep.Value,
            PathDefinition
               .Empty
               .Append((EmployeeVMDescriptor x) => x.Name),
            DelegateValidator.For(validationAction)
         );
      }

      [TestMethod]
      public void PropertyValidator_ForDescendantViewModel_AddsValidators() {
         Action<PropertyValidationArgs<EmployeeVM, IViewModel<ProjectVMDescriptor>, DateTime>> validationAction = (args) => { };

         var d = BuildDescriptor(b => b
            .ValidateDescendant(x => x.SelectedProject)
            .Check(x => x.EndDate)
            .Custom(validationAction)
         );

         AssertStandardValidators(
            d,
            ValidationStep.Value,
            PathDefinition
               .Empty
               .Append(new Func<EmployeeVMDescriptor, IVMPropertyDescriptor<ProjectVM>>(x => x.SelectedProject))
               .Append((ProjectVMDescriptor x) => x.EndDate),
            DelegateValidator.For(validationAction)
         );
      }

      [TestMethod]
      public void PropertyValidator_ForDescendantCollection_AddsValidators() {
         Action<PropertyValidationArgs<EmployeeVM, IViewModel<ProjectVMDescriptor>, DateTime>> validationAction = (args) => { };

         var d = BuildDescriptor(b => b
            .ValidateDescendant(x => x.Projects)
            .Check(x => x.EndDate)
            .Custom(validationAction)
         );

         var collectionSelector = new Func<EmployeeVMDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<ProjectVMDescriptor>>>>(x => x.Projects);

         AssertStandardValidators(
            d,
            ValidationStep.Value,
            PathDefinition
               .Empty
               .Append(collectionSelector)
               .Append((ProjectVMDescriptor x) => x.EndDate),
            DelegateValidator.For(validationAction)
         );
      }

      [TestMethod]
      public void CollectionViewModelValidator_AddsValidators() {
         Action<CollectionValidationArgs<EmployeeVM, ProjectVM>> validationAction = (args) => { };

         var d = BuildDescriptor(b => b
            .CheckCollection(x => x.Projects)
            .Custom(validationAction)
         );

         AssertStandardValidators(
            d,
            ValidationStep.ViewModel,
            PathDefinition
               .Empty
               .AppendCollection(new Func<EmployeeVMDescriptor, IVMPropertyDescriptor<IVMCollection<ProjectVM>>>(x => x.Projects)),
            DelegateValidator.For(validationAction)
         );
      }

      [TestMethod]
      public void CollectionPropertyValidators_AddsValidators() {
         Action<CollectionValidationArgs<EmployeeVM, IViewModel<ProjectVMDescriptor>, DateTime>> validationAction = (args) => { };

         var d = BuildDescriptor(b => b
            .CheckCollection(x => x.Projects, x => x.EndDate)
            .Custom(validationAction)
         );

         var collectionSelector = new Func<EmployeeVMDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<ProjectVMDescriptor>>>>(x => x.Projects);

         AssertStandardValidators(
            d,
            ValidationStep.Value,
            PathDefinition
               .Empty
               .AppendCollection(collectionSelector)
               .AppendCollectionProperty((ProjectVMDescriptor x) => x.EndDate),
            DelegateValidator.For(validationAction)
         );
      }

      [TestMethod]
      public void ViewModelValidator_AddsValidators() {
         Action<ViewModelValidationArgs<EmployeeVM, EmployeeVM>> validationAction = (args) => { };

         var d = BuildDescriptor(b => b
            .CheckViewModel(validationAction)
         );

         AssertStandardValidators(
            d,
            ValidationStep.ViewModel,
            PathDefinition.Empty,
            DelegateValidator.For(validationAction)
         );
      }

      [TestMethod]
      public void ViewModelValidator_ForDescendantViewModel_AddsValidators() {
         Action<ViewModelValidationArgs<EmployeeVM, IViewModel<ProjectVMDescriptor>>> validationAction = (args) => { };

         var d = BuildDescriptor(b => b
            .ValidateDescendant(x => x.SelectedProject)
            .CheckViewModel(validationAction)
         );

         AssertStandardValidators(
            d,
            ValidationStep.ViewModel,
            PathDefinition
               .Empty
               .Append(new Func<EmployeeVMDescriptor, IVMPropertyDescriptor<ProjectVM>>(x => x.SelectedProject)),
            DelegateValidator.For(validationAction)
         );
      }

      [TestMethod]
      public void PropertyValidator_EnablesValueValidationSourceBehavior() {
         var d = BuildDescriptor(b => b
            .Check(x => x.Name)
            .Custom(_ => { })
         );

         Assert.IsTrue(IsEnabled<ValueValidationSourceBehavior<string>>(d, x => x.Name));
      }

      [TestMethod]
      public void PropertyValidator_ForDescendant_DoesNotEnableValueValidationSourceBehavior() {
         var d = BuildDescriptor(b => b
            .ValidateDescendant(x => x.SelectedProject)
            .Check(x => x.EndDate)
            .Custom(_ => { })
         );

         Assert.IsFalse(IsEnabled<ValueValidationSourceBehavior<string>>(d, x => x.Name));
      }

      [TestMethod]
      public void ViewModelValidator_EnablesViewModelValidationSourceBehavior() {
         var d = BuildDescriptor(b => b
            .CheckViewModel(_ => { })
         );

         Assert.IsTrue(IsEnabled<ViewModelValidationSourceBehavior>(d));
      }

      [TestMethod]
      public void ViewModelValidator_ForDescendant_DoesNotEnableViewModelValidationSourceBehavior() {
         var d = BuildDescriptor(b => b
            .ValidateDescendant(x => x.Projects)
            .CheckViewModel(_ => { })
         );

         Assert.IsFalse(IsEnabled<ViewModelValidationSourceBehavior>(d));
      }

      [TestMethod]
      public void EnableParentValidation_EnablesValueValidationSourceBehavior() {
         var d = BuildDescriptor(b => b
            .EnableParentValidation(x => x.Name)
         );

         Assert.IsTrue(IsEnabled<ValueValidationSourceBehavior<string>>(d, x => x.Name));
      }

      private bool IsEnabled<TPropertyBehavior>(
         EmployeeVMDescriptor d,
         Func<EmployeeVMDescriptor, IVMPropertyDescriptor> propertySelector
      ) {
         var property = propertySelector(d);
         TPropertyBehavior b;
         return property.Behaviors.TryGetBehavior(out b);
      }

      private bool IsEnabled<TViewModelBehavior>(
         EmployeeVMDescriptor d
      ) {
         TViewModelBehavior b;
         return d.Behaviors.TryGetBehavior(out b);
      }

      //private static Func<TDescriptor, IVMPropertyDescriptor<IVMCollection<IViewModelExpression<TItemDescriptor>>>>
      //   CollectionSelector<TDescriptor, TItemDescriptor>()
      //   where TDescriptor : VMDescriptorBase
      //   where TItemDescriptor : VMDescriptorBase {

      //   return new Func<TDescriptor, IVMPropertyDescriptor<IVMCollection<IViewModelExpression<TItemDescriptor>>>>(x => null);
      //}

      //private static Func<TDescriptor, IVMPropertyDescriptor<IViewModel<TChildDescriptor>>>
      //   ViewModelSelector<TDescriptor, TChildDescriptor>()
      //   where TDescriptor : IVMDescriptor
      //   where TChildDescriptor : IVMDescriptor {

      //   return new Func<TDescriptor, IVMPropertyDescriptor<IViewModel<TChildDescriptor>>>(x => null);
      //}

      private void AssertStandardValidators(
         IVMDescriptor descriptor,
         ValidationStep step,
         PathDefinition targetPath,
         IValidator validator
      ) {
         var expected = new ValidatorExecutorBehavior();

         expected.AddValidator(
            new ConditionalValidator(
               new ValidationTargetCondition(targetPath),
               new ConditionalValidator(
                  new ValidationStepCondition(step),
                  validator
               )
            )
         );

         Assert.AreEqual(
            expected.ToString(),
            descriptor
               .Behaviors
               .GetNextBehavior<ValidatorExecutorBehavior>()
               .ToString()
         );
      }

      private EmployeeVMDescriptor BuildDescriptor(
         Action<RootValidatorBuilder<EmployeeVM, EmployeeVM, EmployeeVMDescriptor>> configurationAction
      ) {
         return VMDescriptorBuilder
            .OfType<EmployeeVMDescriptor>()
            .For<EmployeeVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.Name = v.Property.Of<string>();
               d.SelectedProject = v.VM.Of<ProjectVM>();
               d.Projects = v.Collection.Of<ProjectVM>(new ProjectVMDescriptor());
            })
            .WithValidators(configurationAction)
            .Build();
      }


      private sealed class EmployeeVM : ViewModel<EmployeeVMDescriptor> { }

      private sealed class EmployeeVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> Name { get; set; }
         public IVMPropertyDescriptor<ProjectVM> SelectedProject { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ProjectVM>> Projects { get; set; }
      }

      private sealed class ProjectVM : ViewModel<ProjectVMDescriptor> { }

      private sealed class ProjectVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<DateTime> EndDate { get; set; }
         public IVMPropertyDescriptor<CustomerVM> Customer { get; set; }
      }

      private sealed class CustomerVM : ViewModel<CustomerVMDescriptor> { }

      private sealed class CustomerVMDescriptor : VMDescriptor {
         public double Rating { get; set; }
      }
   }
}