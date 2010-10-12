namespace Inspiring.MvvmTest.ViewModels {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;

   internal sealed class TestVM : ViewModel<TestVMDescriptor> {
      public static readonly new TestVMDescriptor Descriptor = VMDescriptorBuilder
         .For<TestVM>()
         .CreateDescriptor(c => {
            var v = c.GetPropertyFactory();
            var p = c.GetPropertyFactory(x => x.Source);

            return new TestVMDescriptor {
               CalculatedMutableProperty = p.Calculated(x => x.GetCalculated(), (x, val) => x.SetCalculated(val)),
               MappedMutableProperty = p.Mapped(x => x.MappedMutableValue),
               LocalProperty = v.Local<decimal>(),
               MappedVMProperty = p.MappedVM(x => x.ChildValue).Of<ChildVM>(),
               MappedCollectionProperty = p.MappedCollection(x => x.ChildCollection).Of<ChildVM>(ChildVM.Descriptor),
               MappedParentedCollectionProperty = p.MappedCollection(x => x.ChildCollection).OfParentAware<ParentedChildVM>(ParentedChildVM.Descriptor)
            };
         })
         .Build();

      public TestVM()
         : base(Descriptor) {
      }

      public ValidationResult ViewModelValidationResult { get; set; }

      public ValidationResult LocalPropertyValidationResult { get; set; }

      public TestVMSource Source { get; set; }

      public int CalculatedMutableAccessor {
         get { return GetValue(Descriptor.CalculatedMutableProperty); }
         set { SetValue(Descriptor.CalculatedMutableProperty, value); }
      }

      public string MappedMutableAccessor {
         get { return GetValue(Descriptor.MappedMutableProperty); }
         set { SetValue(Descriptor.MappedMutableProperty, value); }
      }

      public decimal LocalAccessor {
         get { return GetValue(Descriptor.LocalProperty); }
         set { SetValue(Descriptor.LocalProperty, value); }
      }

      public ChildVM MappedVMAccessor {
         get { return GetValue(Descriptor.MappedVMProperty); }
         set { SetValue(Descriptor.MappedVMProperty, value); }
      }

      public VMCollection<ChildVM> MappedCollectionAccessor {
         get { return GetValue(Descriptor.MappedCollectionProperty); }
         set { SetValue(Descriptor.MappedCollectionProperty, value); }
      }

      public VMCollection<ParentedChildVM> MappedParentedCollectionAccessor {
         get { return GetValue(Descriptor.MappedParentedCollectionProperty); }
         set { SetValue(Descriptor.MappedParentedCollectionProperty, value); }
      }

      public void InvokeUpdateFromSource(VMProperty property) {
         UpdateFromSource(property);
      }

      public void InvokeUpdateSource(VMProperty property) {
         UpdateSource(property);
      }

      protected override ValidationResult Validate() {
         return ViewModelValidationResult ?? base.Validate();
      }

      protected override ValidationResult ValidateProperty(VMProperty property) {
         return LocalPropertyValidationResult ?? base.ValidateProperty(property);
      }
   }

   internal sealed class TestVMDescriptor : VMDescriptor {
      public VMProperty<int> CalculatedMutableProperty { get; set; }
      public VMProperty<string> MappedMutableProperty { get; set; }
      public VMProperty<decimal> LocalProperty { get; set; }
      public VMProperty<ChildVM> MappedVMProperty { get; set; }
      public VMCollectionProperty<ChildVM> MappedCollectionProperty { get; set; }
      public VMCollectionProperty<ParentedChildVM> MappedParentedCollectionProperty { get; set; }
   }


   internal sealed class ChildVM :
      ViewModel<ChildVMDescriptor>,
      ICanInitializeFrom<ChildVMSource>,
      ICreatableItem<TestVM, ChildVMSource>,
      IHasSourceObject<ChildVMSource> {

      public static readonly ChildVMDescriptor Descriptor = VMDescriptorBuilder
         .For<ChildVM>()
         .CreateDescriptor(c => {
            var v = c.GetPropertyFactory();
            var p = c.GetPropertyFactory(x => x.Source);

            return new ChildVMDescriptor {
               MappedMutableProperty = p.Mapped(x => x.MappedMutableValue)
            };
         })
         .WithValidations((d, c) => {
            c.Check(d.MappedMutableProperty).Custom((_, __) => ValidationResult.Success());
         })
         .Build();

      public ChildVM()
         : base(Descriptor) {
      }

      public void InitializeFrom(ChildVMSource source) {
         Source = source;
      }

      public ChildVMSource Source { get; private set; }


      public ValidationResult ViewModelValidationResult { get; set; }

      public ValidationResult MappedMutablePropertyValidationResult { get; set; }

      public string MappeddMutableAccessor {
         get { return GetValue(Descriptor.MappedMutableProperty); }
         set { SetValue(Descriptor.MappedMutableProperty, value); }
      }

      public void OnNewItem(ItemCreationArguments<ChildVMSource> args, TestVM parent) {
         if (args.IsStartNewItem) {
            args.NewSoureObject = new ChildVMSource { Parent = parent.Source };
            InitializeFrom(args.NewSoureObject);
         }
      }


      protected override ValidationResult Validate() {
         return ViewModelValidationResult ?? base.Validate();
      }

      protected override ValidationResult ValidateProperty(VMProperty property) {
         return MappedMutablePropertyValidationResult ?? base.ValidateProperty(property);
      }
   }

   internal sealed class ChildVMDescriptor : VMDescriptor {
      public VMProperty<string> MappedMutableProperty { get; set; }
   }

   internal sealed class ParentedChildVM :
      ViewModel<ParentedChildVMDescriptor>,
      ICanInitializeFrom<SourceWithParent<TestVM, ChildVMSource>>,
      ICreatableItem<TestVM, ChildVMSource>,
      IHasSourceObject<ChildVMSource> {

      public static readonly ParentedChildVMDescriptor Descriptor = VMDescriptorBuilder
         .For<ParentedChildVM>()
         .CreateDescriptor(c => {
            var v = c.GetPropertyFactory();
            var p = c.GetPropertyFactory(x => x.Source);

            return new ParentedChildVMDescriptor {
               MappedMutableProperty = p.Mapped(x => x.Source.MappedMutableValue)
            };
         })
         .Build();

      public ParentedChildVM()
         : base(Descriptor) {
      }

      public void InitializeFrom(SourceWithParent<TestVM, ChildVMSource> source) {
         Source = source;
      }

      public SourceWithParent<TestVM, ChildVMSource> Source { get; private set; }

      public ValidationResult ViewModelValidationResult { get; set; }

      public ValidationResult MappedMutablePropertyValidationResult { get; set; }

      public string MappeddMutableAccessor {
         get { return GetValue(Descriptor.MappedMutableProperty); }
         set { SetValue(Descriptor.MappedMutableProperty, value); }
      }

      public void OnNewItem(ItemCreationArguments<ChildVMSource> args, TestVM parent) {
         if (args.IsStartNewItem) {
            args.NewSoureObject = new ChildVMSource { Parent = parent.Source };
            InitializeFrom(new SourceWithParent<TestVM, ChildVMSource>(parent, args.NewSoureObject));
         }
      }


      protected override ValidationResult Validate() {
         return ViewModelValidationResult ?? base.Validate();
      }

      protected override ValidationResult ValidateProperty(VMProperty property) {
         return MappedMutablePropertyValidationResult ?? base.ValidateProperty(property);
      }

      ChildVMSource IHasSourceObject<ChildVMSource>.Source {
         get { return Source.Source; }
      }
   }

   internal sealed class ParentedChildVMDescriptor : VMDescriptor {
      public VMProperty<string> MappedMutableProperty { get; set; }
   }


   internal class TestVMSource {
      private List<ChildVMSource> _children = new List<ChildVMSource>();

      public int CalculatedMutableValue { get; private set; }
      public string MappedMutableValue { get; set; }
      public ChildVMSource ChildValue { get; set; }
      public IEnumerable<ChildVMSource> ChildCollection {
         get { return _children; }
      }

      public int GetCalculated() {
         return CalculatedMutableValue;
      }

      public void SetCalculated(int value) {
         CalculatedMutableValue = value;
      }

      public void AddChild(ChildVMSource child) {
         _children.Add(child);
      }
   }

   internal class ChildVMSource {
      public string MappedMutableValue { get; set; }

      public TestVMSource Parent { get; set; }
   }
}
