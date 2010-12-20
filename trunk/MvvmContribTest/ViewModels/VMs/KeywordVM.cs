namespace Inspiring.MvvmContribTest.ViewModels {
   using Inspiring.Mvvm.ViewModels;

   internal sealed class KeywordVM : ViewModel<KeywordVMDescriptor>, ICanInitializeFrom<Keyword>, IVMCollectionItem<Keyword> {
      public static readonly KeywordVMDescriptor Descriptor = VMDescriptorBuilder
         .For<KeywordVM>()
         .CreateDescriptor(c => {
            var vm = c.GetPropertyBuilder();
            var k = c.GetPropertyBuilder(x => x.Keyword);

            return new KeywordVMDescriptor {
               Name = k.Property.MapsTo(x => x.Name)
            };
         })
         .Build();

      public KeywordVM()
         : base() {
      }

      public Keyword Keyword { get; private set; }

      public void InitializeFrom(Keyword source) {
         Keyword = source;
      }

      Keyword IVMCollectionItem<Keyword>.Source {
         get { return Keyword; }
      }
   }

   public sealed class KeywordVMDescriptor : VMDescriptor {
      public VMProperty<string> Name { get; set; }
   }
}
