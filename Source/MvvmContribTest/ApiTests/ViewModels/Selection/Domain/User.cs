﻿namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using System;
   using System.Collections.Generic;

   internal sealed class User {
      private Department _department;

      public User(string name = null) {
         Name = name ?? "User";
         Groups = new List<Group>();
      }

      public User(params Group[] groups) {
         Groups = new List<Group>(groups);
      }

      public string Name { get; set; }

      public Department Department {
         get { return _department; }
         set { _department = value; }
      }

      public ICollection<Group> Groups { get; private set; }

      public override string ToString() {
         return String.Format("{{User: {0}}}", Name);
      }
   }
}
