namespace Inspiring.Mvvm.ViewModels.Core {
   public sealed class ValueStage {
      public static readonly ValueStage DisplayValue = new ValueStage(0x00FF);
      public static readonly ValueStage Value = new ValueStage(0x0FFF);
      public static readonly ValueStage ValidatedValue = new ValueStage(0xFFFF);

      private ValueStage(int sequence) {
         Sequence = sequence;
      }

      public int Sequence { get; private set; }
   }
}
