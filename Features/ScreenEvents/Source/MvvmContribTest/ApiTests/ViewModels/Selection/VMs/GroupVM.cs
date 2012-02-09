namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using System;
   using Inspiring.Mvvm.ViewModels;

   internal sealed class GroupVM : DefaultViewModelWithSourceBase<GroupVMDescriptor, Group> {
      public static readonly GroupVMDescriptor ClassDescriptor = VMDescriptorBuilder
         .OfType<GroupVMDescriptor>()
         .For<GroupVM>()
         .WithProperties((d, c) => {
            var vm = c.GetPropertyBuilder();
            var k = c.GetPropertyBuilder(x => x.Source);

            d.Name = k.Property.MapsTo(x => x.Name);
            d.BirthDate = k.Property.Of<DateTime>();
         })
         .WithValidators(b => {
            b.EnableParentValidation(x => x.Name);
            b.EnableParentViewModelValidation();
         })
         .Build();

      public GroupVM()
         : base(ClassDescriptor) {
      }

      public string Name {
         get { return GetValue(Descriptor.Name); }
      }

      public override string ToString() {
         return Name ?? base.ToString();
      }
   }

   public sealed class GroupVMDescriptor : VMDescriptor {
      public IVMPropertyDescriptor<string> Name { get; set; }
      public IVMPropertyDescriptor<DateTime> BirthDate { get; set; }
   }
}
