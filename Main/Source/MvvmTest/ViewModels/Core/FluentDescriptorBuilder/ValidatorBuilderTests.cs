namespace Inspiring.MvvmTest.ViewModels.Core.FluentDescriptorBuilder {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidatorBuilderTests {
      private static readonly Func<
            EmployeeVMDescriptor,
            IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<ProjectVMDescriptor>>>>
         ProjectsCollectionSelector = x => x.Projects;

      private static readonly Action<
            PropertyValidationArgs<EmployeeVM, IViewModel<ProjectVMDescriptor>, DateTime>>
         ProjectPropertyValidatorAction = x => { };

      private static readonly Action<
            ViewModelValidationArgs<EmployeeVM, IViewModel<ProjectVMDescriptor>>>
         ProjectViewModelValidatorAction = x => { };

      private static readonly Action<
            CollectionValidationArgs<EmployeeVM, IViewModel<ProjectVMDescriptor>, DateTime>>
         ProjectCollectionPropertyValidatorAction = x => { };


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
         var d = BuildDescriptor(b => b
            .ValidateDescendant(x => x.SelectedProject)
            .Check(x => x.EndDate)
            .Custom(ProjectPropertyValidatorAction)
         );

         AssertStandardValidators(
            d,
            ValidationStep.Value,
            PathDefinition
               .Empty
               .Append(new Func<EmployeeVMDescriptor, IVMPropertyDescriptor<ProjectVM>>(x => x.SelectedProject))
               .Append((ProjectVMDescriptor x) => x.EndDate),
            DelegateValidator.For(ProjectPropertyValidatorAction)
         );
      }

      [TestMethod]
      public void PropertyValidator_ForDescendantCollection_AddsValidators() {
         var d = BuildDescriptor(b => b
            .ValidateDescendant(x => x.Projects)
            .Check(x => x.EndDate)
            .Custom(ProjectPropertyValidatorAction)
         );

         AssertStandardValidators(
            d,
            ValidationStep.Value,
            PathDefinition
               .Empty
               .Append(ProjectsCollectionSelector)
               .Append((ProjectVMDescriptor x) => x.EndDate),
            DelegateValidator.For(ProjectPropertyValidatorAction)
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
         var d = BuildDescriptor(b => b
            .CheckCollection(x => x.Projects, x => x.EndDate)
            .Custom(ProjectCollectionPropertyValidatorAction)
         );

         AssertStandardValidators(
            d,
            ValidationStep.Value,
            PathDefinition
               .Empty
               .AppendCollection(ProjectsCollectionSelector)
               .AppendCollectionProperty((ProjectVMDescriptor x) => x.EndDate),
            DelegateValidator.For(ProjectCollectionPropertyValidatorAction)
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
         var d = BuildDescriptor(b => b
            .ValidateDescendant(x => x.SelectedProject)
            .CheckViewModel(ProjectViewModelValidatorAction)
         );

         AssertStandardValidators(
            d,
            ValidationStep.ViewModel,
            PathDefinition
               .Empty
               .Append(new Func<EmployeeVMDescriptor, IVMPropertyDescriptor<ProjectVM>>(x => x.SelectedProject)),
            DelegateValidator.For(ProjectViewModelValidatorAction)
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

      [TestMethod]
      public void Condition_ForPropertyValidatorOfDescendantCollection_AddsConditionalValidators() {
         var d = BuildDescriptor(b => b
            .ValidateDescendant(x => x.Projects)
            .When(ValidatorPredicate1)
            .Check(x => x.EndDate)
            .Custom(ProjectPropertyValidatorAction)
         );

         AssertStandardValidators(
            d,
            ValidationStep.Value,
            PathDefinition
               .Empty
               .Append(ProjectsCollectionSelector)
               .Append((ProjectVMDescriptor x) => x.EndDate),
            DelegateValidator.For(ProjectPropertyValidatorAction),
            ValidatorPredicate1
         );
      }

      [TestMethod]
      public void Conditions_ForViewModelValidatorOfDescendantViewModel_AddsConditionalValidators() {
         var d = BuildDescriptor(b => b
            .ValidateDescendant(x => x.SelectedProject)
            .When(ValidatorPredicate1)
            .When(ValidatorPredicate2)
            .CheckViewModel(ProjectViewModelValidatorAction)
         );

         AssertStandardValidators(
            d,
            ValidationStep.ViewModel,
            PathDefinition
               .Empty
               .Append(new Func<EmployeeVMDescriptor, IVMPropertyDescriptor<ProjectVM>>(x => x.SelectedProject)),
            DelegateValidator.For(ProjectViewModelValidatorAction),
            ValidatorPredicate1,
            ValidatorPredicate2
         );
      }

      private static bool ValidatorPredicate1(ValidatorConditionArgs<EmployeeVM, IViewModel<ProjectVMDescriptor>> args) {
         throw new NotSupportedException();
      }

      private static bool ValidatorPredicate2(ValidatorConditionArgs<EmployeeVM, IViewModel<ProjectVMDescriptor>> args) {
         throw new NotSupportedException();
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

      private void AssertStandardValidators(
         IVMDescriptor descriptor,
         ValidationStep step,
         PathDefinition targetPath,
         IValidator validator,
         Func<ValidatorConditionArgs<EmployeeVM, IViewModel<ProjectVMDescriptor>>, bool> condition1 = null,
         Func<ValidatorConditionArgs<EmployeeVM, IViewModel<ProjectVMDescriptor>>, bool> condition2 = null
      ) {
         IValidator expectedValidator = validator;

         if (condition2 != null) {
            expectedValidator = new ConditionalValidator(
               new DelegateValidatorCondition<EmployeeVM, IViewModel<ProjectVMDescriptor>>(condition2, 0),
               expectedValidator
            );
         }

         if (condition1 != null) {
            expectedValidator = new ConditionalValidator(
               new DelegateValidatorCondition<EmployeeVM, IViewModel<ProjectVMDescriptor>>(condition1, 0),
               expectedValidator
            );
         }

         expectedValidator = new ConditionalValidator(
            new ValidationStepCondition(step),
            new ConditionalValidator(
               new ValidationTargetCondition(targetPath),
               expectedValidator
            )
         );

         ValidatorExecutorBehavior expectedBehavior = new ValidatorExecutorBehavior();
         expectedBehavior.AddValidator(expectedValidator);

         Assert.AreEqual(
            expectedBehavior.ToString(),
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