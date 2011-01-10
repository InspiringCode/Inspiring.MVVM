namespace Inspiring.MvvmTest.ViewModels {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.Mvvm.ViewModels.Core;

   internal sealed class TestVM : ViewModel<TestVMDescriptor> {
      public static readonly new TestVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<TestVMDescriptor>()
         .For<TestVM>()
         .WithProperties((d, c) => {
            var v = c.GetPropertyBuilder();
            var p = c.GetPropertyBuilder(x => x.Source);

            d.CalculatedMutableProperty = p.Property.DelegatesTo(x => x.GetCalculated(), (x, val) => x.SetCalculated(val));
            d.MappedMutableProperty = p.Property.MapsTo(x => x.MappedMutableValue);
            d.LocalProperty = v.Property.Of<decimal>();
            d.MappedVMProperty = p.VM.Wraps(x => x.ChildValue).With<ChildVM>();
            d.MappedCollectionProperty = p.Collection.Wraps(x => x.ChildCollection).With<ChildVM>(ChildVM.ClassDescriptor);
            //d.MappedParentedCollectionProperty = p.Collection.Of<ParentedChildVM>(ParentedChildVM.ClassDescriptor);
            //MappedParentedCollectionProperty = p.Collection(x => x.ChildCollection).Of<ParentedChildVM>(ParentedChildVM.Descriptor)
         })
         .Build();

      public TestVM()
         : base(ClassDescriptor) {
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

      public IVMCollection<ChildVM> MappedCollectionAccessor {
         get { return GetValue(Descriptor.MappedCollectionProperty); }
         set { SetValue(Descriptor.MappedCollectionProperty, value); }
      }

      //public IVMCollection<ParentedChildVM> MappedParentedCollectionAccessor {
      //   get { return GetValue(Descriptor.MappedParentedCollectionProperty); }
      //   set { SetValue(Descriptor.MappedParentedCollectionProperty, value); }
      //}

      public void InvokeUpdateFromSource(IVMPropertyDescriptor property) {
         //UpdateFromSource(property);
      }

      public void InvokeUpdateSource(IVMPropertyDescriptor property) {
         //UpdateSource(property);
      }

      //protected override void OnValidate(_ViewModelValidationArgs args) {
      //   base.OnValidate(args);

      //   if (ViewModelValidationResult != null && !ViewModelValidationResult.Successful) {
      //      args.AddError(ViewModelValidationResult.ErrorMessage);
      //   }
      //}

      //protected override ValidationResult ValidateProperty(VMPropertyBase property) {
      //   return LocalPropertyValidationResult ?? base.ValidateProperty(property);
      //}
   }

   internal sealed class TestVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<int> CalculatedMutableProperty { get; set; }
      public IVMPropertyDescriptor<string> MappedMutableProperty { get; set; }
      public IVMPropertyDescriptor<decimal> LocalProperty { get; set; }
      public IVMPropertyDescriptor<ChildVM> MappedVMProperty { get; set; }
      public IVMPropertyDescriptor<IVMCollection<ChildVM>> MappedCollectionProperty { get; set; }
      //public IVMProperty<IVMCollection<ParentedChildVM>> MappedParentedCollectionProperty { get; set; }
   }


   internal sealed class ChildVM :
      ViewModel<ChildVMDescriptor>,
      ICanInitializeFrom<ChildVMSource>,
      IHasSourceObject<ChildVMSource> {

      public static readonly ChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<ChildVMDescriptor>()
         .For<ChildVM>()
         .WithProperties((d, c) => {
            var v = c.GetPropertyBuilder();
            var p = c.GetPropertyBuilder(x => x.Source);

            d.MappedMutableProperty = p.Property.MapsTo(x => x.MappedMutableValue);
         })
         .WithValidators(c => {
            //c.Check(d.MappedMutableProperty).Custom((_, __) => ValidationResult.Success());
         })
         .Build();

      public ChildVM()
         : base(ClassDescriptor) {
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

      //public void OnNewItem(ItemCreationArguments<ChildVMSource> args, TestVM parent) {
      //   if (args.IsStartNewItem) {
      //      args.NewSoureObject = new ChildVMSource { Parent = parent.Source };
      //      InitializeFrom(args.NewSoureObject);
      //   }
      //}

      //protected override void OnValidate(_ViewModelValidationArgs args) {
      //   base.OnValidate(args);

      //   if (ViewModelValidationResult != null && !ViewModelValidationResult.Successful) {
      //      args.AddError(ViewModelValidationResult.ErrorMessage);
      //   }
      //}

      //protected override ValidationResult ValidateProperty(VMPropertyBase property) {
      //   return MappedMutablePropertyValidationResult ?? base.ValidateProperty(property);
      //}
   }

   internal sealed class ChildVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> MappedMutableProperty { get; set; }
   }

   //internal sealed class ParentedChildVM :
   //   ViewModel<ParentedChildVMDescriptor>,
   //   ICanInitializeFrom<SourceWithParent<TestVM, ChildVMSource>>,
   //   IVMCollectionItem<ChildVMSource> {

   //   public static readonly ParentedChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
   //      .OfType<ParentedChildVMDescriptor>()
   //      .For<ParentedChildVM>()
   //      .WithProperties((d, c) => {
   //         var v = c.GetPropertyBuilder();
   //         var p = c.GetPropertyBuilder(x => x.Source);

   //         d.MappedMutableProperty = p.Property.MapsTo(x => x.Source.MappedMutableValue);
   //      })
   //      .Build();

   //   public ParentedChildVM()
   //      : base() {
   //   }

   //   public void InitializeFrom(SourceWithParent<TestVM, ChildVMSource> source) {
   //      Source = source;
   //   }

   //   public SourceWithParent<TestVM, ChildVMSource> Source { get; private set; }

   //   public ValidationResult ViewModelValidationResult { get; set; }

   //   public ValidationResult MappedMutablePropertyValidationResult { get; set; }

   //   public string MappeddMutableAccessor {
   //      get { return GetValue(Descriptor.MappedMutableProperty); }
   //      set { SetValue(Descriptor.MappedMutableProperty, value); }
   //   }

   //   //public void OnNewItem(ItemCreationArguments<ChildVMSource> args, TestVM parent) {
   //   //   if (args.IsStartNewItem) {
   //   //      args.NewSoureObject = new ChildVMSource { Parent = parent.Source };
   //   //      InitializeFrom(new SourceWithParent<TestVM, ChildVMSource>(parent, args.NewSoureObject));
   //   //   }
   //   //}

   //   //protected override void OnValidate(_ViewModelValidationArgs args) {
   //   //   base.OnValidate(args);

   //   //   if (ViewModelValidationResult != null && !ViewModelValidationResult.Successful) {
   //   //      args.AddError(ViewModelValidationResult.ErrorMessage);
   //   //   }
   //   //}

   //   //protected override ValidationResult ValidateProperty(VMPropertyBase property) {
   //   //   return MappedMutablePropertyValidationResult ?? base.ValidateProperty(property);
   //   //}

   //   ChildVMSource IVMCollectionItem<ChildVMSource>.Source {
   //      get { return Source.Source; }
   //   }

   //   public void InitializeFrom(ChildVMSource source) {
   //      throw new System.NotImplementedException();
   //   }
   //}

   //internal sealed class ParentedChildVMDescriptor : VMDescriptor {
   //   public IVMProperty<string> MappedMutableProperty { get; set; }
   //}


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
