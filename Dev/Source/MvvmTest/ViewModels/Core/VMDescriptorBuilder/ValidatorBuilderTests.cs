namespace Inspiring.MvvmTest.ViewModels.Core {
   using System;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Inspiring.Mvvm.ViewModels.Core.Validation.Validators;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ValidatorBuilderTests {
      [TestMethod]
      public void PropertyValidator_AddsValidators() {
         Action<PropertyValidationArgs<EmployeeVM, EmployeeVM, string>> validationAction = (args) => { };

         var d = BuildDescriptor(b => b.Check(x => x.Name).Custom(validationAction));

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
               .Append(ViewModelSelector<EmployeeVMDescriptor, ProjectVMDescriptor>())
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

         AssertStandardValidators(
            d,
            ValidationStep.Value,
            PathDefinition
               .Empty
               .Append(CollectionSelector<EmployeeVMDescriptor, ProjectVMDescriptor>())
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
               .Append(new Func<EmployeeVMDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<ProjectVM>>>(x => x.Projects)),
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

         AssertStandardValidators(
            d,
            ValidationStep.Value,
            PathDefinition
               .Empty
               .Append(CollectionSelector<EmployeeVMDescriptor, ProjectVMDescriptor>())
               .Append((ProjectVMDescriptor x) => x.EndDate),
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
               .Append(ViewModelSelector<EmployeeVMDescriptor, ProjectVMDescriptor>()),
            DelegateValidator.For(validationAction)
         );
      }

      private static Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<TItemDescriptor>>>>
         CollectionSelector<TDescriptor, TItemDescriptor>()
         where TDescriptor : VMDescriptorBase
         where TItemDescriptor : VMDescriptorBase {

         return new Func<TDescriptor, IVMPropertyDescriptor<IVMCollectionExpression<IViewModelExpression<TItemDescriptor>>>>(x => null);
      }

      private static Func<TDescriptor, IVMPropertyDescriptor<IViewModel<TChildDescriptor>>>
         ViewModelSelector<TDescriptor, TChildDescriptor>()
         where TDescriptor : VMDescriptorBase
         where TChildDescriptor : VMDescriptorBase {

         return new Func<TDescriptor, IVMPropertyDescriptor<IViewModel<TChildDescriptor>>>(x => null);
      }

      private void AssertStandardValidators(
         VMDescriptorBase descriptor,
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
            .WithNewValidators(configurationAction)
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