namespace Inspiring.MvvmContribTest.ApiTests.ViewModels {
   using System.Collections.Generic;
   using Inspiring.MvvmTest.ViewModels;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class MultiSelectionTest : TestBase {
      private Group _firstKeyword;
      private Group _secondKeyword;
      private Group _thirdKeyword;
      private Group _fourthKeyword;

      private List<Group> _allKeywords;

      private User _document;
      private UserVM _vm;

      //[TestInitialize]
      //public void Setup() {
      //   _firstKeyword = new Group("Keyword 1");
      //   _secondKeyword = new Group("Keyword 2");
      //   _thirdKeyword = new Group("Keyword 3");
      //   _fourthKeyword = new Group("Keyword 4");

      //   List<Group> allKeywords = new List<Group> {
      //      _firstKeyword,
      //      _secondKeyword,
      //      _thirdKeyword,
      //      _fourthKeyword
      //   };

      //   _document = new User("Document");
      //   _document.Groups.Add(_firstKeyword);
      //   _document.Groups.Add(_secondKeyword);

      //   _vm = new UserVM();
      //   _vm.InitializeFrom(_document);
      //   _vm.Groups.AllSourceItems = allKeywords;
      //}

      //[TestMethod]
      //public void TestInitiallySelectedItems() {
      //   Group[] actualItems = _vm
      //      .Groups
      //      .SelectedItems
      //      .Select(x => x.GroupSource)
      //      .ToArray();

      //   CollectionAssert.AreEquivalent(_document.Groups.ToArray(), actualItems);
      //}

      //[TestMethod]
      //public void TestAddNewSelectedItem() {
      //   GroupVM thirdKeywordVM = _vm
      //      .Groups
      //      .AllItems
      //      .Single(x => x.GroupSource == _thirdKeyword);

      //   _vm.Groups.SelectedItems.Add(thirdKeywordVM);

      //   CollectionAssert.AreEquivalent(
      //      new Group[] { _firstKeyword, _secondKeyword, _thirdKeyword },
      //      _document.Groups.ToArray()
      //   );
      //}

      //[TestMethod]
      //public void TestRemoveSelectedItem() {
      //   GroupVM firstKeywordVM = _vm
      //      .Groups
      //      .AllItems
      //      .Single(x => x.GroupSource == _firstKeyword);

      //   _vm.Groups.SelectedItems.Remove(firstKeywordVM);

      //   CollectionAssert.AreEquivalent(
      //      new Group[] { _secondKeyword },
      //      _document.Groups.ToArray()
      //   );
      //}
   }
}