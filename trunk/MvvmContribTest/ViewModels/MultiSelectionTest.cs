namespace Inspiring.MvvmContribTest.ViewModels {
   using System.Collections.Generic;
   using System.Linq;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

   [TestClass]
   public class MultiSelectionTest {
      private Keyword _firstKeyword;
      private Keyword _secondKeyword;
      private Keyword _thirdKeyword;
      private Keyword _fourthKeyword;

      private List<Keyword> _allKeywords;

      private Document _document;
      private DocumentVM _vm;

      [TestInitialize]
      public void Setup() {
         _firstKeyword = new Keyword("Keyword 1");
         _secondKeyword = new Keyword("Keyword 2");
         _thirdKeyword = new Keyword("Keyword 3");
         _fourthKeyword = new Keyword("Keyword 4");

         List<Keyword> allKeywords = new List<Keyword> {
            _firstKeyword,
            _secondKeyword,
            _thirdKeyword,
            _fourthKeyword
         };

         _document = new Document("Document");
         _document.Keywords.Add(_firstKeyword);
         _document.Keywords.Add(_secondKeyword);

         _vm = new DocumentVM();
         _vm.InitializeFrom(_document);
         _vm.Keywords.AllSourceItems = allKeywords;
      }

      [TestMethod]
      public void TestInitiallySelectedItems() {
         Keyword[] actualItems = _vm
            .Keywords
            .SelectedItems
            .Select(x => x.Keyword)
            .ToArray();

         CollectionAssert.AreEquivalent(_document.Keywords.ToArray(), actualItems);
      }

      [TestMethod]
      public void TestAddNewSelectedItem() {
         KeywordVM thirdKeywordVM = _vm
            .Keywords
            .AllItems
            .Single(x => x.Keyword == _thirdKeyword);

         _vm.Keywords.SelectedItems.Add(thirdKeywordVM);

         CollectionAssert.AreEquivalent(
            new Keyword[] { _firstKeyword, _secondKeyword, _thirdKeyword },
            _document.Keywords.ToArray()
         );
      }

      [TestMethod]
      public void TestRemoveSelectedItem() {
         KeywordVM firstKeywordVM = _vm
            .Keywords
            .AllItems
            .Single(x => x.Keyword == _firstKeyword);

         _vm.Keywords.SelectedItems.Remove(firstKeywordVM);

         CollectionAssert.AreEquivalent(
            new Keyword[] { _secondKeyword },
            _document.Keywords.ToArray()
         );
      }
   }
}