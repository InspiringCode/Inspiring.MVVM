namespace Inspiring.Mvvm.Common {

   public interface IEventCondition {
      bool IsTrue(object payload);
   }
}
