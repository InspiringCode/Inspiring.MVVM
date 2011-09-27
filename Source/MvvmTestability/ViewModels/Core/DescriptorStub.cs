namespace Inspiring.MvvmTest.ViewModels {
   using System.Collections.Generic;
   using System.Linq;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   public sealed class DescriptorStub : VMDescriptorBase {
      private IVMPropertyDescriptor[] _properties;

      public DescriptorStub(
         IEnumerable<IBehavior> viewModelBehaviors = null,
         IEnumerable<IVMPropertyDescriptor> properties = null
      ) {
         viewModelBehaviors = viewModelBehaviors ?? Enumerable.Empty<IBehavior>();
         properties = properties ?? Enumerable.Empty<IVMPropertyDescriptor>();

         _properties = properties.ToArray();

         IBehavior last = Behaviors;

         foreach (IBehavior b in viewModelBehaviors) {
            last.Successor = b;
            last = b;
         }

         Behaviors.Initialize(this);

         foreach (var p in properties) {
            p.Behaviors.Initialize(this, p);
         }
      }

      public static DescriptorStubBuilder WithProperties(params IVMPropertyDescriptor[] properties) {
         return new DescriptorStubBuilder().WithProperties(properties);
      }

      public static DescriptorStubBuilder WithBehaviors(params IBehavior[] behaviors) {
         return new DescriptorStubBuilder().WithBehaviors(behaviors);
      }

      public static DescriptorStub Build() {
         return new DescriptorStubBuilder().Build();
      }

      protected override VMPropertyCollection DiscoverProperties() {
         return new VMPropertyCollection(_properties);
      }
   }

   public class DescriptorStubBuilder {
      private List<IVMPropertyDescriptor> _properties = new List<IVMPropertyDescriptor>();
      private List<IBehavior> _behaviors = new List<IBehavior>();

      public DescriptorStubBuilder WithProperties(params IVMPropertyDescriptor[] properties) {
         _properties.AddRange(properties);
         return this;
      }

      public DescriptorStubBuilder WithBehaviors(params IBehavior[] behaviors) {
         _behaviors.AddRange(behaviors);
         return this;
      }

      public DescriptorStub Build() {
         _behaviors.Add(new ValidationResultAggregatorStub());
         return new DescriptorStub(_behaviors, _properties);
      }
   }
}
