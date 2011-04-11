namespace Inspiring.MvvmContribTest.ViewModels {
   using System.Collections.Generic;
   using Inspiring.Mvvm.ViewModels;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class ReverseOneToManyCollectionTests : TestBase {
      private Group Admins { get; set; }
      private Group Operators { get; set; }

      private User John { get; set; }
      private User Tim { get; set; }
      private User Jane { get; set; }

      [TestInitialize]
      public void Setup() {
         Admins = new Group("Admins");
         Operators = new Group("Operators");

         John = new User("John");
         Tim = new User("Tim");
         Jane = new User("Jane");
      }

      [TestMethod]
      public void Constructor_PopulatesCollectionWithCorrectTargetObjects() {
         John.Groups.Add(Admins);
         John.Groups.Add(Operators);

         Tim.Groups.Add(Admins);

         Jane.Groups.Add(Operators);

         var collection = CreateCollection(Admins);
         var expectedUsers = new[] { John, Tim };

         CollectionAssert.AreEquivalent(expectedUsers, collection);
      }

      [TestMethod]
      public void Add_TargetObject_AddsSourceObjectToTargetObject() {
         Jane.Groups.Add(Operators);

         var collection = CreateCollection(Admins);
         collection.Add(Jane);

         CollectionAssert.AreEquivalent(
            new[] { Operators, Admins },
            Jane.Groups
         );
      }

      [TestMethod]
      public void Remove_TargetObject_RemovesSourceObjectFromTargetObject() {
         Jane.Groups.Add(Admins);
         Jane.Groups.Add(Operators);

         var collection = CreateCollection(Admins);
         collection.Remove(Jane);

         CollectionAssert.AreEquivalent(
            new[] { Operators },
            Jane.Groups
         );
      }

      [TestMethod]
      public void Clear_RemovesSourceObjectFromAllTargetObjects() {
         Jane.Groups.Add(Admins);

         John.Groups.Add(Admins);
         John.Groups.Add(Operators);

         var collection = CreateCollection(Admins);
         collection.Clear();

         CollectionAssert.AreEquivalent(new Group[] { }, Jane.Groups);
         CollectionAssert.AreEquivalent(new[] { Operators }, John.Groups);
      }

      [TestMethod]
      public void Set_RemovesAndAddsSourceObject() {
         var collection = CreateCollection(Admins);
         collection.Add(Jane);
         collection[0] = John;

         CollectionAssert.AreEquivalent(new Group[] { }, Jane.Groups);
         CollectionAssert.AreEquivalent(new[] { Admins }, John.Groups);
      }

      private ReverseOneToManyCollection<Group, User> CreateCollection(Group sourceObject) {
         return new ReverseOneToManyCollection<Group, User>(
            sourceObject,
            new User[] { John, Tim, Jane },
            x => x.Groups
         );
      }

      private class User {
         public User(string name) {
            Name = name;
            Groups = new List<Group>();
         }
         public string Name { get; set; }
         public List<Group> Groups { get; set; }
      }

      private class Group {
         public Group(string name) {
            Name = name;
         }
         public string Name { get; set; }
      }
   }
}