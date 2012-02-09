namespace Inspiring.MvvmTest.ApiTests.ViewModels.Refresh {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class SimplePropertyRefreshTests {
      private RootVM VM { get; set; }

      [TestInitialize]
      public void Setup() {
         VM = new RootVM();
      }

      [TestMethod]
      public void Refresh_OfSimpleProperty_CallsNotifyChange() {
         ParameterizedTest
            .TestCase("InstanceProperty", new Func<RootVMDescriptor, IVMPropertyDescriptor>(x => x.InstanceProperty))
            .TestCase("MappedProperty", x => x.MappedProperty)
            .TestCase("DelegateProperty", x => x.DelegateProperty)
            .Run(propertySelector => {
               VM.OnChangeInvocations.Clear();
               VM.Refresh(propertySelector);

               var expectedChangaArgs = ChangeArgs
                  .PropertyChanged(propertySelector(VM.Descriptor), ValueStage.ValidatedValue)
                  .PrependViewModel(VM);

               DomainAssert.AreEqual(new[] { expectedChangaArgs }, VM.OnChangeInvocations);
            });
      }

      [TestMethod]
      public void Refresh_OfSimpleProperty_RevalidatesPropertyValue() {
         ParameterizedTest
            .TestCase("InstanceProperty", new Func<RootVMDescriptor, IVMPropertyDescriptor>(x => x.InstanceProperty))
            .TestCase("MappedProperty", x => x.MappedProperty)
            .TestCase("DelegateProperty", x => x.DelegateProperty)
            .Run(propertySelector => {
               var property = propertySelector(VM.Descriptor);
               var expectedError = "Validation error";
               VM.ValidationErrors[property] = expectedError;

               VM.Refresh(propertySelector);
               ValidationAssert.ErrorMessages(VM.GetValidationResult(property), expectedError);
            });
      }

      private sealed class RootVM : TestViewModel<RootVMDescriptor> {
         public static readonly RootVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<RootVMDescriptor>()
            .For<RootVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.InstanceProperty = v.Property.Of<string>();
               d.MappedProperty = v.Property.MapsTo(x => x.MappedPropertySource);
               d.DelegateProperty = v.Property.DelegatesTo(
                  x => x.DelegatePropertyResult,
                  (x, val) => x.DelegatePropertyResult = val
               );
            })
            .WithValidators(b => {
               b.Check(x => x.InstanceProperty).Custom(PerformValidation);
               b.Check(x => x.MappedProperty).Custom(PerformValidation);
               b.Check(x => x.DelegateProperty).Custom(PerformValidation);
            })
            .Build();

         public RootVM()
            : base(ClassDescriptor) {
            ValidationErrors = new Dictionary<IVMPropertyDescriptor, string>();
         }

         public string MappedPropertySource { get; set; }
         public string DelegatePropertyResult { get; set; }

         public Dictionary<IVMPropertyDescriptor, string> ValidationErrors { get; private set; }

         private static void PerformValidation(PropertyValidationArgs<RootVM, RootVM, string> args) {
            string errorMessage;
            if (args.Owner.ValidationErrors.TryGetValue(args.TargetProperty, out errorMessage)) {
               args.AddError(errorMessage);
            }
         }
      }

      private sealed class RootVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> InstanceProperty { get; set; }
         public IVMPropertyDescriptor<string> MappedProperty { get; set; }
         public IVMPropertyDescriptor<string> DelegateProperty { get; set; }
      }
   }
}