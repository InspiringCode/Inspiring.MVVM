namespace SaveDiscard {
   using System.Windows;
   using System.Windows.Controls;

   internal partial class SaveDiscardView : UserControl {
      public static readonly DependencyProperty ActualContentProperty = DependencyProperty.Register(
         "ActualContent",
         typeof(object),
         typeof(SaveDiscardView),
         new PropertyMetadata(HandleActualPropertyChanged)
      );

      public SaveDiscardView() {
         InitializeComponent();
         DataContextChanged += new DependencyPropertyChangedEventHandler(SaveDiscardView_DataContextChanged);
      }

      void SaveDiscardView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
         var x = DataContext as ISaveDiscardScreen;
         if (x != null) {

         }
      }

      

      public object ActualContent {
         get { return (object)GetValue(ActualContentProperty); }
         set { SetValue(ActualContentProperty, value); }
      }

      private static void HandleActualPropertyChanged(
         DependencyObject sender,
         DependencyPropertyChangedEventArgs e
      ) {
         var view = (SaveDiscardView)sender;
         view._actualContent.Content = e.NewValue;
      }
   }
}
