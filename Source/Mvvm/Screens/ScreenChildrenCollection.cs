namespace Inspiring.Mvvm.Screens {
   using System.Collections.ObjectModel;
   using Inspiring.Mvvm.Common;

   public class ScreenChildrenCollection<T> :
      AbstractCollection<T> {

      private readonly EventAggregator _aggregator;
      private readonly IScreenBase _parent;

      internal ScreenChildrenCollection(EventAggregator aggregator, IScreenBase parent)
         : this(new ObservableCollection<T>()) {
         _aggregator = aggregator;
         _parent = parent;
      }

      private ScreenChildrenCollection(ObservableCollection<T> observableItems)
         : base(inner: observableItems) {
         ObservableItems = observableItems;
      }

      internal ObservableCollection<T> ObservableItems { get; private set; }

      public TScreen AddScreen<TScreen>(IScreenFactory<TScreen> screen)
         where TScreen : T, IScreenBase {
         TScreen s = screen.Create(_aggregator);

         // The screen is added AFTER it was initialized by the screen
         // factory.
         Attach(s);

         return s;
      }

      public void Attach(T service) {
         AddCore(service);
      }

      public void Remove(T item) {
         RemoveCore(item);
      }

      protected override void AddCore(T item) {
         IScreenBase s = item as IScreenBase;

         if (s != null && s.Parent == null) {
            s.Parent = _parent;
         }

         base.AddCore(item);
      }

      protected override bool RemoveCore(T item) {
         bool result = base.RemoveCore(item);

         IScreenBase s = item as IScreenBase;

         if (s != null && s.Parent == _parent) {
            s.Parent = null;
         }

         return result;
      }
   }
}
