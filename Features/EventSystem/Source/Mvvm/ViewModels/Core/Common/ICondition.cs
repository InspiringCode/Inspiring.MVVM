namespace Inspiring.Mvvm.ViewModels.Core {

   internal interface ICondition<TOperand> {
      bool IsTrue(TOperand operand);
   }
}
