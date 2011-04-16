namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class ViewModelStub : ViewModel<VMDescriptorBase> {
      private string _description;

      /// <summary>
      ///   Use the <see cref="ViewModelStubBuilder"/> to create configured 
      ///   instances.
      /// </summary>
      public ViewModelStub(string description = null)
         : this(new DescriptorStub(), description) {
      }

      /// <summary>
      ///   Use the <see cref="ViewModelStubBuilder"/> to create configured 
      ///   instances.
      /// </summary>
      public ViewModelStub(VMDescriptorBase descriptor, string description = null)
         : base(descriptor) {

         NotifyChangeInvocations = new List<ChangeArgs>();
         _description = description;
      }

      public new VMKernel Kernel {
         get { return base.Kernel; }
      }

      public new VMDescriptorBase Descriptor {
         get { return base.Descriptor; }
      }

      public Dictionary<ValidationResultScope, ValidationResult> ReturnedValidationResults {
         get {
            ValidationResultAggregatorStub b;

            if (!Descriptor.Behaviors.TryGetBehavior(out b)) {
               throw new InvalidOperationException(
                  "Setting up validaton result only works if the configured descriptor " +
                  "contains an ValidationResultAggregatorStub behavior."
               );
            }

            return b.ReturnedValidationResults;
         }
      }

      public List<ChangeArgs> NotifyChangeInvocations {
         get;
         private set;
      }

      public static ViewModelStubBuilder Named(string description) {
         return new ViewModelStubBuilder().Named(description);
      }

      public static ViewModelStubBuilder WithProperties(params IVMPropertyDescriptor[] properties) {
         return new ViewModelStubBuilder().WithProperties(properties);
      }

      public static ViewModelStubBuilder WithBehaviors(params IBehavior[] behaviors) {
         return new ViewModelStubBuilder().WithBehaviors(behaviors);
      }

      public static ViewModelStub Build() {
         return new ViewModelStubBuilder().Build();
      }

      public static BehaviorContextStub BuildContext() {
         return new ViewModelStubBuilder().BuildContext();
      }

      public override string ToString() {
         return _description ?? base.ToString();
      }

      protected override void OnChange(ChangeArgs args) {
         NotifyChangeInvocations.Add(args);
      }
   }

   public class ViewModelStubBuilder {
      private DescriptorStubBuilder _descriptorBuilder = new DescriptorStubBuilder();
      private string _description;

      public ViewModelStubBuilder Named(string description) {
         _description = description;
         return this;
      }

      public ViewModelStubBuilder WithProperties(params IVMPropertyDescriptor[] properties) {
         _descriptorBuilder.WithProperties(properties);
         return this;
      }

      public ViewModelStubBuilder WithBehaviors(params IBehavior[] behaviors) {
         _descriptorBuilder.WithBehaviors(behaviors);
         return this;
      }

      public ViewModelStub Build() {
         _descriptorBuilder.WithBehaviors(new ValidationResultAggregatorStub());
         return new ViewModelStub(_descriptorBuilder.Build(), _description);
      }

      public BehaviorContextStub BuildContext() {
         return new BehaviorContextStub(Build());
      }
   }
}
