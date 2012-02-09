namespace Inspiring.Mvvm.Common {

   public interface IEventCondition<TPayload> {
      bool IsTrue(TPayload payload);
   }
}
