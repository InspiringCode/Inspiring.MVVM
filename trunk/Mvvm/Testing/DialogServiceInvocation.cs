namespace Inspiring.Mvvm.Testing {
   using System;
   using System.Collections.Generic;
   using System.Linq;

   internal sealed class DialogServiceInvocation {
      private List<ArgumentValue> _parameter;

      public DialogServiceInvocation(DialogServiceMethod method) {
         Method = method;
         _parameter.Add(Parent = new ArgumentValue("parent"));
         _parameter.Add(Message = new StringArgumentValue("message"));
         _parameter.Add(Caption = new StringArgumentValue("caption"));
         _parameter.Add(Filter = new StringArgumentValue("filter"));
         _parameter.Add(InitialDirectory = new StringArgumentValue("initialDirectory"));
      }

      public DialogServiceMethod Method { get; private set; }
      public ArgumentValue Parent { get; private set; }
      public StringArgumentValue Message { get; private set; }
      public StringArgumentValue Caption { get; private set; }
      public StringArgumentValue Filter { get; private set; }
      public StringArgumentValue InitialDirectory { get; private set; }

      public override string ToString() {
         return String.Format(
            "{0}({1})",
            Method,
            String.Join(", ", _parameter.Where(x => x.IsSet))
         );
      }
   }
}
