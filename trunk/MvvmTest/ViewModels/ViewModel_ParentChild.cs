namespace Inspiring.MvvmTest.ViewModels {

   //[TestClass]
   //public class ViewModel_ParentChild {
   //   [TestMethod]
   //   public void OnChildChanged_ExpectImmedateParentGetsNotified() {
   //      // Arrange
   //      var child = new ChildVM();
   //      IBehaviorContext childContext = child;

   //      var parentMock = new Mock<ParentVM>();
   //      parentMock.CallBase = true;
   //      parentMock.Object.Children.Add(child);

   //      // Act
   //      childContext.RaisePropertyChanged(ChildVM.Descriptor.StringProperty);

   //      // Assert
   //      parentMock.Verify(
   //         x => x.OnChildChangedHook<string>(child, ChildVM.Descriptor.StringProperty),
   //         Times.Once()
   //      );
   //   }

   //   [TestMethod]
   //   public void OnChildChanged_ExpectGrandparentGetsNotified() {
   //      // Arrange
   //      var child = new ChildVM();
   //      IBehaviorContext childContext = child;

   //      var parent = new ParentVM();
   //      parent.Children.Add(child);

   //      var grandParentMock = new Mock<ParentParentVM>();
   //      grandParentMock.CallBase = true;
   //      grandParentMock.Object.Children.Add(parent);

   //      // Act
   //      childContext.RaisePropertyChanged(ChildVM.Descriptor.StringProperty);

   //      // Assert
   //      grandParentMock.Verify(
   //         x => x.OnChildChangedHook<string>(child, ChildVM.Descriptor.StringProperty),
   //         Times.Once()
   //      );
   //   }

   //   [TestMethod]
   //   public void OnChildValidationStateChanged_ExpectImmediateParentGetsNotified() {
   //      // Arrange
   //      var child = new ChildVM();
   //      IBehaviorContext childContext = child;

   //      var parentMock = new Mock<ParentVM>();
   //      parentMock.CallBase = true;
   //      parentMock.Object.Children.Add(child);

   //      // Act
   //      childContext.RaiseValidationStateChanged(ChildVM.Descriptor.StringProperty);

   //      // Assert
   //      parentMock.Verify(
   //         x => x.OnChildValidationStateChangedHook<string>(child, ChildVM.Descriptor.StringProperty),
   //         Times.Once()
   //      );
   //   }

   //   [TestMethod]
   //   public void OnChildValidationStateChanged_ExpectGrandparentGetsNotified() {
   //      // Arrange
   //      var child = new ChildVM();
   //      IBehaviorContext childContext = child;

   //      var parent = new ParentVM();
   //      parent.Children.Add(child);

   //      var grandParentMock = new Mock<ParentParentVM>();
   //      grandParentMock.CallBase = true;
   //      grandParentMock.Object.Children.Add(parent);

   //      // Act
   //      childContext.RaiseValidationStateChanged(ChildVM.Descriptor.StringProperty);

   //      // Assert
   //      grandParentMock.Verify(
   //         x => x.OnChildValidationStateChangedHook<string>(child, ChildVM.Descriptor.StringProperty),
   //         Times.Once()
   //      );
   //   }

   //   public class ParentParenTVM : IViewModel<ParentParentVMDescriptor> {
   //      public static readonly ParentParentVMDescriptor Descriptor = VMDescriptorBuilder
   //         .For<ParentParentVM>()
   //         .CreateDescriptor(c => {
   //            var vm = c.GetPropertyFactory();

   //            return new ParentParentVMDescriptor {
   //               Children = vm.Local<VMCollection<ParentVM>>()
   //            };
   //         })
   //         .Build();

   //      public ParentParentVM()
   //         : base(Descriptor) {
   //         SetValue(Descriptor.Children, new VMCollection<ParentVM>(this, ParentVM.Descriptor));
   //      }

   //      public VMCollection<ParentVM> Children {
   //         get { return GetValue(Descriptor.Children); }
   //      }

   //      protected override void OnChildChanged<T>(ViewModel changedChild, VMPropertyBase<T> changedProperty) {
   //         base.OnChildChanged<T>(changedChild, changedProperty);
   //         OnChildChangedHook(changedChild, changedProperty);
   //      }

   //      protected override void OnChildValidationStateChanged<T>(ViewModel changedChild, VMPropertyBase<T> changedProperty) {
   //         base.OnChildValidationStateChanged<T>(changedChild, changedProperty);
   //         OnChildValidationStateChangedHook(changedChild, changedProperty);
   //      }

   //      public virtual void OnChildChangedHook<T>(ViewModel changedChild, VMPropertyBase<T> changedProperty) {
   //      }

   //      public virtual void OnChildValidationStateChangedHook<T>(ViewModel changedChild, VMPropertyBase<T> changedProperty) {
   //      }
   //   }

   //   public class ParenTVM : IViewModel<ParentVMDescriptor> {
   //      public static readonly ParentVMDescriptor Descriptor = VMDescriptorBuilder
   //         .For<ParentVM>()
   //         .CreateDescriptor(c => {
   //            var v = c.GetPropertyFactory();

   //            return new ParentVMDescriptor {
   //               Children = v.MappedCollection(x => x.Source).Of<ChildVM>(ChildVM.Descriptor)
   //            };
   //         })
   //         .Build();

   //      public ParentVM()
   //         : base(Descriptor) {
   //         Source = new List<string>();
   //      }

   //      public VMCollection<ChildVM> Children {
   //         get { return GetValue(Descriptor.Children); }
   //         set { SetValue(Descriptor.Children, value); }
   //      }

   //      private List<string> Source { get; set; }

   //      protected override void OnChildChanged<T>(ViewModel changedChild, VMPropertyBase<T> changedProperty) {
   //         base.OnChildChanged<T>(changedChild, changedProperty);
   //         OnChildChangedHook(changedChild, changedProperty);
   //      }

   //      protected override void OnChildValidationStateChanged<T>(ViewModel changedChild, VMPropertyBase<T> changedProperty) {
   //         base.OnChildValidationStateChanged<T>(changedChild, changedProperty);
   //         OnChildValidationStateChangedHook(changedChild, changedProperty);
   //      }

   //      public virtual void OnChildChangedHook<T>(ViewModel changedChild, VMPropertyBase<T> changedProperty) {
   //      }

   //      public virtual void OnChildValidationStateChangedHook<T>(ViewModel changedChild, VMPropertyBase<T> changedProperty) {
   //      }
   //   }

   //   public class ChildVM : ViewModel<ChildVMDescriptor>, ICanInitializeFrom<string>, IHasSourceObject<string> {
   //      public static readonly ChildVMDescriptor Descriptor = VMDescriptorBuilder
   //         .For<ChildVM>()
   //         .CreateDescriptor(c => {
   //            var v = c.GetPropertyFactory();

   //            return new ChildVMDescriptor {
   //               StringProperty = v.Local<string>()
   //            };
   //         })
   //         .Build();

   //      public ChildVM()
   //         : base(Descriptor) {
   //      }

   //      public string StringProperty {
   //         get { return GetValue(Descriptor.StringProperty); }
   //         set { SetValue(Descriptor.StringProperty, value); }
   //      }

   //      public void InitializeFrom(string source) {
   //         StringProperty = source;
   //      }

   //      public string Source {
   //         get { return GetValue(Descriptor.StringProperty); }
   //      }
   //   }

   //   public class ParentVMDescriptor : VMDescriptor {
   //      public VMCollectionProperty<ChildVM> Children { get; set; }
   //   }

   //   public class ChildVMDescriptor : VMDescriptor {
   //      public VMProperty<string> StringProperty { get; set; }
   //   }

   //   public class ParentParentVMDescriptor : VMDescriptor {
   //      public VMProperty<VMCollection<ParentVM>> Children { get; set; }
   //   }
   //}
}
