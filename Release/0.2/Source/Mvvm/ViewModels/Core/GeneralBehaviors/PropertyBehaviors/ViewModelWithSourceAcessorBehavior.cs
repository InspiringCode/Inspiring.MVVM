namespace Inspiring.Mvvm.ViewModels.Core {

   internal sealed class ViewModelWithSourceAcessorBehavior<TVM, TSource> :
      Behavior, IValueAccessorBehavior<TVM>
      where TVM : IViewModel, IHasSourceObject<TSource> {

      public TVM GetValue(IBehaviorContext context) {
         var factory = GetNextBehavior<IViewModelFactoryBehavior<TVM>>();

         TVM instance = factory.CreateInstance(context);

         TSource sourceValue = this.GetValueNext<TSource>(context);
         instance.Source = sourceValue;

         // TODO: This and same line in DelegatesTo (PropertyBuilder) is a hack!
         instance.Kernel.Revalidate(ValidationScope.SelfOnly, ValidationMode.CommitValidValues);

         return instance;
      }

      public void SetValue(IBehaviorContext context, TVM value) {
         this.SetValueNext<TSource>(context, value.Source);
      }
   }
}
