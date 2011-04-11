namespace Inspiring.MvvmTest.ViewModels {
   using System;
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public class ViewModelStub : ViewModel<VMDescriptorBase> {
      /// <summary>
      ///   Use the <see cref="ViewModelStubBuilder"/> to create configured 
      ///   instances.
      /// </summary>
      public ViewModelStub()
         : this(new DescriptorStub()) {
      }

      /// <summary>
      ///   Use the <see cref="ViewModelStubBuilder"/> to create configured 
      ///   instances.
      /// </summary>
      public ViewModelStub(VMDescriptorBase descriptor)
         : base(descriptor) {
         NotifyChangeInvocations = new List<ChangeArgs>();
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
   }

   public class ViewModelStubBuilder {
      private DescriptorStubBuilder _descriptorBuilder = new DescriptorStubBuilder();

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
         return new ViewModelStub(_descriptorBuilder.Build());
      }

      public BehaviorContextStub BuildContext() {
         return new BehaviorContextStub(Build());
      }
   }
}
