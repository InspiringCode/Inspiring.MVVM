namespace Inspiring.MvvmTest.ApiTests.ViewModels.Validation {
   using Inspiring.Mvvm.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class HierarchicalValidationFixture {
      protected GrandparentVM Grandparent { get; private set; }
      protected ParentVM Parent { get; private set; }
      protected ChildVM Child { get; private set; }
      protected ItemVM Item { get; private set; }

      protected ChildVMDescriptor ChildDescriptor { get; private set; }
      protected ItemVMDescriptor ItemDescriptor { get; private set; }

      protected ValidatorMockConfigurationFluent Results { get; private set; }

      [TestInitialize]
      public void BaseSetup() {
         Results = new ValidatorMockConfigurationFluent();
         var initialState = Results.GetState();

         ChildDescriptor = ChildVM.ClassDescriptor;
         ItemDescriptor = ItemVM.ClassDescriptor;

         Item = new ItemVM(Results);
         Child = new ChildVM(Results);
         Parent = new ParentVM(Results);
         Grandparent = new GrandparentVM(Results);

         Child.GetValue(x => x.Items).Add(Item);
         Parent.SetValue(x => x.Child, Child);
         Grandparent.GetValue(x => x.Children).Add(Parent);

         initialState.RestoreToState();
      }


      protected class GrandparentVM : TestViewModel<GrandparentVMDescriptor> {
         public static readonly GrandparentVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<GrandparentVMDescriptor>()
            .For<GrandparentVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Children = v.Collection.Of<ParentVM>(ParentVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.ValidateDescendant(x => x.Children)
                  .ValidateDescendant(x => x.Child)
                  .Check(x => x.ChildProperty).Custom(args => args.Owner.Results.PerformValidation(args));

               b.ValidateDescendant(x => x.Children)
                  .ValidateDescendant(x => x.Child)
                  .ValidateDescendant(x => x.Items)
                  .Check(x => x.ItemProperty)
                  .Custom(args => args.Owner.Results.PerformValidation(args));


               b.ValidateDescendant(x => x.Children)
                  .ValidateDescendant(x => x.Child)
                  .CheckCollection(x => x.Items, x => x.ItemProperty)
                  .Custom<ItemVM>(args => args.Owner.Results.PerformValidation(args));

               b.ValidateDescendant(x => x.Children)
                  .ValidateDescendant(x => x.Child)
                  .CheckCollection(x => x.Items)
                  .Custom(args => args.Owner.Results.PerformValidation(args));


               b.CheckViewModel(args => args.Owner.Results.PerformValidation(args));

               b.ValidateDescendant(x => x.Children)
                  .CheckViewModel(args => args.Owner.Results.PerformValidation(args));

               b.ValidateDescendant(x => x.Children)
                  .ValidateDescendant(x => x.Child)
                  .CheckViewModel(args => args.Owner.Results.PerformValidation(args));

               b.ValidateDescendant(x => x.Children)
                  .ValidateDescendant(x => x.Child)
                  .ValidateDescendant(x => x.Items)
                  .CheckViewModel(args => args.Owner.Results.PerformValidation(args));

            })
            .Build();

         public GrandparentVM(ValidatorMockConfigurationFluent results)
            : base(ClassDescriptor) {
            Results = results;
         }

         private ValidatorMockConfigurationFluent Results { get; set; }
      }

      protected class GrandparentVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<IVMCollection<ParentVM>> Children { get; set; }
      }

      protected class ParentVM : TestViewModel<ParentVMDescriptor> {
         public static readonly ParentVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ParentVMDescriptor>()
            .For<ParentVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.Child = v.VM.Of<ChildVM>();
            })
            .WithValidators(b => {
               b.ValidateDescendant(x => x.Child)
                  .Check(x => x.ChildProperty).Custom(args => args.Owner.Results.PerformValidation(args));

               b.ValidateDescendant(x => x.Child)
                  .ValidateDescendant(x => x.Items)
                  .Check(x => x.ItemProperty)
                  .Custom(args => args.Owner.Results.PerformValidation(args));


               b.ValidateDescendant(x => x.Child)
                  .CheckCollection(x => x.Items, x => x.ItemProperty)
                  .Custom<ItemVM>(args => args.Owner.Results.PerformValidation(args));

               b.ValidateDescendant(x => x.Child)
                  .CheckCollection(x => x.Items)
                  .Custom(args => args.Owner.Results.PerformValidation(args));


               b.CheckViewModel(args => args.Owner.Results.PerformValidation(args));

               b.ValidateDescendant(x => x.Child)
                  .CheckViewModel(args => args.Owner.Results.PerformValidation(args));

               b.ValidateDescendant(x => x.Child)
                  .ValidateDescendant(x => x.Items)
                  .CheckViewModel(args => args.Owner.Results.PerformValidation(args));
            })
            .Build();

         public ParentVM(ValidatorMockConfigurationFluent results)
            : base(ClassDescriptor) {
            Results = results;
         }

         private ValidatorMockConfigurationFluent Results { get; set; }
      }

      protected class ParentVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<ChildVM> Child { get; set; }
      }


      protected class ChildVM : TestViewModel<ChildVMDescriptor> {
         public static readonly ChildVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ChildVMDescriptor>()
            .For<ChildVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();

               d.ChildProperty = v.Property.Of<string>();
               d.Items = v.Collection.Of<ItemVM>(ItemVM.ClassDescriptor);
            })
            .WithValidators(b => {
               b.Check(x => x.ChildProperty)
                  .Custom(args => args.Owner.Results.PerformValidation(args));

               b.ValidateDescendant(x => x.Items)
                  .Check(x => x.ItemProperty)
                  .Custom(args => args.Owner.Results.PerformValidation(args));

               b.CheckCollection(x => x.Items, x => x.ItemProperty)
                  .Custom<ItemVM>(args => args.Owner.Results.PerformValidation(args));

               b.CheckCollection(x => x.Items)
                  .Custom(args => args.Owner.Results.PerformValidation(args));


               b.CheckViewModel(args => args.Owner.Results.PerformValidation(args));

               b.ValidateDescendant(x => x.Items)
                  .CheckViewModel(args => args.Owner.Results.PerformValidation(args));
            })
            .Build();

         public ChildVM(ValidatorMockConfigurationFluent results)
            : base(ClassDescriptor) {
            Results = results;
         }

         public IVMCollection<ItemVM> Items {
            get { return GetValue(Descriptor.Items); }
         }

         private ValidatorMockConfigurationFluent Results { get; set; }
      }

      protected class ChildVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> ChildProperty { get; set; }
         public IVMPropertyDescriptor<IVMCollection<ItemVM>> Items { get; set; }
      }

      protected class ItemVM : TestViewModel<ItemVMDescriptor> {
         public static readonly ItemVMDescriptor ClassDescriptor = VMDescriptorBuilder
            .OfType<ItemVMDescriptor>()
            .For<ItemVM>()
            .WithProperties((d, b) => {
               var v = b.GetPropertyBuilder();
               d.ItemProperty = v.Property.Of<string>();
            })
            .WithValidators(b => {
               b.EnableParentValidation(x => x.ItemProperty);
               b.EnableParentViewModelValidation();

               b.Check(x => x.ItemProperty)
                  .Custom(args => args.Owner.Results.PerformValidation(args));

               b.CheckViewModel(args => args.Owner.Results.PerformValidation(args));
            })
            .Build();

         public ItemVM(ValidatorMockConfigurationFluent results)
            : base(ClassDescriptor) {
            Results = results;
         }

         private ValidatorMockConfigurationFluent Results { get; set; }
      }

      protected class ItemVMDescriptor : VMDescriptor {
         public IVMPropertyDescriptor<string> ItemProperty { get; set; }
      }
   }
}