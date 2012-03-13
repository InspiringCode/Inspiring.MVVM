namespace Inspiring.Mvvm.Screens {
   using System;
   using System.Collections.Generic;
   using System.Linq;


   public class InitializeEventArgs : ScreenEventArgs {
      public InitializeEventArgs(IScreenBase target)
         : base(target) {
      }

      internal virtual bool CanConvertTo<TBaseSubject>() {
         return false;
      }

      internal virtual InitializeEventArgs<TBaseSubject> ConvertTo<TBaseSubject>() {
         throw new NotSupportedException();
      }
   }

   public class InitializeEventArgs<TSubject> : InitializeEventArgs {
      private readonly TSubject _subject;

      public InitializeEventArgs(IScreenBase target, TSubject subject)
         : base(target) {
         _subject = subject;
      }

      public TSubject Subject {
         get { return _subject; }
      }

      internal override bool CanConvertTo<TBaseSubject>() {
         if (_subject != null) {
            return _subject is TBaseSubject;
         }

         return typeof(TBaseSubject).IsAssignableFrom(typeof(TSubject));
      }

      internal override InitializeEventArgs<TBaseSubject> ConvertTo<TBaseSubject>() {
         return new InitializeEventArgs<TBaseSubject>(
            Target,
            (TBaseSubject)(object)Subject
         );
      }
   }

}
