namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class IsValidPropertyChangedTests {
      private ParentVM Parent { get; set; }

      [TestInitialize]
      public void Setup() {
         Parent = new ParentVM();
         Assert.IsTrue(Parent.IsValid);
      }

      [TestMethod]
      public void PropertyChanged_WhenInvalidItemIsAddedToCollection_IsRaised() {
         var counter = new PropertyChangedCounter(Parent, "IsValid");

         Assert.IsTrue(Parent.IsValid);
         Parent.ChildCollection.Add(new ChildVM { ValidationResultToReturn = false });
         Assert.IsFalse(Parent.IsValid);

         counter.AssertOneRaise();
      }

      [TestMethod]
      public void PropertyChanged_WhenInvalidItemIsRemovedFromCollection_IsRaised() {
         Parent.ChildCollection.Add(new ChildVM { ValidationResultToReturn = false });

         var counter = new PropertyChangedCounter(Parent, "IsValid");

         Assert.IsFalse(Parent.IsValid);
         Parent.ChildCollection.Clear();
         Assert.IsTrue(Parent.IsValid);

         counter.AssertOneRaise();
      }

      [TestMethod]
      public void PropertyChanged_WhenViewModelPropertyIsSetToInvalidVM_IsRaised() {
         var counter = new PropertyChangedCounter(Parent, "IsValid");

         Assert.IsTrue(Parent.IsValid);
         Parent.ChildProperty = new ChildVM { ValidationResultToReturn = false }; ;
         Assert.IsFalse(Parent.IsValid);

         counter.AssertOneRaise();
      }

      [TestMethod]
      public void PropertyChanged_WhenViewModelPropertyWithInvalidViewModelIsSetToNull_IsRaised() {
         Parent.ChildProperty = new ChildVM { ValidationResultToReturn = false };
         var counter = new PropertyChangedCounter(Parent, "IsValid");

         Assert.IsFalse(Parent.IsValid);
         Parent.ChildProperty = null;
         Assert.IsTrue(Parent.IsValid);

         counter.AssertOneRaise();
      }

      public sealed class ParentVM : ViewModel<ParentVMDescriptor> {
         public static readonly ParentVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ParentVMDescriptor>()
            .For<ParentVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.ChildProperty = v.VM.Of<ChildVM>();
               d.ChildCollection = v.Collection.Of<ChildVM>(ChildVM.ClassDescriptor);
            })
            .Build();

         public ParentVM()
            : base(ClassDescriptor) {
         }

         public ChildVM ChildProperty {
            get { return GetValue(Descriptor.ChildProperty); }
            set { SetValue(Descriptor.ChildProperty, value); }
         }

         public IVMCollection<ChildVM> ChildCollection {
            get { return GetValue(Descriptor.ChildCollection); }
         }
      }

      public sealed class ParentVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ChildVM> ChildProperty { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ChildVM>> ChildCollection { get; set; }
      }


      public sealed class ChildVM : ViewModel<ChildVMDescriptor> {
         private bool _validationResultToReturn = true;

         public static readonly ChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ChildVMDescriptor>()
            .For<ChildVM>()
            .WithProperties((d, b) => { })
            .WithValidators(b => {
               b.CheckViewModel(args => {
                  if (!args.Owner.ValidationResultToReturn) {
                     args.AddError("Child error");
                  }
               });
            })
            .Build();

         public ChildVM()
            : base(ClassDescriptor) {
         }

         public bool ValidationResultToReturn {
            get { return _validationResultToReturn; }
            set {
               _validationResultToReturn = value;
               Revalidate();
            }
         }
      }

      public sealed class ChildVMDescriptor : VMDescriptor {
      }
   }
}